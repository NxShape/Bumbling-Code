using UnityEngine;
using System.Collections;

public class IOAttraction : IInteractiveObject
{
    /// <summary>
    /// Контролирууемый аттракцион
    /// </summary>
    public AttractionBase ControllingAttraction;
    
    public override void OnTouchDownSuccess()
    {
        if(!ControllingAttraction.IsLaunched)
            ControllingAttraction.LaunchAttraction();
        else
            ControllingAttraction.ShowStatistics();
    }
}
