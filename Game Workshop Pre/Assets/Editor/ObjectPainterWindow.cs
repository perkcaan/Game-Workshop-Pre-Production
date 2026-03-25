using UnityEngine;
using UnityEditor;

public class ObjectPainterWindow : EditorWindow
{
    private GameObject prefabToPaint;
    private Transform parentObject;
    private bool isPainting = false;
    private Grid grid;

    [MenuItem("Tools/Object Painter")]
    public static void ShowWindow()
    {
        GetWindow<ObjectPainterWindow>("Object Painter");
    }

    void OnGUI()
    {
        if (grid == null)
        {
            grid = FindObjectOfType<Grid>();
        }

        GUILayout.Label("Object Painter", EditorStyles.boldLabel);
        prefabToPaint = (GameObject)EditorGUILayout.ObjectField("Prefab to Paint", prefabToPaint, typeof(GameObject), false);
        parentObject = (Transform)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(Transform), true);

        EditorGUILayout.Space();
        if (GUILayout.Button("Start Painting")) isPainting = true;
        if (GUILayout.Button("End Painting")) isPainting = false;

        EditorGUILayout.Space();
        GUILayout.Label("Controls", EditorStyles.boldLabel);
        GUILayout.Label("- Drag Left Click to Paint\n- Hold Left Shift to Erase\n- Hold Left Control for Offset Placement", EditorStyles.label);
    }

    void OnEnable()
    {
        SceneView.duringSceneGui += this.OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (!isPainting || grid == null) return;
        Event e = Event.current;
       
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
        Vector3Int cellPos;
        Vector3 snappedPos;

        if (e.control || e.shift)
        {
            float tileSize = grid.CellToWorld(Vector3Int.one).x;
            mousePos -= Vector2.one * tileSize / 4.0f;
            mousePos *= 2f;
            cellPos = grid.WorldToCell(mousePos);
            snappedPos = grid.GetCellCenterWorld(cellPos);
            snappedPos /= 2f;
            snappedPos += grid.cellSize / 4.0f;
        }
        else
        {
            cellPos = grid.WorldToCell(mousePos);
            snappedPos = grid.GetCellCenterWorld(cellPos);
        }

        Handles.color = e.shift ? Color.red : Color.green;
        Handles.DrawWireCube(snappedPos, grid.cellSize);
        sceneView.Repaint();

        if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
        {
            if (e.button == 0)
            {
                if (e.shift)
                {
                    EraseObject(snappedPos);
                }
                else if (prefabToPaint != null)
                {
                    PaintObject(snappedPos);
                } 
                e.Use();
            }
        }
    }

    void PaintObject(Vector3 position)
    {
        if (GetObjectAtPosition(position) != null)
        {
            return;
        }
        
        GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefabToPaint);
        newObj.transform.position = position;
        
        if (parentObject != null)
        {
            newObj.transform.SetParent(parentObject);
        }

        Undo.RegisterCreatedObjectUndo(newObj, "Paint Object");
    }

    void EraseObject(Vector3 position)
    {
        GameObject objToErase = GetObjectAtPosition(position);

        if (objToErase != null)
        {
            Undo.DestroyObjectImmediate(objToErase);
        }
    }

    GameObject GetObjectAtPosition(Vector3 position)
    {
        Transform targetParent = parentObject != null ? parentObject : null;
        if(targetParent == null) return null; 

        foreach (Transform child in targetParent)
        {
            if (Vector3.Distance(child.position, position) < 0.1f)
            {
                return child.gameObject;
            }
        }
        return null;
    }
}