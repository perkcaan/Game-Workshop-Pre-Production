using System;
using Unity.Properties;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class SerializeReferenceDropdownAttribute : PropertyAttribute { }


public class ReadOnlyAttribute : PropertyAttribute { }
