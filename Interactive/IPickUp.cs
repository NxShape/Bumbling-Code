using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class IPickUp : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    /// <summary>
    /// Событие подбора предмета
    /// </summary>
    static public event Action<IItem> PickUpEvent = delegate { };

    /// <summary>
    /// Предмета из бд
    /// </summary>
    //public string ItemID;
    public IItem Item;

    /// <summary>
    /// Есть литач на обьекте
    /// </summary>
    public bool IsTouch;

    /// <summary>
    /// анимация
    /// </summary>
    public Animator Anim;

    /// <summary>
    /// Твин перемещения
    /// </summary>
    public TweenPosition Tween;

    /// <summary>
    /// Твин масштабирования
    /// </summary>
    public TweenScale TweenScale;

    /// <summary>
    /// Эффект подбора
    /// </summary>
    public GameObject PickUpEffect;

    /// <summary>
    /// Колайдер для связи
    /// </summary>
    public Collider TouchCollider;

    public void OnTriggerEnter(Collider _col)
    {
        if (_col.CompareTag("Player"))
            OnMoveTo(_col.transform.position);
    }

    /// <summary>
    /// Функция подьема предмета
    /// </summary>
    public virtual void OnPickUp()
    {
        //Убрать колайдер
        TouchCollider.enabled = false;

        //Создать эффект
        PickUpEffect.transform.parent = null;
        PickUpEffect.transform.localScale = Vector3.one;
        PickUpEffect.SetActive(true);

        //Вызвать событие
        PickUpEvent(Item);
        
        //Уничтожить
        Destroy(gameObject);
    }

    /// <summary>
    /// Перемещение обьекта к позиции
    /// </summary>
    /// <param name="_position"></param>
    public virtual void OnMoveTo(Vector3 _position)
    {
        //Убрать колайдер
        TouchCollider.enabled = false;

        //Убрать родителя
        transform.parent = null;

        //Запустить твин
        Tween.from = transform.position;
        Tween.to = _position + new Vector3(0.0f, 1.0f, 0.0f);
        Tween.PlayForward();
        TweenScale.PlayForward();
    }

    #region Функции интерактивного обьекта
    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsTouch) return;
        if (WindowsController.Instance.ActiveWindow != EWindow.GAME_UI) return;

        IsTouch = true;
        StartCoroutine("CheckTouchTime");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Проверка
        if (WindowsController.Instance.ActiveWindow != EWindow.GAME_UI) return;

        //Остановить коротину
        StopCoroutine("CheckTouchTime");

        //проверить над предметом ли еще
        if (InteractiveManager.CheckInteractive())
            OnPickUp();
    }
    #endregion

    /// <summary>
    /// Коротина ожидания общего сбора
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckTouchTime()
    {
        yield return new WaitForSeconds(1.0f);

        //Найти все обьекты интерактива рядом
        foreach(Collider _col in Physics.OverlapSphere(transform.position, 5.0f, 1 << 10))
        {
            //Проверить подбор
            IPickUp _item = _col.GetComponent<IPickUp>();

            //Проверить скрипт
            if (_item == null) continue;

            //Поднть предмет
            _item.OnMoveTo(transform.position);
        }
    }
}
