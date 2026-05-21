using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tools/SceneFillDefaults")]
public class SceneFillDefaults : ScriptableObject
{
    public List<GameObject> packedPrefabs;
    public List<GameObject> unpackedPrefabs;
}
