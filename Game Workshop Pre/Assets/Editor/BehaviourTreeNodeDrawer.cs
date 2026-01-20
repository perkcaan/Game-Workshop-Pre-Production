using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BehaviourTreeNode), true)]
public class BehaviourTreeNodeDrawer : PropertyDrawer
{
    // Per-owner cache key -> cached flattened node paths + signature
    private class CachedTree
    {
        public List<string> nodePaths = new List<string>();
        public int signature = 0;
    }

    private readonly Dictionary<string, CachedTree> cache = new();

    // Foldout state (per-node path)
    private readonly Dictionary<string, bool> nodeFoldouts = new();

    // Top-level foldout (per-owner)
    private readonly Dictionary<string, bool> topFoldouts = new();

    // UI constants
    private const float RowSpacing = 2f;
    private const float LeftPadding = 16f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Update before reading values and begin property
        property.serializedObject.Update();
        EditorGUI.BeginProperty(position, label, property);

        float y = position.y;

        // property label & button to edit the tree
        Rect labelRect = new Rect(position.x, y, position.width * 0.3f, EditorGUIUtility.singleLineHeight);
        Rect buttonRect = new Rect(position.x + position.width * 0.3f, y, position.width * 0.7f, EditorGUIUtility.singleLineHeight);

        EditorGUI.LabelField(labelRect, "Behaviour");
        if (GUI.Button(buttonRect, "Edit Behaviour Tree"))
        {
            BehaviourTreeEditorWindow.Open(property);
        }

        y += EditorGUIUtility.singleLineHeight + RowSpacing;

