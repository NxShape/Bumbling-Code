using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public enum E_AIState
{
    NONE,
    PATROLL,
    ATTACK,
    DETECT
}

public class EnemyBase : MonoBehaviour, IReceivedDamage, IFastPoolItem
{
    /// <summary>
    /// Смерть событие смерти локальное
    /// </summary>
    public event Action<EnemyBase> EventDeath = delegate { };

    /// <summary>
    /// Трансформа
    /// </summary>
    public Transform Root;

    /// <summary>
    /// Инициализировался ли враг
    /// </summary>
    public bool IsInitialized;

    /// <summary>
    /// Идентификатор врага
    /// </summary>
    public EnemyData Data;

    /// <summary>
    /// Даные о дропе
    /// </summary>
    public DroperData DropingData;

    /// <summary>
    /// Оружие
    /// </summary>
    public WeaponBase Weapon;

    /// <summary>
    /// Тело
    /// </summary>
    public CharacterController Body;

    /// <summary>
    /// Агресивный или нет
    /// </summary>
    public bool IsAngree;

    /// <summary>
    /// Напаузе или нет
    /// </summary>
    public bool IsPause;

    /// <summary>
    /// Умер или нет
    /// </summary>
    public bool IsDead
    {
        get { return _health <= 0; }
    }

    /// <summary>
    /// Уровень врага
    /// </summary>
    public int EnemyLevel;

    /// <summary>
    /// Жизни
    /// </summary>
    public int Health
    {
        get { return _health; }
        set
        {
            _health = value;

            //Обновить строку
            if (HPBar != null)
                HPBar.value = (float)_health / (float)MaxHealth;
        }
    }

    [SerializeField]
    int _health;

    /// <summary>
    /// Максимальные жизни
    /// </summary>
    public int MaxHealth;

    /// <summary>
    /// Урон
    /// </summary>
    public int Damage;

    /// <summary>
    /// Эффект смерти
    /// </summary>
    public GameObject DeathEffect;

    /// <summary>
    /// Масштабирование жизней
    /// </summary>
    public float HPBarScale;

    /// <summary>
    /// Строка жизней
    /// </summary>
    public UIProgressBar HPBar;

    /// <summary>
    /// Контроллер видимости
    /// </summary>
    public VisibleController Visible;

    /// <summary>
    /// Состояние контроллеров
    /// </summary>
    public E_AIState State;

    /// <summary>
    /// Контроллер движения
    /// </summary>
    public AIMoving MovingController;

    /// <summary>
    /// Скрипт патрулирования
    /// </summary>
    public AIPatroll PatrollController;

    /// <summary>
    /// Детектирование атаки
    /// </summary>
    public AIDetect DetectController;

    /// <summary>
    /// Скрипт атаки
    /// </summary>
    public AIAttack AttackController;

    /// <summary>
    /// Модуль детектирования
    /// </summary>
    public GameObject DetectModule;
     
    /// <summary>
    /// Атакует ли сейчас моб
    /// </summary>
    public bool IsAttackStarted;

    /// <summary>
    /// Тень
    /// </summary>
    public GameObject Shadow;

    /// <summary>
    /// ЗАвиксированый обьект
    /// </summary>
    public Transform Detected;

    /// <summary>
    /// Активное сообщение
    /// </summary>
    public TimingMessage ActiveMessage;

    public void OnDestroy()
    {
        if (EnemyDatabase.Instance != null)
            EnemyDatabase.Instance.CreatedEnemies.Remove(this);
    }

    /// <summary>
    /// Инициализация через пул
    /// </summary>
    public virtual void Start()
    {   
        //Запустить регистрацию
        if(!IsInitialized)
        {
            StartCoroutine("Register");

            //Установить функции видимости
            Visible.OnChangeVisible = (_visible) =>
            {
                //Если стал видимым
                if(_visible)
                {
                    //Включить бар если он злой
                    if(IsAngree && HPBar != null)
                        HPBar.gameObject.SetActive(true);

                    //Включить модули
                    if (DetectModule != null) DetectModule.SetActive(true);
                    Body.enabled = true;
                    Shadow.SetActive(true);
                }
                else
                {
                    //Выключить бар
                    if(HPBar != null) 
                        HPBar.gameObject.SetActive(false);

                    if (DetectModule != null) DetectModule.SetActive(false);
                    Body.enabled = false;
                    Shadow.SetActive(false);
                    MovingController.AnimatorController.enabled = false;
                }

                //Обновить контроллеры
                if(MovingController != null)
                    MovingController.UpdateVisible(_visible);
                if(PatrollController != null)
                    PatrollController.UpdateVisible(_visible);
                if(AttackController != null)
                    AttackController.UpdateVisible(_visible);
                if(DetectController != null)
                    DetectController.UpdateVisible(_visible);
            };
        }

        //проверка бара
        if(EnemyHPManager.Instance != null)
        {
            HPBar = EnemyHPManager.Instance.CreateHPBar(false);

            //Установка настроек для бара
            HPBar.transform.localScale = Vector3.one * HPBarScale;
        }

        //Сбросить
        OnFastInstantiate();
    }

