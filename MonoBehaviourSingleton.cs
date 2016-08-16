using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T _instance;
    public static Transform _cachedTransform;
    public static GameObject _root;

    /// <summary>
    /// Получение ссылки на себя
    /// </summary>
    public static T Instance
    {
        get
        {
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
        _root = _cachedTransform.GetChild(0).gameObject;
    }

    /// <summary>
    /// Открыть окно
    /// </summary>
    static public void Open(bool _action = true)
    {
        _root.SetActive(true);

        //Если надо вызвать событие
        if (_action)
            _cachedTransform.SendMessage("OpenAction", SendMessageOptions.DontRequireReceiver);
    }

    /// <summary>
    /// Закрыть окно
    /// </summary>
    static public void Hide(bool _action = true)
    {
        _root.SetActive(false);

        //Если надо вызвать событие
        if (_action)
            _cachedTransform.SendMessage("CloseAction", SendMessageOptions.DontRequireReceiver);
    }

    /// <summary>
    /// Зaдник
    /// </summary>
    public TextureLoader Background;

    /// <summary>
    /// Событие по открыванию
    /// </summary>
    public virtual void OpenAction()
    {
    }

    /// <summary>
    /// Событие закрытия
    /// </summary>
    public virtual void CloseAction()
    {
    }
}