        if (property.managedReferenceValue is BehaviourTreeNode rootNode)
        {
            string ownerKey = GetOwnerKey(property);
            EnsureCacheUpToDate(property, ownerKey, rootNode);

            CachedTree tree = cache[ownerKey];

            // Top-level "Properties" foldout
            bool topExpanded = topFoldouts.TryGetValue(ownerKey, out bool enabled) ? enabled : true;
            Rect topRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
            topExpanded = EditorGUI.Foldout(topRect, topExpanded, "Properties", true);
            topFoldouts[ownerKey] = topExpanded;
            y += EditorGUIUtility.singleLineHeight + RowSpacing;

            if (topExpanded)
            {
                // Iterate cached node paths ( DFS order). Resolve a fresh SerializedProperty for each.
                foreach (string nodePath in tree.nodePaths)
                {
                    SerializedProperty nodeProp = property.serializedObject.FindProperty(nodePath);
                    if (nodeProp == null) continue;

                    BehaviourTreeNode node = nodeProp.managedReferenceValue as BehaviourTreeNode;
                    if (node == null) continue;

                    // Get fields to display
                    List<SerializedProperty> fields = GetEditableFields(nodeProp);
                    if (fields.Count == 0) continue; // skip nodes with no fields

                    // Node foldout
                    bool expanded = nodeFoldouts.TryGetValue(nodePath, out bool isEnabled) ? isEnabled : true;
                    Rect headerRect = new Rect(position.x + LeftPadding, y, position.width - LeftPadding, EditorGUIUtility.singleLineHeight);
                    GUIStyle boldFoldout = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };
                    expanded = EditorGUI.Foldout(headerRect, expanded, node.DisplayName ?? node.GetType().Name, true, boldFoldout);
                    nodeFoldouts[nodePath] = expanded;
                    y += EditorGUIUtility.singleLineHeight + RowSpacing;

                    if (expanded)
                    {
                        // Draw each field using a fresh SerializedProperty (so edits are tracked)
                        foreach (SerializedProperty fieldProp in fields)
                        {
                            float height = EditorGUI.GetPropertyHeight(fieldProp, true);
                            Rect fieldRect = new Rect(position.x + LeftPadding, y, position.width - LeftPadding, height);
                            EditorGUI.PropertyField(fieldRect, fieldProp, true);
                            y += height + RowSpacing;
                        }
                    }
                }
            }
        }

        EditorGUI.EndProperty();

        // Apply modifications at end so changes from inline edits are committed.
        property.serializedObject.ApplyModifiedProperties();
    }

    // Build or update cached flattened node path list if tree structure changed.
    private void EnsureCacheUpToDate(SerializedProperty rootProperty, string ownerKey, BehaviourTreeNode rootNode)
    {
        // Get signature
        int currentSignature = ComputeTreeSignature(rootNode);

        if (cache.TryGetValue(ownerKey, out CachedTree existingTree) && existingTree.signature == currentSignature) {
            return;//is valid
        }

        //else rebuild
        CachedTree newTree = new CachedTree { signature = currentSignature };
        // Use SerializedProperty-based flattening once to capture the authoritative property paths.
        FlattenNodeProperties(rootProperty, newTree.nodePaths);
        cache[ownerKey] = newTree;
    }

    // Tree signature is the sum of runtime hashcodes and node counts
    private int ComputeTreeSignature(BehaviourTreeNode rootNode)
    {
        if (rootNode == null) return 0;
        int sig = 0;
        Stack<BehaviourTreeNode> stack = new Stack<BehaviourTreeNode>();
        stack.Push(rootNode);

        unchecked // this removes overflow for hashing
        {
            while (stack.Count > 0)
            {
                BehaviourTreeNode node = stack.Pop();
                // hashing
                sig = sig * 31 + RuntimeHelpers.GetHashCode(node);
                sig = sig * 31 + node.GetType().GetHashCode();

                List<BehaviourTreeNode> children = node.Children;
                if (children != null)
                {
                    // iterate in reverse so DFS order matches flattening
                    for (int i = children.Count - 1; i >= 0; i--)
                    {
                        BehaviourTreeNode child = children[i];
                        if (child != null) stack.Push(child);
                    }
                }
            }
        }

        return sig;
    }

    // Flatten nodes, record SerializedProperty.propertyPath strings in DFS order
    private void FlattenNodeProperties(SerializedProperty nodeProp, List<string> outPaths)
    {
        BehaviourTreeNode node = nodeProp.managedReferenceValue as BehaviourTreeNode;
        if (node == null) return;

        outPaths.Add(nodeProp.propertyPath);

        SerializedProperty childrenProp = nodeProp.FindPropertyRelative("Children");
        if (childrenProp == null) return;

        int maxChildren = node.MaxChildren;
        int desiredSlots = maxChildren > 0 ? maxChildren : childrenProp.arraySize + 1;
        if (childrenProp.arraySize < desiredSlots) childrenProp.arraySize = desiredSlots;

        for (int i = 0; i < childrenProp.arraySize; i++)
        {
            // Infinite children end at first null...
            if (maxChildren < 0 && i > 0)
            {
                SerializedProperty prev = childrenProp.GetArrayElementAtIndex(i - 1);
                if (prev.managedReferenceValue == null) break;
            }

            SerializedProperty childProp = childrenProp.GetArrayElementAtIndex(i);
            if (childProp != null) {
                FlattenNodeProperties(childProp, outPaths);
            }
        }
    }

    // Return a list of SerializedProperty for fields to draw.
    private List<SerializedProperty> GetEditableFields(SerializedProperty nodeProp)
    {
        List<SerializedProperty> fields = new List<SerializedProperty>();

        SerializedProperty iter = nodeProp.Copy();
        SerializedProperty end = iter.GetEndProperty();

        // Advance to first visible child
        if (!iter.NextVisible(true)) return fields;

        do
        {
            string propName = iter.propertyPath;
            // Skip Editor-only fields
            if (propName.EndsWith("Children") || propName.EndsWith("DisplayName"))
            {
                continue;   
            }

            fields.Add(iter.Copy());
        } while (iter.NextVisible(false) && !SerializedProperty.EqualContents(iter, end));

        return fields;
    }

    // Make the cache key unique for every owner object and property path
    private string GetOwnerKey(SerializedProperty property)
    {
        int ownerId = property.serializedObject.targetObject != null ? property.serializedObject.targetObject.GetInstanceID() : 0;
        return ownerId + ":" + property.propertyPath;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent labellabel)
    {
        // Behaviour + Edit Behaviour Tree button
        float height = EditorGUIUtility.singleLineHeight + RowSpacing; 

        BehaviourTreeNode rootNode = property.managedReferenceValue as BehaviourTreeNode;
        if (rootNode == null) return height;

        string ownerKey = GetOwnerKey(property);
        EnsureCacheUpToDate(property, ownerKey, rootNode);

        CachedTree tree = cache[ownerKey];

        // Properties foldout
        bool topExpanded = topFoldouts.TryGetValue(ownerKey, out bool enabled) ? enabled : true;
        height += EditorGUIUtility.singleLineHeight + RowSpacing;

        if (!topExpanded) return height;

        // For each node + its visible fields
        foreach (string nodePath in tree.nodePaths)
        {
            SerializedProperty nodeProp = property.serializedObject.FindProperty(nodePath);
            if (nodeProp == null) continue;

            BehaviourTreeNode node = nodeProp.managedReferenceValue as BehaviourTreeNode;
            if (node == null) continue;

            List<SerializedProperty> fields = GetEditableFields(nodeProp);
            if (fields.Count == 0) continue;

            bool expanded = nodeFoldouts.TryGetValue(nodePath, out bool isEnabled) ? isEnabled : true;
            height += EditorGUIUtility.singleLineHeight + RowSpacing; // header
            if (expanded)
            {
                foreach (SerializedProperty field in fields)
                {
                    height += EditorGUI.GetPropertyHeight(field, true) + RowSpacing;
                }
            }
        }

        // return the final height
        return height;
    }
}
