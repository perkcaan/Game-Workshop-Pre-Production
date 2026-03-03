using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{
    private Room _room;

    private void OnEnable()
    {
        _room = (Room)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox(
                "Room drawer editing is disabled in Play Mode.",
                MessageType.Warning
            );
            return;
        }

        bool hasValidPrefab = IsValidPrefabAsset(_room.RoomDrawerPrefab);
        bool hasValidInstance = HasValidActiveInstance(_room);

        if (_room.RoomDrawerPrefab != null && !hasValidPrefab)
        {
            EditorGUILayout.HelpBox(
                "RoomDrawerPrefab must be a prefab asset, not a scene object.",
                MessageType.Error
            );
        }

        GUILayout.Space(5);

        GUI.enabled = hasValidPrefab && !hasValidInstance;
        if (GUILayout.Button("Pull Out Drawer"))
        {
            PullOutDrawer(_room);
        }

        GUI.enabled = hasValidInstance;
        if (GUILayout.Button("Push In Drawer"))
        {
            PushInDrawer(_room);
        }

        GUI.enabled = true;

        DrawStatusInfo(hasValidPrefab, hasValidInstance);
    }

    private void DrawStatusInfo(bool hasValidPrefab, bool hasValidInstance)
    {
        if (!hasValidPrefab)
            return;

        if (hasValidInstance)
        {
            EditorGUILayout.HelpBox(
                "Room drawer is currently pulled out and being edited.",
                MessageType.Info
            );
        }
        else
        {
            EditorGUILayout.HelpBox(
                "Room drawer is currently pushed in.",
                MessageType.None
            );
        }
    }

    private static bool IsValidPrefabAsset(GameObject obj)
    {
        if (obj == null) return false;

        PrefabAssetType type = PrefabUtility.GetPrefabAssetType(obj);
        return type != PrefabAssetType.NotAPrefab;
    }

    private static bool HasValidActiveInstance(Room room)
    {
        if (room.ActiveRoomDrawer == null) return false;

        if (!room.ActiveRoomDrawer) return false;

        return PrefabUtility.IsPartOfPrefabInstance(room.ActiveRoomDrawer);
    }

    private static void PullOutDrawer(Room room)
    {
        if (room == null) return;

        if (room.ActiveRoomDrawer != null) return;

        if (room.RoomDrawerPrefab == null)
        {
            Debug.LogWarning(
                "Cannot pull out drawer: RoomDrawerPrefab is not assigned.",
                room
            );
            return;
        }

        if (!IsValidPrefabAsset(room.RoomDrawerPrefab))
        {
            Debug.LogWarning(
                "Cannot pull out drawer: RoomDrawerPrefab is not a prefab asset.",
                room
            );
            return;
        }

        if (room.transform == null)
        {
            Debug.LogWarning(
                "Cannot pull out drawer: Room has no valid transform.",
                room
            );
            return;
        }

        GameObject instance = PrefabUtility.InstantiatePrefab(room.RoomDrawerPrefab, room.transform) as GameObject;

        if (instance == null)
        {
            Debug.LogWarning("Failed to instantiate RoomDrawerPrefab. The prefab may be missing or invalid.",
                room
            );
            return;
        }

        instance.name = "RoomDrawer (Editing)";
        room.ActiveRoomDrawer = instance;

        Undo.RegisterCreatedObjectUndo(instance, "Pull Out Room Drawer");
        EditorUtility.SetDirty(room);
    }

    private static void PushInDrawer(Room room)
    {
        if (!HasValidActiveInstance(room))
        {
            room.ActiveRoomDrawer = null;
            EditorUtility.SetDirty(room);
            return;
        }

        GameObject instance = room.ActiveRoomDrawer;

        PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.UserAction);

        Undo.DestroyObjectImmediate(instance);

        room.ActiveRoomDrawer = null;
        EditorUtility.SetDirty(room);
    }
}
