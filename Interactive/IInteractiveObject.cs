using UnityEngine;
using UnityEngine.EventSystems;

public class IInteractiveObject : MonoBehaviour, IPointerDownHandler
{
    /// <summary>
    /// Требуется ли проверять видисмость
    /// </summary>
    public bool NeedCheckVisible;

    /// <summary>
    /// Обьекты которые пропускаются
    /// </summary>
    public LayerMask Mask;

    /// <summary>
    /// Необходимая дистанция
    /// </summary>
    public float NeedDistance = -1.0f;

    /// <summary>
    /// Виртуальная функция тача удачная
    /// </summary>
    public virtual void OnTouchDownSuccess() { }

    /// <summary>
    /// Неудачный тач, отрабатывает когда предмет не виден
    /// </summary>
    public virtual void OnTouchDownFailure() { }

    #region Делегаты интерактивного обьекта
    public void OnPointerDown(PointerEventData eventData)
    {
        //Првоерка открытого окна
        if (WindowsController.Instance.ActiveWindow != EWindow.GAME_UI)
        {
            Debug.Log("Interactive not in GameUI");
            return;
        }

        //Првоерка условия
        if (!NeedCheckVisible) OnTouchDownSuccess();
        else
        {
            //Установить точки
            Vector3 _start = transform.position + new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 _end = PlayerController.CachedTransform.position + new Vector3(0.0f, 1.0f, 0.0f);

            //Проверка дистанции
            if (NeedDistance > 0.0f)
            {
                //Првоерка
                if (Vector3.Distance(_start, _end) > NeedDistance)
                {
                    Debug.Log("Interactive distance: " + Vector3.Distance(_start, _end) + " to " + NeedDistance);
                    OnTouchDownFailure();
                    return;
                }
            }

            //Сделать рэйкаст
            foreach (RaycastHit _hit in Physics.RaycastAll(_start, _end - _start, Vector3.Distance(_start, _end), Mask))
            {
                //проверка обьектов
                if (_hit.transform != transform && _hit.transform != PlayerController.CachedTransform)
                {
                    Debug.Log("Interactive visible: " + _hit.transform.name);

                    //проверить

                    OnTouchDownFailure();
                    return;
                }
            }

            //Если не было неудачных обьектов
            OnTouchDownSuccess();
        }
    }
    #endregion
}
