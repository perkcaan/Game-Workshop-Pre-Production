using UnityEditor;
using UnityEngine;
 
[CustomEditor(typeof(DistrictManager))]
public class DistrictManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Generate Rooms"))
        {
            DistrictManager manager = (DistrictManager) target;
            manager.GenerateRooms();
            EditorUtility.SetDirty(manager);
        }
 
        GUILayout.Space(5);
 
        DrawDefaultInspector();
    }
}