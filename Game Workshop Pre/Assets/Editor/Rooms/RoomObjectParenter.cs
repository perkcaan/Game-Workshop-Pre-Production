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
 
        if (targetRoom.ActiveRoomDrawer == null)
        {
            Debug.LogWarning(
                $"[RoomObjectAutoParenter] Could not pull out drawer for '{targetRoom.name}'. " +
                "Is RoomDrawerPrefab assigned?",
                targetRoom
            );
            return;
        }
 
        // If the object is inside a pulled-out drawer (a live prefab instance),
        // we must surgically remove it from that prefab. The correct order:
        //   1. Unpack the drawer instance so everything becomes plain scene objects
        //      and the RoomObject is free to move.
        //   2. Depart the RoomObject to scene root.
        //   3. SaveAsPrefabAsset the unpacked drawer back to the original asset path
        //      — now saved without the RoomObject.
        //   4. Destroy the unpacked drawer instance and re-instantiate it as a
        //      fresh prefab instance so RoomEditor sees it as still pulled out.
        if (currentRoom != null && PrefabUtility.IsPartOfPrefabInstance(roomObject))
        {
            GameObject oldDrawer        = currentRoom.ActiveRoomDrawer;
            GameObject oldPrefabAsset   = PrefabUtility.GetCorrespondingObjectFromSource(oldDrawer);
            string     oldPrefabPath    = AssetDatabase.GetAssetPath(oldPrefabAsset);
 
            // Step 1: Unpack — drawer and all children become plain scene objects.
            PrefabUtility.UnpackPrefabInstance(
                oldDrawer,
                PrefabUnpackMode.Completely,
                InteractionMode.UserAction
            );
 
            // Step 2: Depart the RoomObject before saving so it isn't included.
            roomObject.transform.SetParent(null, worldPositionStays: true);
 
            // Step 3: Save the now-unpacked drawer (without the RoomObject) back
            // to the original prefab asset on disk.
            PrefabUtility.SaveAsPrefabAsset(oldDrawer, oldPrefabPath);
 
            // Step 4: Destroy the plain-GameObject drawer and replace it with a
            // fresh prefab instance so RoomEditor's HasValidActiveInstance stays true.
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
            targetRoom.ActiveRoomDrawer.transform,
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
                $"[RoomObjectAutoParenter] Failed to instantiate drawer prefab for '{room.name}'.",
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

