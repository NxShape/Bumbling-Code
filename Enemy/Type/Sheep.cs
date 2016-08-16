using UnityEngine;
using System.Collections;

public class Sheep : EnemyBase
{
    /// <summary>
    /// Ссылка на игрока
    /// </summary>
    public Transform FollowTarget;

    /// <summary>
    /// Дистанция для следования
    /// </summary>
    public float FollowDistance;

    /// <summary>
    /// Минимальная дистанция
    /// </summary>
    public float MinFollowDistance;

    /// <summary>
    /// Активная дистанци
    /// </summary>
    public float ActiveDistance;

    public override void Start()
    {
        base.Start();

        StartCoroutine("CustomRegister");
    }

    /// <summary>
    /// Кастомная инициализация
    /// </summary>
    /// <returns>The register.</returns>
    IEnumerator CustomRegister()
    {
        while(PlayerController.Instance == null) {yield return null;}

        FollowTarget = PlayerController.CachedTransform;
    }

    /// <summary>
    /// Получение урона
    /// </summary>
    /// <returns>true</returns>
    /// <c>false</c>
    /// <param name="_damage">Damage.</param>
    /// <param name="_power">Power.</param>
    /// <param name="_direction">Direction.</param>
    protected override bool _receiveDamage(int _damage, float _power, Vector3 _direction)
    {
        return false;
    }

    /// <summary>
    /// Обновление
    /// </summary>
    public override void Update()
    {
        //Проверка
        if (IsDead || IsPause || !IsInitialized)
            return;

        //Обновить ИИ
        switch (State)
        {
            case E_AIState.PATROLL:
                //PatrollController.UpdateState(ref State);
                UpdateFollow();
                break;
            case E_AIState.DETECT:
                //DetectController.UpdateState(ref State);
                break;
            case E_AIState.ATTACK:
                //AttackController.UpdateState(ref State);
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
    /// Обновление следования
    /// </summary>
    public void UpdateFollow()
    {
        if(FollowTarget == null) return;

        ActiveDistance = Vector3.Distance(FollowTarget.position, Root.position);

        if(ActiveDistance > FollowDistance)
            MovingController.SetTarget(FollowTarget.position);
        else if(ActiveDistance < MinFollowDistance)
            MovingController.SetTarget((Root.position - FollowTarget.position).normalized * 5.0f + Root.position);
        else if(MovingController.IsMoving && ActiveDistance < MovingController.DistanceToStop)
            MovingController.Stop();
    }
}
