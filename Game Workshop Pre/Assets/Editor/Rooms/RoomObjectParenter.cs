using System.Collections.Generic;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEditor;
using UnityEngine;


[InitializeOnLoad]
public static class RoomObjectParenter
{
    private static readonly List<RoomObject> _dragCandidates = new();
    private static bool _isDragging;

    static RoomObjectParenter()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        ObjectChangeEvents.changesPublished += OnChangesPublished;
    }

    private static void OnSceneGUI(SceneView _)
    {
        if (Application.isPlaying) return;
 
        Event e = Event.current;
 
        // Left mouse button only.
        if (e.button != 0) return;
 
        switch (e.type)
        {
            case EventType.MouseDown:
                BeginDrag();
                break;
 
            case EventType.MouseUp:
                if (_isDragging) EndDrag();
                break;
        }
    }

    private static void BeginDrag()
    {
        _dragCandidates.Clear();
        _isDragging = true;
 
        foreach (GameObject selected in Selection.gameObjects)
        {
            // Skip prefab assets
            if (PrefabUtility.IsPartOfPrefabAsset(selected)) continue;

            RoomObject root = selected.GetComponentInParent<RoomObject>();
            if (root != null && !_dragCandidates.Contains(root)) {
                _dragCandidates.Add(root);
            }
        }
    }

    private static void EndDrag()
    {
        _isDragging = false;
 
        if (_dragCandidates.Count == 0) return;
 
        bool anyChanged = false;
 
        foreach (RoomObject roomObject in _dragCandidates)
        {
            // Object may have been destroyed mid-drag.
            if (roomObject == null) continue;

            roomObject.ValidateSprite();
            Vector2 worldPos = roomObject.transform.position;
            Room targetRoom  = FindRoomAt(worldPos);
 
            // Outside every room — leave as-is.
            if (targetRoom == null) continue;
 
            Room currentRoom = FindCurrentRoom(roomObject.gameObject); 
            if (targetRoom == currentRoom) continue;
 
            ParentToRoom(roomObject.gameObject, targetRoom, currentRoom);
            anyChanged = true;
        }
 
        if (anyChanged) _dragCandidates.Clear();
    }

    private static void OnChangesPublished(ref ObjectChangeEventStream stream)
    {
        if (Application.isPlaying) return;
 
        for (int i = 0; i < stream.length; i++)
        {
            if (stream.GetEventType(i) != ObjectChangeKind.CreateGameObjectHierarchy) continue;
 
            stream.GetCreateGameObjectHierarchyEvent(i, out var data);
            GameObject obj = EditorUtility.EntityIdToObject(data.instanceId) as GameObject;
            if (obj == null) continue;
 
            // Only care about created prefabs
            if (PrefabUtility.IsPartOfPrefabAsset(obj)) continue;
            if (!PrefabUtility.IsPartOfPrefabInstance(obj)) continue;

            RoomObject ro = obj.GetComponent<RoomObject>();
            if (ro == null) continue;
            GameObject roomObjectRoot = ro.gameObject;
            if (roomObjectRoot == null) continue;
 
            Vector2 worldPos = roomObjectRoot.transform.position;
            Room targetRoom = FindRoomAt(worldPos);
            if (targetRoom == null) continue;
 
            Room currentRoom = FindCurrentRoom(roomObjectRoot);
            if (targetRoom == currentRoom) continue;
 

            if (roomObjectRoot != null) ParentToRoom(roomObjectRoot, targetRoom, currentRoom);
        }
    }


    private static readonly int RoomLayerMask = LayerMask.GetMask("Room");
    private static Room FindRoomAt(Vector2 worldPos)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldPos, RoomLayerMask);
        return hit != null ? hit.GetComponent<Room>() : null;
    }

    private static Room FindCurrentRoom(GameObject roomObject)
    {
        Room currentRoom = roomObject.GetComponentInParent<Room>();
        return currentRoom;
    }
 

    private static void ParentToRoom(GameObject roomObject, Room targetRoom, Room currentRoom)
    {
        EnsureDrawerPulledOut(targetRoom);
 
        // Determine the destination parent — drawer if available, Room itself as fallback.
        Transform destinationParent = targetRoom.ActiveRoomDrawer != null ? targetRoom.ActiveRoomDrawer.transform : targetRoom.transform;
 
        if (currentRoom != null && PrefabUtility.IsPartOfPrefabInstance(roomObject))
        {
            GameObject oldDrawer        = currentRoom.ActiveRoomDrawer;
            GameObject oldPrefabAsset   = PrefabUtility.GetCorrespondingObjectFromSource(oldDrawer);
            string     oldPrefabPath    = AssetDatabase.GetAssetPath(oldPrefabAsset);
 
            // Unpack
            PrefabUtility.UnpackPrefabInstance(
                oldDrawer,
                PrefabUnpackMode.Completely,
                InteractionMode.UserAction
            );
 
            // Depart RoomObject
            roomObject.transform.SetParent(null, worldPositionStays: true);
 
            // Save unpacked drawer as prefab
            PrefabUtility.SaveAsPrefabAsset(oldDrawer, oldPrefabPath);
 
            // Step 4: Destroy the old drawer gameobject and replace with the new instance
            Object.DestroyImmediate(oldDrawer);
            GameObject newInstance = PrefabUtility.InstantiatePrefab(
                oldPrefabAsset, currentRoom.transform
            ) as GameObject;
            newInstance.name = "RoomDrawer (Editing)";
            currentRoom.ActiveRoomDrawer = newInstance;
            Undo.RegisterCreatedObjectUndo(newInstance, "Repack Old Room Drawer");
            EditorUtility.SetDirty(currentRoom);
        }
 
        Undo.SetTransformParent(
            roomObject.transform,
            destinationParent,
            $"Auto-parent '{roomObject.name}' → Room '{targetRoom.name}'"
        );
 
        EditorUtility.SetDirty(roomObject);
    }


    private static void EnsureDrawerPulledOut(Room room)
    {
        if (room.ActiveRoomDrawer != null) return;
        if (room.RoomDrawerPrefab == null) return;
 
        if (PrefabUtility.GetPrefabAssetType(room.RoomDrawerPrefab) == PrefabAssetType.NotAPrefab)
        {
            Debug.LogWarning(
                $"[RoomObjectAutoParenter] RoomDrawerPrefab on '{room.name}' is not a prefab asset.",
                room
            );
            return;
        }
 
        GameObject instance = PrefabUtility.InstantiatePrefab(
            room.RoomDrawerPrefab, room.transform
        ) as GameObject;
 
        if (instance == null)
        {
            Debug.LogWarning(
                $"[RoomObjectParenter] Failed to instantiate drawer prefab for '{room.name}'.",
                room
            );
            return;
        }
 
        instance.name = "RoomDrawer (Editing)";
        room.ActiveRoomDrawer = instance;
 
        Undo.RegisterCreatedObjectUndo(instance, "Pull Out Room Drawer (Auto)");
        EditorUtility.SetDirty(room);

        Debug.Log($"Pulled out {room.name} drawer automatically to place object.");
    }
 

}

