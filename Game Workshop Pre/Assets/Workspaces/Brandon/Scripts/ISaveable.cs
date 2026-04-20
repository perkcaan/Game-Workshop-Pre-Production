using UnityEngine;
using System.Collections.Generic;

public interface ISaveable
{

    public SaveData DataType { get; set; }


    public void AddSaveableData();
}
