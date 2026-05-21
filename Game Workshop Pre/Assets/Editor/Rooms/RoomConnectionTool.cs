using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;


[EditorTool("Room Connection Tool")]
public class RoomConnectionTool : EditorTool
{
    private const float CLICK_DISTANCE = 0.2f;

    // Cache system
    private struct RoomPair
    {
        public Room a;
        public Room b;

        public RoomPair(Room x, Room y)
        {
            if (x.GetInstanceID() < y.GetInstanceID())
            {
                a = x;
                b = y;
            }
            else
            {
                a = y;
                b = x;
            }
        }

        public override int GetHashCode()
        {
            return a.GetInstanceID() ^ b.GetInstanceID();
        }

        public override bool Equals(object obj)
        {
            if (obj is not RoomPair other)
                return false;

            return (a == other.a && b == other.b);
        }
    }

    private struct RoomGateConnection
    {
        public Room room;
        public Gate gate;

        public RoomGateConnection(Room room, Gate gate)
        {
            this.room = room;
            this.gate = gate;
        }

        public override int GetHashCode()
        {
            return room.GetInstanceID() ^ gate.GetInstanceID();
        }

        public override bool Equals(object obj)
        {
            if (obj is not RoomGateConnection other)
                return false;

            return room == other.room && gate == other.gate;
        }
    }

    private List<Room> _rooms = new();
    private List<Gate> _gates = new();
    private HashSet<RoomPair> _connections = new();
    private HashSet<RoomGateConnection> _gateConnections = new();
    private bool _dirty = true;

    private void OnEnable()
    {
        EditorApplication.hierarchyChanged += MarkDirty;
    }

    private void OnDisable()
    {
        EditorApplication.hierarchyChanged -= MarkDirty;
    }

    private void MarkDirty()
    {
        _dirty = true;
    }

    private void RebuildCache()
    {
        _rooms.Clear();
        _gates.Clear();

        _rooms.AddRange(FindObjectsByType<Room>(FindObjectsSortMode.None));
        _gates.AddRange(FindObjectsByType<Gate>(FindObjectsSortMode.None));

        _connections.Clear();
        foreach (Room room1 in _rooms)
        {
            foreach (Room room2 in room1.NearbyRooms)
            {
                if (room1 == room2) continue;
                _connections.Add(new RoomPair(room1, room2));
            }
        }

        _gateConnections.Clear();
        foreach (Room room in _rooms)
        {
            foreach (Gate gate in room.ConnectedGates)
            {
                _gateConnections.Add(new RoomGateConnection(room, gate));
            }
        }

        _dirty = false;
    }

    private void SanitizeCache()
    {
        _rooms.RemoveAll(r => r == null);
        _gates.RemoveAll(g => g == null);
        _connections.RemoveWhere(c => c.a == null || c.b == null);
        _gateConnections.RemoveWhere(c => c.room == null || c.gate == null);
    }


    // Currently active node
    private Object _activeNode;

    public override void OnToolGUI(EditorWindow window)
    {
        if (_dirty) RebuildCache();
        SanitizeCache();

        Event e = Event.current;

        DrawConnections();
        DrawRooms();
        DrawGates();

        HandleInput(e);

        if (_activeNode != null)
        {
            DrawDragPreview(e.mousePosition);
        }

        if (e.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(
                GUIUtility.GetControlID(FocusType.Passive)
            );
        }
    }

    private void DrawConnections()
    {
        foreach (RoomPair connection in _connections)
        {
            Vector3 room1Pos = connection.a.GetComponent<PolygonCollider2D>().bounds.center;

            Vector3 room2Pos = connection.b.GetComponent<PolygonCollider2D>().bounds.center;

            DrawConnectionLine(room1Pos, room2Pos, Color.blue);
        }


        foreach (RoomGateConnection connection in _gateConnections)
        {
            Vector3 roomPos = connection.room.GetComponent<PolygonCollider2D>().bounds.center;

            Vector3 gatePos = connection.gate.transform.position;

            DrawConnectionLine(roomPos, gatePos, Color.green);
        }

    }

