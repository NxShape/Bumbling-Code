using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemyRespaunArea : MonoBehaviour
{
	/// <summary>
	/// Переменая паузы
	/// </summary>
	static public bool IsPause;

    /// <summary>
    /// ДЕфаулт точка для респа
    /// </summary>
    public Transform RespawnPoint;

    /// <summary>
    /// ИД врага для респауна
    /// </summary>
    public int IDEnemy;

    /// <summary>
    /// Время респауна
    /// </summary>
    public float TimeToRespaun;

    /// <summary>
    /// Временная величина
    /// </summary>
    public float DeltaTime;

    /// <summary>
    /// Количество врагов
    /// </summary>
    public int EnemiesCount
    {
        get
        {
            if (EnemiesCreated >= EnemiesCountBeforeKill)
                return EnemiesCountAfterKill;
            return EnemiesCountBeforeKill;
        }
    }

    /// <summary>
    /// Количество врагов до первоначального количества
    /// </summary>
    public int EnemiesCountBeforeKill;

    /// <summary>
    /// Количество врагов после первоначального респавна
    /// </summary>
    public int EnemiesCountAfterKill;

    /// <summary>
    /// Уже созданные враги
    /// </summary>
    public int EnemiesCreated;

    /// <summary>
    /// Созданные враги
    /// </summary>
    public List<EnemyBase> Created;

    /// <summary>
    /// Можно ли генерировать
    /// </summary>
    public bool CanGenerate;

    /// <summary>
    /// Радиус создания
    /// </summary>
    public float CreatingRadius;

    /// <summary>
    /// Изменение параметров врага при генерации
    /// </summary>
    public Action<EnemyBase> OnUpdateEnemyData = delegate{};

    public void Start()
    {
        EnemiesCreated = 0;
        DeltaTime = TimeToRespaun + UnityEngine.Random.Range(-2.5f, 2.5f);
    }

    public void Update()
    {
		if (IsPause) return;
        if(EnemyDatabase.Instance == null) return;
        if (!CanGenerate) return;
        if (Created.Count >= EnemiesCount) return;

        DeltaTime -= Time.deltaTime;

        if (DeltaTime <= 0.0f)
        {
            DeltaTime = TimeToRespaun + UnityEngine.Random.Range(-TimeToRespaun * 0.5f, TimeToRespaun * 0.5f);

            //Сгенерить
            GenerateEnemy();
        }
    }

    /// <summary>
    /// Генерация врага
    /// </summary>
    public void GenerateEnemy()
    {
        //Сгенерить врага
        EnemyBase _enemy = EnemyDatabase.Instance.GenerateEnemy(IDEnemy);

        //Правильно поставить моба
        if(RespawnPoint == null)
            _enemy.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f) * CreatingRadius, 0.0f, UnityEngine.Random.Range(-1.0f, 1.0f) * CreatingRadius);
        else
            _enemy.transform.position = RespawnPoint.position;

        //Указать точку спавна
        if (_enemy.PatrollController is AIRandomPointPatroll)
        {
            ((AIRandomPointPatroll)_enemy.PatrollController).InitPoint = transform.position;
            ((AIRandomPointPatroll)_enemy.PatrollController).RandomGenerate = new Vector2(CreatingRadius, CreatingRadius);
        }

        //Активировать модули
        _enemy.MovingController.Activate();

        //Если есть точка движения
        if(RespawnPoint != null)
        {
            _enemy.PatrollController.SetState(AIPatroll.EPatrollState.MOVE_TO);
            _enemy.MovingController.SetTarget(transform.position + new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f) * CreatingRadius, 0.0f, UnityEngine.Random.Range(-1.0f, 1.0f) * CreatingRadius));
        }

        //Установить делегат
        _enemy.EventDeath += OnEnemyDeath;

        //Обработка врага
        OnUpdateEnemyData(_enemy);

        //Запомнить
        Created.Add(_enemy);

        //Прибавить
        EnemiesCreated++;
    }

    /// <summary>
    /// Смерть врага
    /// </summary>
    public void OnEnemyDeath(EnemyBase _enemy)
    {
        //найти индекс
        int index = Created.FindIndex((p) => p == _enemy);

        //Проверить
        if (index != -1)
            Created.RemoveAt(index);
        else
            Debug.Log("In spawn " + gameObject.name + " no enemy " + _enemy.Data.ID);
    }

    /// <summary>
    /// Герой попал в зону тригера
    /// </summary>
    /// <param name="_col"></param>
    public void OnTriggerEnter(Collider _col)
    {
        CanGenerate = false;
    }

    /// <summary>
    /// Герой вышел из зону тригера
    /// </summary>
    /// <param name="_col"></param>
    public void OnTriggerExit(Collider _col)
    {
        CanGenerate = true;
    }

    /// <summary>
    /// Рисовалка
    /// </summary>
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, CreatingRadius);
    }
}
