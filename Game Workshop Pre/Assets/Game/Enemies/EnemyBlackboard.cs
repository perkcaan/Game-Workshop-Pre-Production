using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// A blackboard is a way to more loosely store runtime data. Good for modular things like behaviour trees.
// This one is used for enemies
public class EnemyBlackboard
{

    // Fields
    //Reference to self
    private EnemyBase _self;
    public EnemyBase Self { get { return _self; } } 

    private Dictionary<string, object> _data = new Dictionary<string, object>();

    // Constructor
    public EnemyBlackboard(EnemyBase self)
    {
        _self = self;
    }

    // Methods
    public void Set<T>(string key, T value)
    {
        _data[key] = value;
    }

    public bool TryGet<T>(string key, out T value)
    {
        if (_data.TryGetValue(key, out object obj) && obj is T typed)
        {
            value = typed;
            return true;
        }
        value = default;
        return false;
    }

    public bool TryGetNotNull<T>(string key, out T value) where T : class
    {
        if (_data.TryGetValue(key, out object obj) && obj is T typed && typed != null)
        {
            value = typed;
            return true;
        }

        value = null;
        return false;
    }

    public bool HasKey(string key)
    {

        return _data.ContainsKey(key);
    }

    public void Remove(string key)
    {
        if (_data.ContainsKey(key))
        {
            _data.Remove(key);
        }
    }
}
