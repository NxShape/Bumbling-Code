using UnityEngine;
using System.Collections;

public class IODialog : IInteractiveObject
{
    /// <summary>
    /// ИД диалога для запуска
    /// </summary>
    public int DialogID;

    /// <summary>
    /// Сообщение когда далеко
    /// </summary>
    public string MessageForDistance;

    /// <summary>
    /// Контролируемый моб
    /// </summary>
    public EnemyBase ControllingEnemy;

    /// <summary>
    /// Билборд 
    /// </summary>
    public GameObject QuestBillboard;

    /// <summary>
    /// Сообщение
    /// </summary>
    public TimingMessage Message;

    /// <summary>
    /// Требуемый ид квеста для показывания
    /// </summary>
    public int QuestID = -1;

    /// <summary>
    /// Требуемый ид таска
    /// </summary>
    public int TaskID = -1;

    public void Start()
    {
        QuestManager.OnUpdateQuest += OnUpdateQuest;
    }

    public void OnDestroy()
    {
        QuestManager.OnUpdateQuest -= OnUpdateQuest;
    }
    
    /// <summary>
    /// При удаче тапа
    /// </summary>
    public override void OnTouchDownSuccess()
    {
        //Остановить врага
        if (ControllingEnemy != null)
        {
            //Повернуьься
            ControllingEnemy.UpdateRotation(PlayerController.CachedTransform.position);

            //Подписатся на диалог
            Dialoguer.events.onEnded += OnEndDialog;
            Dialoguer.events.onMessageEvent += OnMessageEvent;
        }

        //Установить героя
        PlayerController.CachedTransform.forward = -transform.forward;

        //Запустить диалог
        DialogManager.Instance.StartDialog(DialogID);
    }

    /// <summary>
    /// При неудаче
    /// </summary>
    public override void OnTouchDownFailure()
    {
        //Показать сообщение
        if (ControllingEnemy != null)
            ControllingEnemy.SayText(MessageForDistance);
        else
        {
            if(Message == null)
                Message = GameUI.Instance.ShowMessage(MessageForDistance, transform);
        }
    }

    /// <summary>
    /// Окончание диалога
    /// </summary>
    public void OnEndDialog()
    {
    }

    /// <summary>
    /// Обновление квеста
    /// наличие квеста - вопрос
    /// просто диалог - спич
    /// активный квест - непоказывать
    /// </summary>
    /// <param name="_quest">_quest.</param>
    public void OnUpdateQuest()
    {
        if(QuestManager.Instance.IsDebug)
			Debug.Log("Quest Update: " + Defines.ActiveQuest.ID + " - " + Defines.ActiveQuest.ActiveTask + " -> " + transform.parent.name);
        
        //Активный таск
        if(QuestID == Defines.ActiveQuest.ID && TaskID == Defines.ActiveQuest.ActiveTask)
        {
            //Удалить билборд
            Destroy(QuestBillboard);
			QuestBillboard = null;
        }
        //Если у героя квест есть
        else if (Defines.CheckQuest(QuestID, TaskID))
		{
            if (QuestBillboard != null)
            {
                //Удалить старый
                if (!QuestBillboard.name.Equals("QuestBillboard"))
				{
					Destroy(QuestBillboard);
					QuestBillboard = null;
				}
	            else return;
            }
            
            //Создать билборд
            QuestBillboard = Billboard.CreateQuestBillboard();
            
            //Установить
            QuestBillboard.name = "QuestBillboard";
            QuestBillboard.transform.parent = transform;
            QuestBillboard.transform.localPosition = Vector3.zero;
        }
        //Если просто можно поговорить
        else
		{
            if (QuestBillboard != null)
            {
                //Удалить старый
                if (!QuestBillboard.name.Equals("SpeechBillboard"))
				{
                    Destroy(QuestBillboard);
					QuestBillboard = null;
				}
                else return;
            }
            
            //Создать билборд
            QuestBillboard = Billboard.CreateSpeechBillboard();
            
            //Установить
            QuestBillboard.name = "SpeechBillboard";
            QuestBillboard.transform.parent = transform;
            QuestBillboard.transform.localPosition = Vector3.zero;
        }
    }

    /// <summary>
    /// Собвтия диалога
    /// </summary>
    /// <param name="message"></param>
    /// <param name="metadata"></param>
    private void OnMessageEvent(string message, string metadata)
    {
        if (DialogManager.Instance.IsDebug) 
            Debug.Log("IODialog event: " + message);

        //сделать выборку
        switch (message)
        {
            case "SetAngree":
                Destroy(QuestBillboard.gameObject);
                break;
        }
    }
}
