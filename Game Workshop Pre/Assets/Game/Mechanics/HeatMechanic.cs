using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeatMechanic : MonoBehaviour
{
    //Constants 
    public const int LOWEST_HEAT_VALUE = 0;
    public const int HIGHEST_HEAT_VALUE = 100;


    // Properties
    [Tooltip("The current heat value of the object. View only.")]
    [ReadOnly]
    [SerializeField, Range(LOWEST_HEAT_VALUE, HIGHEST_HEAT_VALUE)] private float _heat = 20;
    public int Heat
    {
        get { return Mathf.RoundToInt(_heat); }
    }

    [Tooltip("The rate per second at which heat returns to room temperature.")]
    [SerializeField] private float _heatRelaxationRate = 0.5f;
    [Tooltip("The heat level at which this object begins to glow.")]
    [SerializeField] private int _warningThreshold = 60;

    [Tooltip("The heat level at which this object ignites into flame.")]
    [SerializeField] private int _ignitionThreshold = 80;

    [Tooltip("The time spent past the ignition threshold until this object ignites.")]
    [SerializeField] private float _ignitionTime = 1f;

    [Tooltip("The maximum rate per second at which this object flashes.")]
    [SerializeField] private float _maxFlashFrequency = 10f;


    private float _flashPhase = 0f; // This is the time spent in the warning threshold. Used for shader.
    private float _ignitionTimer = 0f; // Current time until ignition. Ignites when reaches _ignitionTime
    private bool _hasIgnited = false; // Whether or not this object has ignited and should currently be burning up.

    // Components
    private Room _currentRoom;
    private SpriteRenderer _spriteRenderer;
    private MaterialPropertyBlock _block;


    // Unity methods
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _block = new MaterialPropertyBlock();
    }

    private void Start()
    {
        _heat = DistrictManager.Instance.Temperature;
    }

    private void Update()
    {
        RelaxHeat();
        UpdateHeatShader();
        CheckForIgnition();
    }


    // Room subscribing
    public void EnterRoom(Room room)
    {
        _currentRoom = room;
    }

    // Room unsubscribing
    public void ExitRoom(Room room)
    {
        if (_currentRoom == room)
        {
            _currentRoom = null;
        }
    }

    public void Reset()
    {
        _flashPhase = 0f;
        _ignitionTimer = 0f;
        _hasIgnited = false;
        _heat = DistrictManager.Instance.Temperature;
    }

    // Heat mechanics
    //call to set heat
    public void ModifyHeat(float change)
    {
        _heat = Mathf.Clamp(_heat + change, LOWEST_HEAT_VALUE, HIGHEST_HEAT_VALUE);
    }

    private void RelaxHeat()
    {
        int roomTemperature = DistrictManager.Instance.Temperature;
        if (_currentRoom != null) roomTemperature = _currentRoom.Temperature;

        if (_heat != roomTemperature)
        {
            _heat = Mathf.MoveTowards(_heat, roomTemperature, _heatRelaxationRate * Time.deltaTime);
        }
    }

    private void CheckForIgnition()
    {
        if (_heat >= _ignitionThreshold)
        {
            if (!_hasIgnited) _ignitionTimer += Time.deltaTime;
        }
        else
        {
            _ignitionTimer = 0f;
            //_hasIgnited = false; 
            // Currently there is no way to unignite.
            // Once ignited, this component considers itself burnt to a crisp and wont ignite again.
            // Unless it is Reset()
            // Might need to change this later?
        }

        if (_ignitionTimer >= _ignitionTime && !_hasIgnited)
        {
            _hasIgnited = true;
            IHeatable[] heatables = GetComponents<MonoBehaviour>().OfType<IHeatable>().ToArray();
            foreach (IHeatable heatable in heatables)
            {
                heatable.OnIgnite(this);
            }
        }
    }



    // update shader... Maybe move all the shader stuff into a shader controller if it becomes more than just heat related
    private void UpdateHeatShader()
    {
        // Get heat from _warningThreshold to _ignitionThreshold as 0-1 float 
        float heat01 = Mathf.Clamp01((_heat - _warningThreshold) / (_ignitionThreshold - _warningThreshold));

        // If above warning threshold, flash according to frequency and heat
        if (_heat >= _warningThreshold)
        {
            float heatFlashRamp = 2f; // This can be serialized if we move
            float xExp = Mathf.Pow(heat01, heatFlashRamp);
            float flashSpeed = Mathf.Lerp(0, _maxFlashFrequency, xExp);
            _flashPhase += Time.deltaTime * flashSpeed;
        }
        else
        {
            _flashPhase = 0f;
        }

        _spriteRenderer.GetPropertyBlock(_block);
        _block.SetFloat("_Heat", heat01);
        _block.SetFloat("_FlashPhase", _flashPhase);
        _spriteRenderer.SetPropertyBlock(_block);
    }



}
