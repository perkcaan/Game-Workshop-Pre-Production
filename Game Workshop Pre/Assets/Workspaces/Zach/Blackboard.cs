using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// A blackboard is a way to more loosely store runtime data. Good for modular things like behaviour trees.
public class Blackboard
{

    //temporary enemy reference
    public BehaviourTreeTest btt;

    private Dictionary<string, object> data = new Dictionary<string, object>();

    public void Set<T>(string key, T value)
    {
        data[key] = value;
    }

    public bool TryGet<T>(string key, out T value)
    {
        if (data.TryGetValue(key, out object obj) && obj is T typed)
        {
            value = typed;
            return true;
        }
        value = default;
        return false;
    }

    public bool HasKey(string key)
    {
        
        return data.ContainsKey(key);
    }

    public void Remove(string key)
    {
        if (data.ContainsKey(key))
        {
            data.Remove(key);
        }
    }
}
