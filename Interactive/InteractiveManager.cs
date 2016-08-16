using UnityEngine;
using System.Collections;

public class InteractiveManager : MonoBehaviourSingleton<InteractiveManager>
{
    /// <summary>
    /// Последний тачнутый обьект
    /// </summary>
    static public IInteractiveObject LastObject;

    /// <summary>
    /// Проверка тача на интерактив
    /// </summary>
    /// <returns></returns>
    static public bool CheckInteractive()
    {
        //Сбросить обьект
        LastObject = null;

        if (Application.isEditor)
        {
            if (Check(Camera.main.ScreenPointToRay(Input.mousePosition)))
                return true;
        }
        else
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Check(Camera.main.ScreenPointToRay(Input.touches[i].position)))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// проверка обьекта
    /// </summary>
    /// <param name="_ray"></param>
    /// <returns></returns>
    static bool Check(Ray _ray)
    {
        //Првоерить обьект
        RaycastHit _hit;
        if (Physics.Raycast(_ray, out _hit, 50.0f, 1 << 10))
        {
            LastObject = _hit.transform.GetComponent<IInteractiveObject>();
            return true;
        }

        return false;
    }

    //public override void Awake() { DontDestroyOnLoad(this); }
}
