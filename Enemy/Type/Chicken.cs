using UnityEngine;
using System.Collections;

public class Chicken : EnemyBase
{
    /// <summary>
    /// Получил пинок от игрока
    /// </summary>
    /// <param name="_direction">Direction.</param>
    public override void OnPlayerPull(Vector3 _direction)
    {
        MovingController.Stop();
        MovingController.AnimatorController.SetTrigger("Hit");
    }

    /// <summary>
    /// Receives the damage.
    /// </summary>
    /// <returns><c>true</c>, if damage was received, <c>false</c> otherwise.</returns>
    /// <param name="_damage">Damage.</param>
    /// <param name="_power">Power.</param>
    /// <param name="_direction">Direction.</param>
    protected override bool _receiveDamage(int _damage, float _power, Vector3 _direction)
    {
        OnPlayerPull(_direction);

        return false;
    }
}
