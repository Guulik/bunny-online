using UnityEngine;

public abstract class SingletonMonoBehavior<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new();
    private static bool _isQuitting;
    public bool isDontDestroyOnLoad = true;

    public static T Instance
    {
        get
        {
            if (_isQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        var singletonObject = new GameObject(typeof(T).Name);
                        _instance = singletonObject.AddComponent<T>();
                        var singleton = _instance as SingletonMonoBehavior<T>;
                        if (singleton != null && singleton.isDontDestroyOnLoad) DontDestroyOnLoad(singletonObject);
                    }
                }

                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            var singleton = _instance as SingletonMonoBehavior<T>;
            if (singleton != null && singleton.isDontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }

    protected virtual void OnApplicationQuit()
    {
        _isQuitting = true;
    }
}