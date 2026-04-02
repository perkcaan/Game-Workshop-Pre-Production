using UnityEngine;
using System.Collections.Generic;

public interface ISaveable
{
    public static Dictionary<string, List<object>> saveableData;

    public void AddSaveableData() { }
}