    private void DrawRooms()
    {
        foreach (Room room in _rooms)
        {
            if (room == null) continue;
            PolygonCollider2D polygon = room.GetComponent<PolygonCollider2D>();
            if (polygon == null) continue;

            Vector3 pos = polygon.bounds.center;
            float size = HandleUtility.GetHandleSize(pos) * 0.2f;
            Handles.color = Color.lightBlue;

            Handles.DrawSolidDisc(
                pos,
                Vector3.forward,
                size
            );

            Handles.color = Color.blue;

            if (Handles.Button(pos, Quaternion.identity, size, size, Handles.CircleHandleCap))
            {
                OnNodeClicked(room);
            }
        }
    }


    private void DrawGates()
    {
        foreach (Gate gate in _gates)
        {
            if (gate == null) continue;

            Vector3 pos = gate.transform.position;
            float size = HandleUtility.GetHandleSize(pos) * 0.16f;
            Handles.color = Color.green;

            Vector3[] verts =
            {
                pos + new Vector3(-size, -size),
                pos + new Vector3(-size,  size),
                pos + new Vector3( size,  size),
                pos + new Vector3( size, -size),
            };

            Handles.DrawSolidRectangleWithOutline(verts, Color.lightGreen, Color.black
            );

            if (Handles.Button(pos, Quaternion.identity, size, size, Handles.RectangleHandleCap))
            {
                OnNodeClicked(gate);
            }
        }
    }


    private void OnNodeClicked(Object node)
    {
        Event e = Event.current;

        // First click = start connection drag
        if (_activeNode == null)
        {
            _activeNode = node;
            return;
        }

        // Clicking same node cancels
        if (_activeNode == node)
        {
            _activeNode = null;
            return;
        }

        CreateConnection(_activeNode, node);

        _activeNode = null;

        e.Use();
    }

    private void HandleInput(Event e)
    {
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            _activeNode = null;
            e.Use();
        }

