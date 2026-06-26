using UnityEngine;
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance = null;
    private static bool _isQuiting = false;

    public virtual string ObjectName { get { return typeof(T).Name; } }

    public static T Instance
    {
        get
        {
            if (_instance == null && !_isQuiting)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying && Application.isEditor)
                {
                    return null;
                }
#endif

                Debug.LogFormat("Singleton: Creating new {0} gameObject", typeof(T).Name);
                var go = new GameObject().AddComponent<T>();
                if (go != null)
                {
                    go.name = go.ObjectName;
                }
            }
            return _instance;
        }
    }

    protected bool IsSingletonInstance => _instance == (T)this;

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = (T)this;
        }
        if (_instance != null && _instance != this)
        {
            Debug.LogWarningFormat("Destroying extra Instance of {0}", typeof(T).Name);
            Destroy(this.gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == (T)this)
        {
            _instance = null;
        }
    }

    private void OnApplicationQuit()
    {
        _isQuiting = true;
    }
}