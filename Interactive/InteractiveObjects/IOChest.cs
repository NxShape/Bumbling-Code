using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IOChest : IInteractiveObject
{
    /// <summary>
    /// Анимации сундука
    /// </summary>
    public Animator Anim;

    /// <summary>
    /// Был ли уже открыт сундук
    /// </summary>
    public bool IsOpen;

    /// <summary>
    /// Эффект открывания
    /// </summary>
    public GameObject Effect;

    /// <summary>
    /// ДАныне о дропе
    /// </summary>
    public DroperData DropData;

    /// <summary>
    /// Контроллер видимости
    /// </summary>
    public VisibleController Visible;

    /// <summary>
    /// Запуск первого
    /// </summary>
    public void LaunchItem()
    {
        //Выкинуть предмет
        DropData.Drop();
    }

    /// <summary>
    /// Сброс сундука
    /// </summary>
    void Reset()
    {
        Anim.SetBool("Close", false);
    }

    /// <summary>
    /// Тач на предмете
    /// </summary>
    public override void OnTouchDownSuccess()
    {
        //Проверка
        if (IsOpen) return;

        //Установить флаг
        IsOpen = true;

        //Сгенерировать дроп
        DropData.GenerateDrop();

        //Включить эффект
        Effect.SetActive(true);

        //Запуск анимации
        Anim.SetBool("Open_1", true);
    }

    /// <summary>
    /// Неудачный тап на сундуке
    /// </summary>
    public override void OnTouchDownFailure()
    {
        //Проверка
        if (IsOpen) return;

        //Запуск анимации
        Anim.SetBool("Close", true);
        Invoke("Reset", 0.1f);
    }

    public void Update()
    {
        if(Visible.IsVisible && !Anim.enabled) Anim.enabled = true;
        else if(!Visible.IsVisible && Anim.enabled) Anim.enabled = false;
    }
}
