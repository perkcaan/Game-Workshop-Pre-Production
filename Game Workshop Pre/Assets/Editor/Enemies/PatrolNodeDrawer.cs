using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PatrolNode))]
public class PatrolNodeDrawer : PropertyDrawer
{
    private const float Spacing = 4f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty patrolPoints = property.FindPropertyRelative("_patrolPoints");
        SerializedProperty reverse = property.FindPropertyRelative("_reversePatrolInsteadOfWrap");
        SerializedProperty arrival = property.FindPropertyRelative("_arrivalProximity");

        float height = 0f;
        height += EditorGUIUtility.singleLineHeight; // header
        height += Spacing;
        height += EditorGUI.GetPropertyHeight(patrolPoints, true);
        height += Spacing;
        height += EditorGUI.GetPropertyHeight(reverse, true);
        height += Spacing;
        height += EditorGUI.GetPropertyHeight(arrival, true);
        height += Spacing;
        height += EditorGUIUtility.singleLineHeight; // edit button
        height += Spacing;
        height += EditorGUIUtility.singleLineHeight; // clear button

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty patrolPoints = property.FindPropertyRelative("_patrolPoints");
        SerializedProperty reverse = property.FindPropertyRelative("_reversePatrolInsteadOfWrap");
        SerializedProperty arrival = property.FindPropertyRelative("_arrivalProximity");

        Rect rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.LabelField(rect, label, EditorStyles.boldLabel);
        rect.y += rect.height + Spacing;

        float pointsHeight = EditorGUI.GetPropertyHeight(patrolPoints, true);
        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, pointsHeight), patrolPoints, true);
        rect.y += pointsHeight + Spacing;

        float reverseHeight = EditorGUI.GetPropertyHeight(reverse, true);
        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, reverseHeight), reverse, true);
        rect.y += reverseHeight + Spacing;

        float arrivalHeight = EditorGUI.GetPropertyHeight(arrival, true);
        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, arrivalHeight), arrival, true);
        rect.y += arrivalHeight + Spacing;

        bool isEditing = PatrolNodeSceneEditor.IsEditing(patrolPoints);

        Color old = GUI.backgroundColor;
        if (isEditing)
            GUI.backgroundColor = Color.green;

        if (GUI.Button(
            new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
            isEditing ? "Stop Editing Patrol Points" : "Edit Patrol Points In Scene"))
        {
            if (isEditing)
                PatrolNodeSceneEditor.StopEditing();
            else
                PatrolNodeSceneEditor.StartEditing(patrolPoints);
        }

        GUI.backgroundColor = old;
        rect.y += EditorGUIUtility.singleLineHeight + Spacing;

        if (GUI.Button(
            new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
            "Clear Patrol Points"))
        {
            patrolPoints.serializedObject.Update();
            patrolPoints.ClearArray();
            patrolPoints.serializedObject.ApplyModifiedProperties();
            SceneView.RepaintAll();
        }

        EditorGUI.EndProperty();
    }
}