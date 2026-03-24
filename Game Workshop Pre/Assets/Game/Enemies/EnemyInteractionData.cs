using System;
using System.Data.Common;
using UnityEngine;

public struct EnemySweepData
{
    public bool CanSweep { get; set; }
}

public struct EnemySwipeData
{
    public bool IsVulnerable { get; set; }
    public float SwipeMultiplier { get; set; }
}

public struct EnemyPokeData
{
    public bool IsVulnerable { get; set; }
    public float PokeMultiplier { get; set; }
}

public struct EnemyAbsorbData
{
    public bool CanAbsorb { get; set; }
}

[Serializable]
public class EnemyInteractionProperties
{
    [Header("Sweep")]
    [SerializeField] private bool _canSweepBase = false;
    public bool CanSweepBase { get { return _canSweepBase; }}

    [Header("Swipe")]
    [SerializeField] private bool _isBaseVulnerableToSwipe = false;
    public bool IsBaseVulnerableToSwipe { get { return _isBaseVulnerableToSwipe; }}
    [SerializeField] private float _swipeBaseMultiplier = 1f;
    public float SwipeBaseMultiplier { get { return _swipeBaseMultiplier; }}
    [SerializeField] private float _swipeVulnerableMultiplier = 0.3f;
    public float SwipeVulnerableMultiplier { get { return _swipeVulnerableMultiplier; }}

    [Header("Poke")]
    [SerializeField] private bool _isBaseVulnerableToPoke = false;
    public bool IsBaseVulnerableToPoke { get { return _isBaseVulnerableToPoke; }}
    [SerializeField] private float _pokeBaseMultiplier = 1f;
    public float PokeBaseMultiplier { get { return _pokeBaseMultiplier; }}
    [SerializeField] private float _pokeVulnerableMultiplier = 2f;
    public float PokeVulnerableMultiplier { get { return _pokeVulnerableMultiplier; }}


    [Header("Absorb")]
    [SerializeField] private bool _canAbsorbBase = false;
    public bool CanAbsorbBase { get { return _canAbsorbBase; }}
}