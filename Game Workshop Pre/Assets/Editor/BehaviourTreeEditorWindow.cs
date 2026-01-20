using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BehaviourTreeEditorWindow : EditorWindow
{
    private UnityEngine.Object targetObject;
    private string propertyPath;
    private SerializedObject serializedObject;
    private SerializedProperty rootNodeProperty;

    private readonly Dictionary<string, bool> foldoutStates = new();
    private const float IndentWidth = 16f;
    private static readonly Color TreeLineColor = new Color(0.6f, 0.6f, 0.6f, 1f);

    public static void Open(SerializedProperty property)
    {
        BehaviourTreeEditorWindow window = GetWindow<BehaviourTreeEditorWindow>("Behaviour Tree");
        window.targetObject = property.serializedObject.targetObject;
        window.propertyPath = property.propertyPath;
        window.RefreshProperty();
        window.Show();
    }

    private bool RefreshProperty()
    {
        if (targetObject == null) return false;
        serializedObject = new SerializedObject(targetObject);
        rootNodeProperty = serializedObject.FindProperty(propertyPath);
        return rootNodeProperty != null;
    }

    private void OnGUI()
    {
        if (!RefreshProperty())
        {
            EditorGUILayout.HelpBox("No Behaviour Tree selected.", MessageType.Info);
            return;
        }

        serializedObject.Update();

        EditorGUILayout.LabelField("Behaviour Tree Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        DrawNodeSlot(rootNodeProperty, new List<bool>(), true);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawNodeSlot(SerializedProperty nodeProperty, List<bool> ancestorIsLast, bool isLast)
    {
        BehaviourTreeNode node = nodeProperty.managedReferenceValue as BehaviourTreeNode;
        Type currentType = node?.GetType();
        bool hasChildren = node != null && node.MaxChildren != 0;
        
        Rect row = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

        // Draw tree lines
        DrawTreeLines(row, ancestorIsLast, isLast);

        float visualIndent = ancestorIsLast.Count * IndentWidth;
        Rect textRect = new Rect(row.x + visualIndent, row.y, EditorGUIUtility.labelWidth + 50f, row.height);
        Rect foldoutRect = new Rect(row.xMax - 16f, row.y, 16f, row.height);
        Rect dropdownRect = new Rect(textRect.xMax, row.y, row.width - visualIndent - textRect.width - (hasChildren ? 18f : 0f), row.height);

        // ==DisplayName field
        if (node != null)
        {
            SerializedProperty displayProp = nodeProperty.FindPropertyRelative("DisplayName");
            if (displayProp != null)
            {
                EditorGUI.BeginChangeCheck();
                string newName = EditorGUI.TextField(textRect, displayProp.stringValue);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(targetObject, "Change Node DisplayName");
                    displayProp.stringValue = newName;
                    serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                EditorGUI.LabelField(textRect, node.DisplayName ?? currentType?.Name ?? "Empty Node");
            }
        }
        else
        {
            EditorGUI.LabelField(textRect, "Empty Node");
        }


        // ==Node type Dropdown
        string dropdownLabel;
        if (node == null)
        {
            dropdownLabel = "Empty Node";
        }
        else
        {
            // Display the actual type name on the dropdown
            dropdownLabel = currentType?.Name ?? "Empty Node";
        }

        if (EditorGUI.DropdownButton(dropdownRect, new GUIContent(dropdownLabel), FocusType.Keyboard))
        {
            List<Type> nodeTypes = BehaviourTreeNodeRegistry.GetAllNodeTypes();
            string[] nodeNames = BehaviourTreeNodeRegistry.GetDisplayNames();
            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < nodeTypes.Count; i++)
            {
                Type type = nodeTypes[i];
                string path = nodeNames[i];

                menu.AddItem(new GUIContent(path), type == currentType, () =>
                {
                    Undo.RecordObject(targetObject, "Set Behaviour Tree Node");
                    nodeProperty.managedReferenceValue = type == null ? null : Activator.CreateInstance(type);
                    serializedObject.ApplyModifiedProperties();
                    foldoutStates[nodeProperty.propertyPath] = true;
                });
            }

            menu.ShowAsContext();
        }


        // ==Foldout
        if (hasChildren)
        {
            if (!foldoutStates.TryGetValue(nodeProperty.propertyPath, out bool expanded)) expanded = true;
            expanded = EditorGUI.Foldout(foldoutRect, expanded, GUIContent.none, false);
            foldoutStates[nodeProperty.propertyPath] = expanded;
        }

        // ==Children
        if (!hasChildren) return;
        if (!foldoutStates.TryGetValue(nodeProperty.propertyPath, out bool nodeExpanded) || !nodeExpanded) return;

        SerializedProperty childrenProp = nodeProperty.FindPropertyRelative("Children");
        if (childrenProp == null) return;

        //remove if null
        for (int i = childrenProp.arraySize - 1; i >= 0; i--)
        {
            if (childrenProp.GetArrayElementAtIndex(i).managedReferenceValue == null)
                childrenProp.DeleteArrayElementAtIndex(i);
        }

        int desiredSlots = node.MaxChildren > 0 ? node.MaxChildren : childrenProp.arraySize + 1;
        while (childrenProp.arraySize < desiredSlots)
        {
            childrenProp.InsertArrayElementAtIndex(childrenProp.arraySize);
            childrenProp.GetArrayElementAtIndex(childrenProp.arraySize - 1).managedReferenceValue = null;
        }

        //draw the children 
        for (int i = 0; i < childrenProp.arraySize; i++)
        {
            SerializedProperty childProp = childrenProp.GetArrayElementAtIndex(i);

            // If infinite draw plus one
            if (node.MaxChildren < 0 && i > 0)
            {
                SerializedProperty prev = childrenProp.GetArrayElementAtIndex(i - 1);
                if (prev.managedReferenceValue == null) break;
            }

            bool childIsLast = i == childrenProp.arraySize - 1;
            List<bool> childAncestors = new List<bool>(ancestorIsLast) { !childIsLast ? false : true };
            DrawNodeSlot(childProp, childAncestors, childIsLast);
        }
    }


    private void DrawTreeLines(Rect row, List<bool> ancestorIsLast, bool isLast)
    {
        Handles.BeginGUI();
        Handles.color = TreeLineColor;
        float midY = row.y + row.height * 0.5f;

        for (int i = 0; i < ancestorIsLast.Count; i++)
        {
            float x = row.x + i * IndentWidth + IndentWidth * 0.5f;
            if (!ancestorIsLast[i])
            {
                Handles.DrawLine(new Vector3(x, row.y), new Vector3(x, row.yMax));
            }
        }

        if (ancestorIsLast.Count > 0)
        {
            float elbowX = row.x + (ancestorIsLast.Count - 1) * IndentWidth + IndentWidth * 0.5f;
            if (isLast)
            {
                Handles.DrawLine(new Vector3(elbowX, row.y), new Vector3(elbowX, midY));
                Handles.DrawLine(new Vector3(elbowX, midY), new Vector3(elbowX + IndentWidth * 0.5f, midY));
            }
            else
            {
                Handles.DrawLine(new Vector3(elbowX, row.y), new Vector3(elbowX, row.yMax));
                Handles.DrawLine(new Vector3(elbowX, midY), new Vector3(elbowX + IndentWidth * 0.5f, midY));
            }
        }

        Handles.EndGUI();
    }
}
