using System;
using System.Collections;
using System.Reflection;
using UnityEngine;


// Result that actions can yield
public class ActionComplete
{
    public bool Success { get; set; }
    
    public ActionComplete(bool success)
    {
        Success = success;
    }
}

[Serializable]
public class EnemyActionReference
{
    [SerializeField] private string _methodName;
    public string MethodName => _methodName;
    
    public IEnumerator Invoke(EnemyBase enemy, System.Action<bool> onComplete)
    {
        if (string.IsNullOrEmpty(_methodName))
        {
            onComplete?.Invoke(false);
            yield break;
        }
        
        // Look for method with Action<bool> parameter
        MethodInfo method = enemy.GetType().GetMethod(_methodName, BindingFlags.Public | BindingFlags.Instance, null,
            new Type[] { typeof(Action<bool>) },  // for onComplete<bool>
            null
        );
        
        if (method != null && method.ReturnType == typeof(IEnumerator))
        {
            IEnumerator coroutine = method.Invoke(enemy, new object[] { onComplete }) as IEnumerator;
            yield return coroutine;
        }
        else
        {
            Debug.LogWarning($"Method {_methodName} not found or invalid on {enemy.GetType().Name}");
            onComplete?.Invoke(false);
        }
    }
}