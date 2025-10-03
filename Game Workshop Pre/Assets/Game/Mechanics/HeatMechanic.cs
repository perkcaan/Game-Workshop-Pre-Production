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

    [Tooltip("The heat level at which this object ignites into flame.")]
    [SerializeField] private float _ignitionThreshold = 80;

    

    private Room _currentRoom;

    private void Start()
    {
        SetHeatToRoom();
    }

    private void Update()
    {
        RelaxHeat();
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


    public void ModifyHeat(int change)
    {
        _heat = Mathf.Clamp(_heat + change, LOWEST_HEAT_VALUE, HIGHEST_HEAT_VALUE);
    }



}
