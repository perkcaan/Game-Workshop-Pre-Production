using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class SingleEventTrigger
{
    [SerializeField] private GameObject _targetObject;
    [SerializeField] private Component _targetComponent;
    [SerializeField] private string _targetAction;
    
    [SerializeReference] private List<EventParameter> _parameters = new List<EventParameter>();


    private static readonly Dictionary<Type, Type> ParameterTypeRegistry = new Dictionary<Type, Type>
    {
        { typeof(int),        typeof(EventParameterInt) },
        { typeof(float),      typeof(EventParameterFloat) },
        { typeof(bool),       typeof(EventParameterBool) },
        { typeof(string),     typeof(EventParameterString) },
        { typeof(Vector2),    typeof(EventParameterVector2) },
        { typeof(Vector3),    typeof(EventParameterVector3) },
        { typeof(Vector4),    typeof(EventParameterVector4) },
        { typeof(Color),      typeof(EventParameterColor) },
        { typeof(Quaternion), typeof(EventParameterQuaternion) },
        { typeof(GameObject), typeof(EventParameterGameObject) },
        { typeof(Transform),  typeof(EventParameterTransform) },
    };

    public void Trigger()
    {
        TriggerInternal(null);
    }

    public void Trigger(params object[] overrideParameters)
    {
        TriggerInternal(overrideParameters);
    }

    private void TriggerInternal(object[] overrides)
    {

        if (_targetComponent == null)
        {
            Debug.LogWarning("EventTrigger: Target component is missing.");
            return;
        }

        if (string.IsNullOrEmpty(_targetAction))
        {
            Debug.LogWarning($"EventTrigger: No target action selected on component {_targetComponent}.");
            return;
        }

        MethodInfo method = _targetComponent.GetType().GetMethod(
            _targetAction,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (method == null)
        {
            Debug.LogWarning($"EventTrigger: Method '{_targetAction}' not found on {_targetComponent.GetType().Name}.");
            return;
        }

        object[] finalParameters;

        if (overrides != null)
        {
            finalParameters = overrides;
        }
        else
        {
            finalParameters = _parameters
                .Select(p => p.GetValue())
                .ToArray();
        }

        try
        {
            method.Invoke(_targetComponent, finalParameters);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(
                $"EventTrigger: Exception invoking '{_targetAction}' on {_targetComponent.name}. " +
                $"{e.GetType().Name}: {e.Message}");
        }
    }

    public void SetMethod(Component component, MethodInfo method)
    {
        _targetComponent = component;
        _targetAction = method?.Name;

        RebuildParameters(method);
    }

    private void RebuildParameters(MethodInfo method)
    {
        _parameters.Clear();

        if (method == null) return;

        foreach (ParameterInfo info in method.GetParameters())
        {
            if (!ParameterTypeRegistry.TryGetValue(info.ParameterType, out Type concreteType))
            {
                Debug.LogError($"EventTrigger: No EventParameter type registered for {info.ParameterType.Name}. Add support for type of parameter '{info.Name}'.");
                continue;
            }

            EventParameter parameter = (EventParameter)Activator.CreateInstance(concreteType);
            parameter.ParameterName = info.Name;
            _parameters.Add(parameter);
        }
    }
}

[Serializable]
public class EventTrigger : IEnumerable<SingleEventTrigger>
{
    [SerializeField] private List<SingleEventTrigger> _eventTriggers = new List<SingleEventTrigger>();

    public int Count => _eventTriggers.Count;
    public SingleEventTrigger this[int index] => _eventTriggers[index];

    public void Trigger()
    {
        foreach (SingleEventTrigger eventTrigger in _eventTriggers)
            eventTrigger.Trigger();
    }

    public void Trigger(int index)
    {
        _eventTriggers[index].Trigger();
    }

    public void Trigger(int index, params object[] overrideParameters)
    {
        _eventTriggers[index].Trigger(overrideParameters);
    }

    public IEnumerator<SingleEventTrigger> GetEnumerator() => _eventTriggers.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}