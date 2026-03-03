using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BehaviourTreeNodeRegistry
{
    private static List<Type> _allTypes;
    private static List<string> _displayNames;

    public static List<Type> GetAllNodeTypes()
    {
        if (_allTypes != null) return _allTypes;

        List<Type> allConcreteTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(BehaviourTreeNode).IsAssignableFrom(t) && !t.IsAbstract)
            .ToList();

        List<Type> withAttribute = allConcreteTypes
            .Where(t => t.GetCustomAttributes(typeof(BehaviourNodeAttribute), false).Length > 0)
            .OrderBy(t =>
            {
                var attr = (BehaviourNodeAttribute)t.GetCustomAttributes(typeof(BehaviourNodeAttribute), false).First();
                return attr.Priority;
            })
            .ToList();

        List<Type> withoutAttribute = allConcreteTypes
            .Where(t => t.GetCustomAttributes(typeof(BehaviourNodeAttribute), false).Length == 0)
            .OrderBy(t => t.Name)
            .ToList();

        _allTypes = withAttribute.Concat(withoutAttribute).ToList();

        // Prepare display names for dropdown (folder paths for organization)
        _displayNames = _allTypes.Select(t =>
        {
            BehaviourNodeAttribute attribute = (BehaviourNodeAttribute)t.GetCustomAttributes(typeof(BehaviourNodeAttribute), false).FirstOrDefault() as BehaviourNodeAttribute;
            return attribute != null && !string.IsNullOrEmpty(attribute.Folder) ? $"{attribute.Folder}/{t.Name}" : t.Name;
        }).ToList();

        // Insert "None" at the top
        _allTypes.Insert(0, null);
        _displayNames.Insert(0, "None");

        return _allTypes;
    }

    // Get label for selected type
    public static string[] GetDisplayNames()
    {
        if (_allTypes == null) GetAllNodeTypes();
        return _displayNames.ToArray();
    }

    
}
