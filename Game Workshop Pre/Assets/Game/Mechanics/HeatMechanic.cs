
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class HeatMechanic : MonoBehaviour
{

    //Constants 
    public const int LOWEST_HEAT_VALUE = 0;
    public const int HIGHEST_HEAT_VALUE = 100;


    // Properties
    [Tooltip("The current heat value of the object. View only.")]
    [ReadOnly]
    [SerializeField, Range(LOWEST_HEAT_VALUE, HIGHEST_HEAT_VALUE)] private float _heat = 20;
    public float Heat
    {
        get { return _heat; }
    }
    public bool coolingOff = false;

    [Tooltip("The rate per second at which heat returns to room temperature.")]
    [SerializeField] private float _heatRelaxationRate = 0.5f;
    [Tooltip("The time after heat changes until it begins to relax.")]
    [SerializeField] private float _timeBeforeRelax = 1f;
    [Tooltip("The heat level at which this object begins to glow.")]
    [SerializeField] private int _warningThreshold = 60;

    [Tooltip("The heat level at which this object ignites into flame.")]
    [SerializeField] private int _ignitionThreshold = 80;

    [Tooltip("The time spent past the ignition threshold until this object ignites.")]
    [SerializeField] private float _ignitionTime = 1f;

    private float _debugTimer = 0f;
    private bool _debugIgnited = false;


    private float _relaxationTimer = 0f;
    private float _ignitionTimer = 0f; // Current time until ignition. Ignites when reaches _ignitionTime
    private bool _hasIgnited = false; // Whether or not this object has ignited and should currently be burning up.

    // Components
    private List<Room> _currentRooms = new List<Room>();
    public Room CurrentRoom { get { return _currentRooms.Count > 0 ? _currentRooms[0] : null; } }
    private ShaderManager _shaderManager;

    // Unity methods
    private void Awake()
    {
        _shaderManager = GetComponentInChildren<ShaderManager>();
    }

    private void Start()
    {
        _heat = DistrictManager.Instance.Temperature;
        
    }

    private void Update()
    {
        RelaxHeat();
        if (_shaderManager) _shaderManager.UpdateHeatShader(_heat, _warningThreshold, _ignitionThreshold);
        CheckForIgnition();

        if (_hasIgnited)
        {
            _debugTimer += Time.deltaTime;
        }
        if (_debugTimer >= 10f)
        {
            Debug.Log("Something is wrong with this object. DI: " + _debugIgnited);
            _debugTimer = 0f;
        }
    }


    // Room subscribing
    public void EnterRoom(Room room)
    {
        if (!_currentRooms.Contains(room))
        {
            _currentRooms.Add(room);
        }
    }

    // Room unsubscribing
    public void ExitRoom(Room room)
    {
        if (_currentRooms.Contains(room))
        {
            _currentRooms.Remove(room);
        }
    }

    public void Reset()
    {
        _ignitionTimer = 0f;
        _hasIgnited = false;
        coolingOff = false;
        _shaderManager.Reset();
        _heat = DistrictManager.Instance.Temperature;
    }

    // Heat mechanics
    // doRelaxDelay determines whether or not there should be a relax delay should be delayed afterwards
    public void ModifyHeat(float change, bool doRelaxDelay = true)
    {
        if (change > 0) coolingOff = false;
        _heat = Mathf.Clamp(_heat + change, LOWEST_HEAT_VALUE, HIGHEST_HEAT_VALUE);
        if (doRelaxDelay) _relaxationTimer = _timeBeforeRelax;
    }

    private void RelaxHeat()
    {
        if (_relaxationTimer > 0f)
        {
            _relaxationTimer = Mathf.MoveTowards(_relaxationTimer, 0, Time.deltaTime);
            return;
        }

        int roomTemperature = 20; //this should never be applied.
        if (DistrictManager.Instance != null) roomTemperature = DistrictManager.Instance.Temperature;
        if (CurrentRoom != null) roomTemperature = CurrentRoom.Temperature;

        if (_heat != roomTemperature)
        {
            _heat = Mathf.MoveTowards(_heat, roomTemperature, _heatRelaxationRate * Time.deltaTime);
            coolingOff = true;
        }
        else
        {
            coolingOff = false;
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
                heatable.PrepareIgnite(this);
                _shaderManager.StartDissolve(() => {
                    heatable.OnIgnite(this);
                    _debugIgnited = true;
                });
            }
        }
    }

}

