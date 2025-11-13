using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using System.Collections;

[CustomPropertyDrawer(typeof(SerializeReferenceDropdownAttribute))]
public class SerializeReferenceDropdownDrawer : PropertyDrawer {
    private static readonly Dictionary<Type, Type[]> _typeCache = new();
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if (property.propertyType != SerializedPropertyType.ManagedReference) {
            EditorGUI.PropertyField(position, property, label, true);
            return;
        }

        Type fieldType = GetFieldDeclaredType(property);
        if (fieldType == null) {
            EditorGUI.PropertyField(position, property, label, true);
            return;
        }

        //Determine current type and get derived types
        Type currentType = property.managedReferenceValue?.GetType();
        Type[] derivedTypes = GetDerivedTypes(fieldType);
        string currentLabel = currentType?.Name ?? "<None>";

        // Draw label + dropdown button
        Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        Rect buttonRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, label);

        if (EditorGUI.DropdownButton(buttonRect, new GUIContent(currentLabel), FocusType.Keyboard)) {
            GenericMenu menu = new GenericMenu();
            foreach (Type type in derivedTypes) {
                string typeName = type.Name;
                bool isSelected = currentType == type;
                menu.AddItem(new GUIContent(typeName), isSelected, () => {
                    property.serializedObject.Update();
                    property.managedReferenceValue = Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            menu.AddItem(new GUIContent("<None>"), currentType == null, () => {
                property.serializedObject.Update();
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            });

            menu.DropDown(buttonRect);
        }

        if (property.managedReferenceValue != null) {
            Rect contentRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2,
                position.width, EditorGUI.GetPropertyHeight(property, true));
            EditorGUI.PropertyField(contentRect, property, GUIContent.none, true);
        }
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        float height = EditorGUIUtility.singleLineHeight;
        if (property.propertyType == SerializedPropertyType.ManagedReference && property.managedReferenceValue != null) {
            height += EditorGUI.GetPropertyHeight(property, true) + 2;
        }
        return height;
    }

    private static Type[] GetDerivedTypes(Type baseType) {
        if (_typeCache.TryGetValue(baseType, out Type[] cached)) return cached;
        Type[] result = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(a => SafeGetTypes(a))
        .Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t))
        .ToArray();

        _typeCache[baseType] = result;
        return result;
    }

    private static IEnumerable<Type> SafeGetTypes(Assembly asm) {
        try { return asm.GetTypes(); }
        catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null);  }
    }

    //Attempts to find the field type of the property.
    private static Type GetFieldDeclaredType(SerializedProperty property) {
        object parentObject = GetParentObject(property);
        if (parentObject == null) return null;

        string propertyName = property.name;

        // Special handling if this property is an element inside a list/array
        if (property.propertyPath.EndsWith("]")) {
            // The last part is something like Array.data[index]
            // Need to get the collection field and its element type
            string path = property.propertyPath.Replace(".Array.data[", "[");
            int lastDot = path.LastIndexOf('.');
            string collectionFieldName = lastDot == -1 ? path : path.Substring(lastDot + 1);

            if (collectionFieldName.Contains("[")) {
                collectionFieldName = collectionFieldName.Substring(0, collectionFieldName.IndexOf("["));
            }

            FieldInfo collectionField = GetFieldInTypeHierarchy(parentObject.GetType(), collectionFieldName);
            if (collectionField == null) return null;

            Type collectionType = collectionField.FieldType;

            if (collectionType.IsArray) {
                return collectionType.GetElementType();
            }
            else if (typeof(IEnumerable).IsAssignableFrom(collectionType) && collectionType.IsGenericType) {
                return collectionType.GetGenericArguments()[0];
            }
            return null;
        }
        else {
            // Normal property — get field from parent object type
            FieldInfo field = GetFieldInTypeHierarchy(parentObject.GetType(), propertyName);
            if (field == null) return null;

            return field.FieldType;
        }
    }

    private static FieldInfo GetFieldInTypeHierarchy(Type type, string fieldName) {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        while (type != null) {
            FieldInfo field = type.GetField(fieldName, flags);
            if (field != null) return field;
            type = type.BaseType;
        }
        return null;
    }

    private static object GetParentObject(SerializedProperty property) {
        object obj = property.serializedObject.targetObject;
        string path = property.propertyPath.Replace(".Array.data[", "[");
        string[] elements = path.Split('.');
        foreach (string element in elements.Take(elements.Length - 1)) {
            if (element.Contains("[")) {
                string fieldName = element.Substring(0, element.IndexOf("["));
                int index = int.Parse(element.Substring(element.IndexOf("[") + 1, element.IndexOf("]") - element.IndexOf("[") - 1));
                obj = GetFieldValue(obj, fieldName);
                if (obj is IList list && index < list.Count) {
                    obj = list[index];
                }
            }
            else {
                obj = GetFieldValue(obj, element);
            }
            if (obj == null) return null;
        }
        return obj;
    }

    private static object GetFieldValue(object obj, string name) {
        if (obj == null) return null;
        Type type = obj.GetType();
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        while (type != null) {
            FieldInfo field = type.GetField(name, flags);
            if (field != null) return field.GetValue(obj);
            type = type.BaseType;
        }
        return null;
    }
}