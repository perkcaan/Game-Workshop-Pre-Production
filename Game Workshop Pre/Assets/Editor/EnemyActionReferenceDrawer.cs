using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnemyActionReference))]
public class EnemyActionReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        SerializedProperty methodNameProp = property.FindPropertyRelative("_methodName");
        
        //get the enemy component. Check from most obvious to least obvious
        EnemyBase enemy = null;
        
        if (property.serializedObject.targetObject is EnemyBase directEnemy)
        {
            enemy = directEnemy;
        }
        else if (property.serializedObject.targetObject is GameObject go)
        {
            enemy = go.GetComponent<EnemyBase>();
        }
        else if (property.serializedObject.targetObject is MonoBehaviour mb)
        {
            enemy = mb.GetComponent<EnemyBase>();
        }

        //safety check
        if (enemy == null)
        {
            EditorGUI.LabelField(position, label.text, "Not on an EnemyBase");
            EditorGUI.EndProperty();
            return;
        }
        
        // Get all valid action methods
        List<MethodInfo> validMethods = GetValidActionMethods(enemy.GetType());
        List<string> methodNames = validMethods.Select(m => m.Name).ToList();
        methodNames.Insert(0, "None");
        
        // Find current selection
        int currentIndex = 0;
        if (!string.IsNullOrEmpty(methodNameProp.stringValue))
        {
            currentIndex = methodNames.IndexOf(methodNameProp.stringValue);
            if (currentIndex == -1) currentIndex = 0;
        }
        
        // always show Element: [index]
        string displayLabel = label.text;
        if (property.propertyPath.Contains("Array.data["))
        {
            int startIndex = property.propertyPath.IndexOf("[") + 1;
            int endIndex = property.propertyPath.IndexOf("]");
            string index = property.propertyPath.Substring(startIndex, endIndex - startIndex);
            displayLabel = $"Element {index}";
        }
        
        // Draw dropdown with consistent label
        int newIndex = EditorGUI.Popup(position, displayLabel, currentIndex, methodNames.ToArray());
        
        // and then update value if changed
        if (newIndex != currentIndex)
        {
            methodNameProp.stringValue = newIndex == 0 ? "" : methodNames[newIndex];
        }
        
        EditorGUI.EndProperty();
    }
    
    private List<MethodInfo> GetValidActionMethods(System.Type enemyType)
    {
        return enemyType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.ReturnType == typeof(System.Collections.IEnumerator))
            .Where(m => {
                ParameterInfo[] parameters = m.GetParameters();
                // Must have exactly 1 parameter of type Action<bool>
                return parameters.Length == 1 && parameters[0].ParameterType == typeof(System.Action<bool>);
            })
            .ToList();
    }
}