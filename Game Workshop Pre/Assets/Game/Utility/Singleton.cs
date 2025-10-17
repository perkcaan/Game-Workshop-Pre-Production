using UnityEngine;

//if this gets put into a scene alongside another one, it will overwrite it
public class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour{
    // Properties
    public static T Instance { get; private set;}

    // Methods
    protected virtual void Awake() => Instance = this as T;
}

//if this gets put into a scene alongside another one, it will delete itself
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour {
    // Methods
    protected override void Awake() {
        if (Instance != null) Destroy(gameObject);
        base.Awake();
    }
}

//above, and survives through scene loads
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour {
    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