        if (e.type == EventType.MouseDown && e.button == 0 && _activeNode == null)
        {
            RemoveLineClick(e);
        }
    }

    private void RemoveLineClick(Event e)
    {
        foreach (RoomGateConnection connection in _gateConnections)
        {
            Vector3 roomPos = connection.room.GetComponent<PolygonCollider2D>().bounds.center;
            Vector3 gatePos = connection.gate.transform.position;

            if (RemoveLineCheck(roomPos, gatePos, e)) {
                RemoveGateConnection(connection);
                return;
            }
        }

        foreach (RoomPair connection in _connections)
        {
            Vector3 room1Pos = connection.a.GetComponent<PolygonCollider2D>().bounds.center;

            Vector3 room2Pos = connection.b.GetComponent<PolygonCollider2D>().bounds.center;

            if (RemoveLineCheck(room1Pos, room2Pos, e)) {
                RemoveRoomPair(connection);
                return;
            }
        }
    }

    private bool RemoveLineCheck(Vector3 start, Vector3 end, Event e)
    {
        Vector2 mouse = e.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(mouse);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 worldMouse = ray.GetPoint(enter);

            float dist = HandleUtility.DistancePointBezier(
                worldMouse,
                start,
                end,
                start,
                end
            );

            if (dist < CLICK_DISTANCE)
            {
                e.Use();
                return true;
            }
        }
        return false;
    }

    private void DrawDragPreview(Vector2 mousePosition)
    {
        Vector3 startPos = GetNodePosition(_activeNode);

        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        if (!plane.Raycast(ray, out float enter))
            return;

        Vector3 worldMouse = ray.GetPoint(enter);
        
        DrawConnectionLine(startPos, worldMouse, Color.white);

        SceneView.RepaintAll();
    }

    private void DrawConnectionLine(Vector3 start, Vector3 end, Color color)
    {
        Handles.color = color;

        Vector3 startTangent = start + Vector3.right * 0.5f;
        Vector3 endTangent = end + Vector3.left * 0.5f;

        Handles.DrawBezier(
            start,
            end,
            startTangent,
            endTangent,
            color,
            null,
            8f //thickness
        );
        
    }

    private Vector3 GetNodePosition(Object node)
    {
        if (node is Room room)
        {
            PolygonCollider2D poly = room.GetComponent<PolygonCollider2D>();

            if (poly != null) return poly.bounds.center;
        }

        if (node is Gate gate)
        {
            return gate.transform.position;
        }

        return Vector3.zero;
    }

    // Actual Room connection logic
    private void CreateConnection(Object a, Object b)
    {
        if (a is Gate && b is Gate)
        {
            return; //Cannot connect gates;
        }

        if (a is Room && b is Gate)
        {
            MakeGateConnection(a as Room, b as Gate);
            return;
        }

        if (a is Gate && b is Room)
        {
            MakeGateConnection(b as Room, a as Gate);
            return;
        }

        if (a is Room && b is Room)
        {
            MakeRoomPair(a as Room, b as Room);
            return;
        }
    }

    private void MakeRoomPair(Room a, Room b)
    {
        RoomPair pair = new RoomPair(a,b);
        if (_connections.Contains(pair)) return;

        int group = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Create Room Pair");

        Undo.RecordObject(pair.a, null);
        Undo.RecordObject(pair.b, null);
        Undo.RecordObject(this, null);

        pair.a.NearbyRooms.Add(pair.b);
        pair.b.NearbyRooms.Add(pair.a);
        _connections.Add(pair);

        EditorUtility.SetDirty(pair.a);
        EditorUtility.SetDirty(pair.b);
        EditorUtility.SetDirty(this);

        Undo.CollapseUndoOperations(group);
    }

    private void MakeGateConnection(Room room, Gate gate)
    {
        RoomGateConnection connection = new RoomGateConnection(room, gate);
        if (_gateConnections.Contains(connection)) return;

        int group = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Create Gate Connection");

        Undo.RecordObject(connection.room, null);
        Undo.RecordObject(this, null);
        
        connection.room.ConnectedGates.Add(gate);
        _gateConnections.Add(connection);

        // Automatically pair this to every other room connected to gate
        foreach (RoomGateConnection otherConnection in _gateConnections)
        {
            if (otherConnection.gate == gate && otherConnection.room != room)
            {
                RoomPair newPair = new RoomPair(room, otherConnection.room);
                if (_connections.Contains(newPair)) continue;

                Undo.RecordObject(otherConnection.room, null);
                room.NearbyRooms.Add(otherConnection.room);
                otherConnection.room.NearbyRooms.Add(room);
                EditorUtility.SetDirty(otherConnection.room);
            }
        }

        EditorUtility.SetDirty(connection.room);
        EditorUtility.SetDirty(this);

        Undo.CollapseUndoOperations(group);
    }

    private void RemoveRoomPair(RoomPair pair)
    {
        int group = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Remove Room Pair");

        Undo.RecordObject(pair.a, null);
        Undo.RecordObject(pair.b, null);
        Undo.RecordObject(this, null);

        pair.a.NearbyRooms.Remove(pair.b);
        pair.b.NearbyRooms.Remove(pair.a);
        _connections.Remove(pair);
        EditorUtility.SetDirty(pair.a);
        EditorUtility.SetDirty(pair.b);
        EditorUtility.SetDirty(this);

        Undo.CollapseUndoOperations(group);
    }

    private void RemoveGateConnection(RoomGateConnection connection)
    {
        int group = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Remove Gate Connection");

        Undo.RecordObject(connection.room, null);
        Undo.RecordObject(this, null);

        connection.room.ConnectedGates.Remove(connection.gate);
        _gateConnections.Remove(connection);
        EditorUtility.SetDirty(connection.room);
        EditorUtility.SetDirty(this);

        Undo.CollapseUndoOperations(group);
    }

}
