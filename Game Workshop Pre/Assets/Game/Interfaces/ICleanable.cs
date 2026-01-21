using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICleanable
{
    public void SetRoom(Room room);

    public int Size { get; set; }
}