    /// <summary>
    /// Регенерация
    /// </summary>
    public void OnFastInstantiate()
    {
        Visible.OnChangeVisible(false);

        //Сбросить данные
        ResetData();

        //генерация лута
        if (DropingData != null)
            DropingData.GenerateDrop();

//        //Активировать модули
//        MovingController.Activate();
    }

    public void OnFastDestroy()
    {
        //Отключить аниматор
        MovingController.Deactivate();
    }

    /// <summary>
    /// Коротина регистрации обьекта
    /// </summary>
    IEnumerator Register()
    {
        //Ожидание инициализации бд
        while (EnemyDatabase.Instance == null)
        {
            yield return null;
        }

        //Внедрение в базу
        Root.parent = EnemyDatabase.CachedTransform;
        EnemyDatabase.Instance.CreatedEnemies.Add(this);

        //Установить инициализацию
        IsInitialized = true;
    }

    /// <summary>
    /// Сброс параметров врага
    /// </summary>
    public void ResetData()
    {
        Health = MaxHealth = (int)(Data.Health * Data.HealthCurve.Evaluate(Defines.Level));
        Damage = (int)(Data.Damage * Data.DamageCurve.Evaluate(Defines.Level));
    }

    /// <summary>
    /// Повернуть моба по направлению цели
    /// </summary>
    public void UpdateRotation(Vector3 _target)
    {
        //Поворот
        Vector3 _forward = (_target - Root.position).normalized;
        _forward.y = 0.0f;

        if (_forward != Vector3.zero)
            Root.rotation = Quaternion.LookRotation(_forward);
    }

    #region IReceivedDamage implementation
    /// <summary>
    /// Raises the receive damage event.
    /// </summary>
    /// <param name="_damage">Damage.</param>
    /// <param name="_power">Power.</param>
    /// <param name="_direction">Direction.</param>
    bool IReceivedDamage.OnReceiveDamage(int _damage, float _power, Vector3 _direction)
    {
        return _receiveDamage(_damage, _power, _direction);
    }

    /// <summary>
    /// Получение урона.
    /// </summary>
    /// <returns><c>true</c>, if damage was received, <c>false</c> otherwise.</returns>
    /// <param name="_damage">Damage.</param>
    /// <param name="_power">Power.</param>
    /// <param name="_direction">Direction.</param>
    protected virtual bool _receiveDamage(int _damage, float _power, Vector3 _direction)
    {
        //Првоерить
        if (!IsDead)
        {
            //Проверка хп
            Health -= _damage;

            //Если не умер после урона
            if (!IsDead)
            {
                //Повернуть
                transform.forward = _direction.normalized;

                //Проверить силу
                MovingController.AnimatorController.SetTrigger(Data.StrongReceivedDamageParameter < _power ? "StrongReceive" : "MiddleReceive");
            } else
            {
                //Убрать бар
                HPBar.gameObject.SetActive(false);

                //Скрыть агента и колайдеры
                Body.enabled = false;
                DetectModule.SetActive(false);

                //Деактивировать модули
                MovingController.Deactivate();

                //Убросить состояния
                State = E_AIState.PATROLL;
                PatrollController.SetState(AIPatroll.EPatrollState.WAIT);

                //Вызвать событие
                EventDeath(this);
                Defines.FireOnDeath(this);
                
                //Запустить выброс предметов
                DropingData.Drop();

                //Запуск смерти
                StartCoroutine("LaunchDeath");

                return true;
            }
        }

        return false;
    }

    #endregion

    /// <summary>
    /// Обновление
    /// </summary>
    public virtual void Update()
    {
        //Проверка
        if (IsDead || IsPause || !IsInitialized)
            return;

        //Обновить ИИ
        switch (State)
        {
            case E_AIState.PATROLL:
                PatrollController.UpdateState(ref State);
                break;
            case E_AIState.DETECT:
                DetectController.UpdateState(ref State);
                break;
            case E_AIState.ATTACK:
                AttackController.UpdateState(ref State);
                break;
        }
		
        //Обновление движения
        MovingController.UpdateState(ref State);

        //Обновление бара
        if (HPBar == null)
            return;
        
        //Есди есть бар переместить
        if (Visible.IsVisible && HPBar.gameObject.activeSelf)
        {
            //Переместить
            HPBar.transform.position = Root.position;
            HPBar.transform.localEulerAngles = new Vector3(-90.0f, 0.0f, transform.localEulerAngles.y - 180.0f);
        }
    }

    /// <summary>
    /// Сказать сообщение
    /// </summary>
    /// <param name="_message"></param>
    public void SayText(string _message)
    {
        //Если он не говорит ничего сейчас
        if (ActiveMessage == null)
            ActiveMessage = GameUI.Instance.ShowMessage(_message, transform);
    }

