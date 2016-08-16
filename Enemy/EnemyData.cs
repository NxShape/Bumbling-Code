using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    /// <summary>
    /// Название врага
    /// </summary>
    public string Name;

    /// <summary>
    /// Описание врага
    /// </summary>
    public string Description;

	/// <summary>
	/// Пулл
	/// </summary>
	public FastPool Pool;

    /// <summary>
    /// ИД врага
    /// </summary>
    public int ID;

    /// <summary>
    /// Уровень
    /// </summary>
    public int Level;

    /// <summary>
    /// Базовое здоровье
    /// </summary>
    public int Health;

    /// <summary>
    /// Базовый урон
    /// </summary>
    public int Damage;

    /// <summary>
    /// Опыт за убиство
    /// </summary>
    public int Expirience;

    /// <summary>
    /// Сила для срабатывания сильного откидывания
    /// </summary>
    public float StrongReceivedDamageParameter;

    /// <summary>
    /// Изменение жизней от уровня
    /// </summary>
    public AnimationCurve HealthCurve;

    /// <summary>
    /// Изменение урона от уровня
    /// </summary>
    public AnimationCurve DamageCurve;

    /// <summary>
    /// Изменение опыта от уровня
    /// </summary>
    public AnimationCurve ExpirienceCurve;

    /// <summary>
    /// Получить игровую модель с изменеными параметрами под героя
    /// </summary>
    /// <returns></returns>
    public EnemyBase GetGameModel()
    {
        //Создать модель
        //GameObject _obj = GameObject.Instantiate((GameObject)Resources.Load("Enemies/" + ID + "_" + Name));
		Pool = FastPoolManager.GetPool((GameObject)Resources.Load("Enemies/" + ID + "_" + Name));
		GameObject _obj = Pool.FastInstantiate();
        _obj.name += Random.Range(0, 99999);
        _obj.transform.parent = EnemyDatabase.CachedTransform;

        //Изменить параметры под уровень игрока
        EnemyBase _enemy = _obj.GetComponent<EnemyBase>();
        _enemy.enabled = true;
        _enemy.Visible.Using = true;
        Level = Defines.Level;
        _enemy.Data = this;

        //Вернуть
        return _enemy;
    }
}
