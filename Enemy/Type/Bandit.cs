using UnityEngine;
using System.Collections;

public class Bandit : EnemyBase
{
	/// <summary>
	/// Активная атака
	/// </summary>
	public int ActiveAttack;

	/// <summary>
	/// Количество возможных ататк
	/// </summary>
	public int AttackCount;

	/// <summary>
	/// Запуск анимации атаки
	/// </summary>
	public override void StartAttack ()
	{
		IsAttackStarted = true;
		MovingController.AnimatorController.SetBool("Attack_" + ActiveAttack, true);
	}

	/// <summary>
	/// Событие из анимации запуска атаки
	/// </summary>
	public override void OnStartAttack ()
	{
		MovingController.AnimatorController.SetBool("Attack_" + ActiveAttack, false);
	}

	/// <summary>
	/// Событие атаки из анимации
	/// </summary>
	public override void OnAttack ()
	{
		//Проверка цели
		if(Detected == null) 
		{
			OnEnemyLose();
			return;
		}
		
		//Првоерка расстояния
		if(AttackController.CanAttack())
		{
			//Нанести урон
			if(Detected.GetComponent<IReceivedDamage>().OnReceiveDamage(Damage, 0.0f, Vector3.zero))
				OnEnemyLose();
			else
			{
				ActiveAttack++;

				if(ActiveAttack >= AttackCount)
					ActiveAttack = 0;
			}
		}
		else
			ActiveAttack = 0;
	}

	/// <summary>
	/// Событие окончания атаки
	/// </summary>
	public override void OnEndAttack ()
	{
		if(ActiveAttack != 0 && AttackController.CanAttack())
			StartAttack();
		else
		{
			//Сбросить анимацию
			MovingController.AnimatorController.SetTrigger("EndAttack");

			base.OnEndAttack();
		}
	}

	/// <summary>
	/// Восстановление после удара
	/// </summary>
	public override void OnResetAfterDamage ()
	{
		base.OnResetAfterDamage();

		//Сброс активной
		ActiveAttack = 0;

		//Сбросить атаки
		for(int i = 0; i < AttackCount; i++)
			MovingController.AnimatorController.SetBool("Attack_" + i, false);
	}

	/// <summary>
	/// Событие при потери врага
	/// </summary>
	public override void OnEnemyLose ()
	{
		base.OnEnemyLose ();

		ActiveAttack = 0;
	}
}
