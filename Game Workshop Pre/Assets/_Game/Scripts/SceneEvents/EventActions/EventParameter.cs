using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class EventParameter
{
    public string ParameterName;
    public abstract object GetValue();
    public abstract void SetValue(object value);
    public abstract Type GetValueType();
}

[Serializable] public class EventParameterInt        : EventParameter { [SerializeField] private int        _value; public override object GetValue() => _value; public override void SetValue(object v) { if (v is int        x) _value = x; } public override Type GetValueType() => typeof(int); }
[Serializable] public class EventParameterFloat      : EventParameter { [SerializeField] private float      _value; public override object GetValue() => _value; public override void SetValue(object v) { if (v is float      x) _value = x; } public override Type GetValueType() => typeof(float); }
[Serializable] public class EventParameterBool       : EventParameter { [SerializeField] private bool       _value; public override object GetValue() => _value; public override void SetValue(object v) { if (v is bool       x) _value = x; } public override Type GetValueType() => typeof(bool); }
[Serializable] public class EventParameterString     : EventParameter { [SerializeField] private string     _value; public override object GetValue() => _value; public override void SetValue(object v) { if (v is string     x) _value = x; } public override Type GetValueType() => typeof(string); }
[Serializable] public class EventParameterVector2    : EventParameter { [SerializeField] private Vector2    _value; public override object GetValue() => _value; public override void SetValue(object v) { if (v is Vector2    x) _value = x; } public override Type GetValueType() => typeof(Vector2); }
[Serializable] public class EventParameterVector3    : EventParameter { [SerializeField] private Vector3    _value; public override object GetValue() => _value; public override void SetValue(object v) { if (v is Vector3    x) _value = x; } public override Type GetValueType() => typeof(Vector3); }
[Serializable] public class EventParameterVector4    : EventParameter { [SerializeField] private Vector4    _value; public override object GetValue() => _value; public override void SetValue(object v) { if (v is Vector4    x) _value = x; } public override Type GetValueType() => typeof(Vector4); }
[Serializable] public class EventParameterColor      : EventParameter { [SerializeField] private Color      _value; public override object GetValue() => _value; public override void SetValue(object v) { if (v is Color      x) _value = x; } public override Type GetValueType() => typeof(Color); }
[Serializable] public class EventParameterQuaternion : EventParameter { [SerializeField] private Quaternion _value; public override object GetValue() => _value; public override void SetValue(object v) { if (v is Quaternion x) _value = x; } public override Type GetValueType() => typeof(Quaternion); }
[Serializable] public class EventParameterGameObject : EventParameter { [SerializeField] private GameObject _value; public override object GetValue() => _value; public override void SetValue(object v) { if (v is GameObject x) _value = x; } public override Type GetValueType() => typeof(GameObject); }
[Serializable] public class EventParameterTransform  : EventParameter { [SerializeField] private Transform  _value; public override object GetValue() => _value; public override void SetValue(object v) { if (v is Transform  x) _value = x; } public override Type GetValueType() => typeof(Transform); }