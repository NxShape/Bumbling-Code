using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static Transform _cachedTransform;

    /// <summary>
    /// Получение ссылки на себя
    /// </summary>
    public static T Instance
    {
        get
        {
            //if(_instance == null) Cach();

            return _instance;
        }
    }

    /// <summary>
    /// Получение трансформы
    /// </summary>
    public static Transform CachedTransform
    {
        get
        {
            //if(_instance == null) Cach();

            return _cachedTransform;
        }
    }

    /// <summary>
    /// При создании
    /// </summary>
    public virtual void Awake()
    {
        _instance = GetComponent<T>();
        _cachedTransform = transform;
    }

//    /// <summary>
//    /// Получение всех элементов
//    /// </summary>
//    protected static void Cach()
//    {
//        if (_instance == null)
//        {
//            _instance = (T)FindObjectOfType(typeof(T));
//
//            if (_instance != null)
//                _cachedTransform = _instance.transform;
//        }
//
//		if(_cachedTransform == null && _instance != null)
//		{
//			_cachedTransform = _instance.transform;
//		}
//    }
}