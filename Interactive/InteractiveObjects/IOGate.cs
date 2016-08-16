using UnityEngine;
using System.Collections;

public class IOGate : IInteractiveObject
{
    /// <summary>
    /// Твин поворота
    /// </summary>
    public TweenRotation Tween;

    /// <summary>
    /// Открыты ли ворота
    /// </summary>
    public bool IsOpen;

    public override void OnTouchDownSuccess()
    {
        base.OnTouchDownSuccess();

        //Првоерка ворот
        if(IsOpen)
        {
            IsOpen = false;
            Tween.PlayReverse();
        }
        else
        {
            IsOpen = true;
            Tween.PlayForward();
        }
    }
}
