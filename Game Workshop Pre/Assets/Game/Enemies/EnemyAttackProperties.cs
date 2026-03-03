using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SimpleAttackProperties
{
    [SerializeField] private float _startup = 0.3f;
    public float Startup { get { return _startup; } }
    [SerializeField] private float _duration = 0.5f;
    public float Duration { get { return _duration; } }
    [SerializeField] private float _endlag = 1f;
    public float Endlag { get { return _endlag; } }
}