    /// <summary>
    /// Установка агра и переход в наступление
    /// </summary>
    public void SetAgree(Transform _target = null)
    {
        //проверка
        if (_target != null)
            Detected = _target;

        //Установка режима
        AttackController.Activate();

        //Сбросить состояния
        IsAttackStarted = false;
        IsAngree = true;

        //Установить статус
        State = E_AIState.ATTACK;
    }

    /// <summary>
    /// Вход в тригер детекта
    /// </summary>
    /// <param name="_col"></param>
    public void OnTriggerEnter(Collider _col)
    {
        if (!_col.CompareTag("Player"))
            return;
        if(IsDead) return;

        //Установить детектинг
        Detected = _col.transform;

        //Вызов функций
        OnEnemyDetected();
    }

    /// <summary>
    /// Выход в тригер детекта
    /// </summary>
    /// <param name="_col"></param>
    public void OnTriggerExit(Collider _col)
    {
        if (!_col.CompareTag("Player"))
            return;
        if(IsDead) return;

        //Вызов функций
        OnEnemyLose();
    }

    /// <summary>
    /// Событие при обнаружении врага
    /// </summary>
    public virtual void OnEnemyDetected()
    {
        //обновить
        PatrollController.Reset();
        DetectController.Reset();

        //Остановить движение
        MovingController.Stop();

        //Показать оповещение
        EnemyDatabase.Instance.CreateDetectAction(Root);

        //Установить агресию
        if (MovingController.AnimatorController.isInitialized)
            MovingController.AnimatorController.SetBool("Aggressive", true);
        
        //обновить состояния
        State = E_AIState.DETECT;
    }

    /// <summary>
    /// Событие при потери врага
    /// </summary>
    public virtual void OnEnemyLose()
    {
        //Установить детектинг
        Detected = null;

        //Установить агресию
        if (MovingController.AnimatorController.isInitialized)
            MovingController.AnimatorController.SetBool("Aggressive", false);

        //обновить состояния
        State = E_AIState.PATROLL;
    }

    /// <summary>
    /// Запуск анимации атаки
    /// </summary>
    public virtual void StartAttack()
    {
        IsAttackStarted = true;
        MovingController.AnimatorController.SetBool("Attack", true);
    }

    /// <summary>
    /// Окончание движения к цели
    /// </summary>
    public void OnEndMoving()
    {
        if (State == E_AIState.PATROLL && PatrollController != null)
        {
            if(PatrollController.State == AIPatroll.EPatrollState.MOVE_TO)
                PatrollController.SetState(AIPatroll.EPatrollState.WAIT);
            else
                PatrollController.ChangeTarget();
        }
    }

    /// <summary>
    /// Коротина смерти
    /// </summary>
    /// <returns>The death.</returns>
    IEnumerator LaunchDeath()
    {
        //Запустить смерть
        MovingController.AnimatorController.SetTrigger("Death");

        yield return new WaitForSeconds(4.0f);

        MovingController.AnimatorController.enabled = false;
        MovingController.AnimatorController.applyRootMotion = false;

        //Цикл погружени
        float _delta = 0.0f;
        do
        {
            _delta += Time.deltaTime;
            
            transform.position -= new Vector3(0.0f, _delta * 0.02f, 0.0f);
            
            yield return new WaitForEndOfFrame();
        } while (_delta < 5.0f); 

        //Отключить аниматор
        MovingController.UpdateVisible(false);

        //Пул дейстрой
        Data.Pool.FastDestroy(gameObject);
    }

    /// <summary>
    /// Получил пинок от игрока
    /// </summary>
    /// <param name="_direction">Direction.</param>
    public virtual void OnPlayerPull(Vector3 _direction)
    {
    }

    #region События из анимаций

    /// <summary>
    /// Событие из анимации запуска атаки
    /// </summary>
    public virtual void OnStartAttack()
    {
        MovingController.AnimatorController.SetBool("Attack", false);
    }

    /// <summary>
    /// Событие атаки из анимации
    /// </summary>
    public virtual void OnAttack()
    { 
        //Проверка цели
        if (Detected == null)
        {
            OnEnemyLose();
            return;
        }

        //Првоерка расстояния
        if (AttackController.CanAttack())
        {
            //Нанести урон
            if (Detected.GetComponent<IReceivedDamage>().OnReceiveDamage(Damage, 0.0f, Vector3.zero))
                OnEnemyLose();
        }
    }

    /// <summary>
    /// Событие окончания атаки
    /// </summary>
    public virtual void OnEndAttack()
    { 
        IsAttackStarted = false;
        AttackController.Activate();
    }

    /// <summary>
    /// Восстановление после удара
    /// </summary>
    public virtual void OnResetAfterDamage()
    {
        //IsReceivedDamage = false;
        IsAttackStarted = false;
    }

    #endregion
}
