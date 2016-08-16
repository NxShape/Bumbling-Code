using UnityEngine;
using System.Collections;

public class IOMessage : IInteractiveObject
{
    /// <summary>
    /// Сообщение
    /// </summary>
    public string Message;
    
    /// <summary>
    /// Созданое сообщение
    /// </summary>
    public TimingMessage CreatedMessage;

    /// <summary>
    /// Автопоказ сообщения
    /// </summary>
    public bool AutoShowMessage;

    /// <summary>
    /// Контроллер видимости
    /// </summary>
    public VisibleController Visible;

    public override void OnTouchDownSuccess()
    {
        if(CreatedMessage) return;

        //Создать сообщение
        CreatedMessage = GameUI.Instance.ShowMessage(Message, transform);   
    }

    public void Update()
    {
        if(DialogManager.Instance == null) return;
        if(DialogManager.Instance.DialogStarter) return;
        if(!AutoShowMessage) return;
        if(!Visible.IsVisible) return;

        //Создать сообщение
        if(CreatedMessage == null)
            CreatedMessage = GameUI.Instance.ShowMessage(Message, transform);  
    }
}
