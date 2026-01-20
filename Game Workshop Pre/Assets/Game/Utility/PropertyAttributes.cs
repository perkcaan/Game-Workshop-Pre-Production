using System;
using Unity.Properties;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class SerializeReferenceDropdownAttribute : PropertyAttribute { }


public class ReadOnlyAttribute : PropertyAttribute { }



[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class BehaviourNodeAttribute : Attribute
{
    public int Priority { get; private set; } = 0;
    public string Folder { get; private set; } = "";

    public BehaviourNodeAttribute(int priority = 0, string folder = "")
    {
        Priority = priority;
        Folder = folder;
    }
}