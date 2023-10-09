// for PATK, MATK, MDEF, PDEF, Hit, Cirt. etc....
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public abstract class Stat : NetworkBehaviour
{
    public abstract int current { get; }

    //[Header("Events")]
    //public UnityEvent onEmpty;

    public override void OnStartServer()
    {
        
    }

    // get percentage
    //public float Percent() =>
    //    (current != 0 && max != 0) ? (float)current / (float)max : 0;
}
