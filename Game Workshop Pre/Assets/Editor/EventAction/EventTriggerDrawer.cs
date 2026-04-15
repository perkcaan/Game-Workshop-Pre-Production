using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System;

[CustomPropertyDrawer(typeof(SingleEventTrigger))]
public class SingleEventTriggerDrawer : PropertyDrawer
{
    private const float Spacing = 2f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float line = EditorGUIUtility.singleLineHeight;

        SerializedProperty targetObjectProp = property.FindPropertyRelative("_targetObject");
        if (targetObjectProp.objectReferenceValue == null) return line;

        float height = line * 2 + Spacing;

        SerializedProperty paramsProp = property.FindPropertyRelative("_parameters");
        if (paramsProp != null)
        {
            for (int i = 0; i < paramsProp.arraySize; i++)
            {
                SerializedProperty elem = paramsProp.GetArrayElementAtIndex(i);
                SerializedProperty valueProp = FindValueProperty(elem);
                if (valueProp != null) {
                    height += EditorGUI.GetPropertyHeight(valueProp, true) + Spacing;
                }
                else {
                    height += line + Spacing;
                }
            }
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty targetObjectProp    = property.FindPropertyRelative("_targetObject");
        SerializedProperty targetComponentProp = property.FindPropertyRelative("_targetComponent");
        SerializedProperty targetActionProp    = property.FindPropertyRelative("_targetAction");

        float line = EditorGUIUtility.singleLineHeight;
        Rect rectPos = new Rect(position.x, position.y, position.width, line);

        // 1. GameObject field
        EditorGUI.PropertyField(rectPos, targetObjectProp);
        rectPos.y += line + Spacing;

        GameObject gameObject = targetObjectProp.objectReferenceValue as GameObject;

        if (gameObject != null)
        {
            // 2. Build valid method list
            List<(Component component, MethodInfo method)> validMethods = new List<(Component component, MethodInfo method)>();
            List<string> displayNames = new List<string> { "None" };

            foreach (Component component in gameObject.GetComponents<Component>())
            {
                if (component == null) continue;

                foreach (MethodInfo method in component.GetType()
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (method.GetCustomAttribute<EventActionAttribute>() != null)
                    {
                        validMethods.Add((component, method));
                        displayNames.Add(component.GetType().Name + "/" + method.Name);
                    }
                }
            }

            // 3. Determine current index
            int selectedIndex = 0;
            for (int i = 0; i < validMethods.Count; i++)
            {
                if (validMethods[i].component == targetComponentProp.objectReferenceValue &&
                    validMethods[i].method.Name == targetActionProp.stringValue)
                {
                    selectedIndex = i + 1;
                    break;
                }
            }

            // 4. Action dropdown
            int newIndex = EditorGUI.Popup(rectPos, "Event Action", selectedIndex, displayNames.ToArray());
            rectPos.y += line + Spacing;

            // 5. Change selection with newIndex
            if (newIndex != selectedIndex)
            {
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();

                object targetObj = GetTargetObjectOfProperty(property);
                if (targetObj is SingleEventTrigger trigger)
                {
                    if (newIndex == 0) {
                        trigger.SetMethod(null, null);
                    }
                    else
                    {
                        int methodIndex = newIndex - 1;
                        if (methodIndex >= 0 && methodIndex < validMethods.Count)
                        {
                            (Component comp, MethodInfo method) = validMethods[methodIndex];
                            trigger.SetMethod(comp, method);
                        }
                    }
                }

                EditorUtility.SetDirty(property.serializedObject.targetObject);
                property.serializedObject.Update();
            }

            // 6. Draw parameters
            SerializedProperty paramsProp = property.FindPropertyRelative("_parameters");

            if (paramsProp != null && paramsProp.arraySize > 0)
            {
                for (int i = 0; i < paramsProp.arraySize; i++)
                {
                    SerializedProperty elem = paramsProp.GetArrayElementAtIndex(i);

                    // Get label from ParameterName
                    SerializedProperty nameProp = elem.FindPropertyRelative("ParameterName");
                    string paramLabel = (nameProp != null && !string.IsNullOrEmpty(nameProp.stringValue)) ? nameProp.stringValue : $"Param {i}";

                    // fails on fields declared in generic base classes
                    SerializedProperty valueProp = FindValueProperty(elem);

                    rectPos.height = line;

                    if (valueProp != null)
                    {
                        float h = EditorGUI.GetPropertyHeight(valueProp, true);
                        rectPos.height = h;
                        EditorGUI.PropertyField(rectPos, valueProp, new GUIContent(paramLabel), true);
                        rectPos.y += h + Spacing;
                    }
                    else
                    {
                        EditorGUI.LabelField(rectPos, paramLabel, "(not serializable)");
                        rectPos.y += line + Spacing;
                    }

                    rectPos.height = line;
                }
            }
        }

        EditorGUI.EndProperty();
    }

    private static SerializedProperty FindValueProperty(SerializedProperty parent)
    {
        SerializedProperty it  = parent.Copy();
        SerializedProperty end = parent.GetEndProperty();

        // Enter children
        if (!it.NextVisible(true)) return null;

        while (!SerializedProperty.EqualContents(it, end))
        {
            if (it.name == "_value") return it.Copy();
            it.NextVisible(false);
        }

        return null;
    }

    private static object GetTargetObjectOfProperty(SerializedProperty prop)
    {
        object obj = prop.serializedObject.targetObject;
        string path = prop.propertyPath.Replace(".Array.data[", "[");
        string[] parts = path.Split('.');

        foreach (string part in parts)
        {
            if (obj == null) return null;

            if (part.Contains("["))
            {
                int bracketIndex = part.IndexOf('[');
                string fieldPart = part.Substring(0, bracketIndex);
                int index = int.Parse(part.Substring(bracketIndex + 1, part.Length - bracketIndex - 2));

                if (!string.IsNullOrEmpty(fieldPart))
                {
                    FieldInfo fi = GetFieldRecursive(obj.GetType(), fieldPart);
                    obj = fi?.GetValue(obj);
                }

                System.Collections.IList list = obj as System.Collections.IList;
                obj = list?[index];
            }
            else
            {
                FieldInfo fi = GetFieldRecursive(obj.GetType(), part);
                obj = fi?.GetValue(obj);
            }
        }

        return obj;
    }

    private static FieldInfo GetFieldRecursive(Type type, string fieldName)
    {
        while (type != null)
        {
            FieldInfo fi = type.GetField(fieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fi != null) return fi;
            type = type.BaseType;
        }
        return null;
    }
}


[CustomPropertyDrawer(typeof(EventTrigger))]
public class EventTriggerDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty listProp = property.FindPropertyRelative("_eventTriggers");
        return EditorGUI.GetPropertyHeight(listProp, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty listProp = property.FindPropertyRelative("_eventTriggers");

        // Draw the list directly, removing one nesting level
        EditorGUI.PropertyField(position, listProp, label, true);

        EditorGUI.EndProperty();
    }
}