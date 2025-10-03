using System.Collections;
using System.Collections.Generic;
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
        set { _heat = Mathf.Clamp(value, LOWEST_HEAT_VALUE, HIGHEST_HEAT_VALUE); }
    }

    [Tooltip("The rate per second at which heat returns to room temperature.")]
    [SerializeField] private float _heatRelaxationRate = 0.5f;
    [Tooltip("The heat level at which this object begins to glow.")]
    [SerializeField] private int _warningThreshold = 60;

    [Tooltip("The heat level at which this object ignites into flame.")]
    [SerializeField] private int _ignitionThreshold = 80;


    [Tooltip("The maximum rate per second at which this object flashes.")]
    [SerializeField] private float _maxFlashFrequency = 10f;

    private float _flashPhase; // This is the time spent in the warning threshold. Used for shader.


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
        SetHeatToRoom();
    }

    private void Update()
    {
        RelaxHeat();
        UpdateHeatShader();
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


    // Heat mechanics
    //call to set heat
    public void ModifyHeat(int change)
    {
        _heat = Mathf.Clamp(_heat + change, LOWEST_HEAT_VALUE, HIGHEST_HEAT_VALUE);
    }

    private void SetHeatToRoom()
    {
        int roomTemperature = DistrictManager.Instance.Temperature;
        if (_currentRoom != null) roomTemperature = _currentRoom.Temperature;
        _heat = roomTemperature;
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
