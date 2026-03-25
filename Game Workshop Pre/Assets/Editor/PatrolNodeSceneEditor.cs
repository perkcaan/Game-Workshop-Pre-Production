using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class PatrolNodeSceneEditor
{
    private static SerializedObject _activeSerializedObject;
    private static Object _activeTargetObject;
    private static string _activePropertyPath;

    private const float SquareSizeMultiplier = 0.08f;
    private static readonly Color FillColor = new Color(0f, 1f, 0f, 0.18f);
    private static readonly Color LineColor = Color.green;

    static PatrolNodeSceneEditor()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    public static void StartEditing(SerializedProperty patrolPointsProperty)
    {
        if (patrolPointsProperty == null)
            return;

        _activeSerializedObject = patrolPointsProperty.serializedObject;
        _activeTargetObject = patrolPointsProperty.serializedObject.targetObject;
        _activePropertyPath = patrolPointsProperty.propertyPath;

        Debug.Log("Patrol edit start: " + _activePropertyPath);
        SceneView.RepaintAll();
    }

    public static void StopEditing()
    {
        _activeSerializedObject = null;
        _activeTargetObject = null;
        _activePropertyPath = null;
        SceneView.RepaintAll();
    }

    public static bool IsEditing(SerializedProperty patrolPointsProperty)
    {
        if (patrolPointsProperty == null)
            return false;

        return patrolPointsProperty.serializedObject.targetObject == _activeTargetObject &&
               patrolPointsProperty.propertyPath == _activePropertyPath;
    }

    private static SerializedProperty GetActivePatrolPointsProperty()
    {
        if (_activeSerializedObject == null || _activeTargetObject == null || string.IsNullOrEmpty(_activePropertyPath))
            return null;

        if (_activeSerializedObject.targetObject == null)
        {
            StopEditing();
            return null;
        }

        _activeSerializedObject.Update();

        SerializedProperty patrolPoints = _activeSerializedObject.FindProperty(_activePropertyPath);

        if (patrolPoints == null || !patrolPoints.isArray)
        {
            StopEditing();
            return null;
        }

        return patrolPoints;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        SerializedProperty patrolPoints = GetActivePatrolPointsProperty();
        if (patrolPoints == null)
            return;

        DrawPatrolVisualization(patrolPoints);
        HandleSceneInput(patrolPoints);
    }

    private static void HandleSceneInput(SerializedProperty patrolPoints)
    {
        Event e = Event.current;
        if (e == null)
            return;

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
        {
            AddPointAtMousePosition(patrolPoints, e.mousePosition);
            e.Use();
        }

        if (e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.Escape)
            {
                StopEditing();
                e.Use();
            }
            else if (e.keyCode == KeyCode.Backspace || e.keyCode == KeyCode.Delete)
            {
                RemoveLastPoint(patrolPoints);
                e.Use();
            }
        }
    }

    private static void AddPointAtMousePosition(SerializedProperty patrolPoints, Vector2 mousePosition)
    {
        Vector2 worldPoint = GetMouseWorldPointXY(mousePosition);

        Undo.RecordObjects(patrolPoints.serializedObject.targetObjects, "Add Patrol Point");

        patrolPoints.serializedObject.Update();
        patrolPoints.arraySize++;
        patrolPoints.GetArrayElementAtIndex(patrolPoints.arraySize - 1).vector2Value = worldPoint;
        patrolPoints.serializedObject.ApplyModifiedProperties();

        SceneView.RepaintAll();
    }

    private static void RemoveLastPoint(SerializedProperty patrolPoints)
    {
        if (patrolPoints.arraySize <= 0)
            return;

        Undo.RecordObjects(patrolPoints.serializedObject.targetObjects, "Remove Patrol Point");

        patrolPoints.serializedObject.Update();
        patrolPoints.DeleteArrayElementAtIndex(patrolPoints.arraySize - 1);
        patrolPoints.serializedObject.ApplyModifiedProperties();

        SceneView.RepaintAll();
    }

    private static Vector2 GetMouseWorldPointXY(Vector2 mousePosition)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hit = ray.GetPoint(enter);
            return new Vector2(hit.x, hit.y);
        }

        return Vector2.zero;
    }

    private static void DrawPatrolVisualization(SerializedProperty patrolPoints)
    {
        Vector3? previous = null;

        for (int i = 0; i < patrolPoints.arraySize; i++)
        {
            SerializedProperty element = patrolPoints.GetArrayElementAtIndex(i);
            Vector2 p2 = element.vector2Value;
            Vector3 p3 = new Vector3(p2.x, p2.y, 0f);

            if (previous.HasValue)
            {
                Handles.color = LineColor;
                Handles.DrawLine(previous.Value, p3);
            }

            DrawPointHandle(patrolPoints, i, p3);
            previous = p3;
        }
    }

    private static void DrawPointHandle(SerializedProperty patrolPoints, int index, Vector3 point)
    {
        float size = HandleUtility.GetHandleSize(point) * SquareSizeMultiplier;

        Vector3[] verts =
        {
            point + new Vector3(-size, -size, 0f),
            point + new Vector3(-size,  size, 0f),
            point + new Vector3( size,  size, 0f),
            point + new Vector3( size, -size, 0f)
        };

        Handles.DrawSolidRectangleWithOutline(verts, FillColor, LineColor);
        Handles.Label(point + new Vector3(size, size, 0f), index.ToString());

        EditorGUI.BeginChangeCheck();

        Vector3 movedPoint = Handles.FreeMoveHandle(
            point,
            size * 0.9f,
            Vector3.zero,
            Handles.RectangleHandleCap
        );

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(patrolPoints.serializedObject.targetObjects, "Move Patrol Point");

            patrolPoints.serializedObject.Update();
            patrolPoints.GetArrayElementAtIndex(index).vector2Value = new Vector2(movedPoint.x, movedPoint.y);
            patrolPoints.serializedObject.ApplyModifiedProperties();

            SceneView.RepaintAll();
        }
    }
}