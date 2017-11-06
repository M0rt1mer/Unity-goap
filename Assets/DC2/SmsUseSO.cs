using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmsUseSO : SmState {

    protected SmartObjectBehavior activeBehavior;
    protected Action onDone;
    protected Action onFailure;

    protected override void FixedUpdate() {
        if(this.IsActive())
            if(activeBehavior.PerformAction( gameObject )) {
                activeBehavior = null;
                onDone();
            }
    }

    public override void Init( StateMachine stateMachine ) {
        base.Init( stateMachine );
        var transistion = new SmTransistion( GetPriority(), Transition );
        var doneTransistion = new SmTransistion( GetPriority(), DoneTransition );
        stateMachine.GetComponent<SmsIdle>().Transistions.Add( transistion );
        Transistions.Add( doneTransistion );
    }

    public Type Transition( ISmState arg1 ) {
        if(activeBehavior != null)
            return typeof( SmsUseSO );
        return null;
    }

    public Type DoneTransition( ISmState arg1 ) {
        if(activeBehavior == null)
            return typeof( SmsUseSO );
        return null;
    }

    public void UseBehavior(  SmartObjectBehavior behavior, Action onDone, Action onFailure ) {
        activeBehavior = behavior;
        this.onDone = onDone;
        this.onFailure = onFailure;
    }

}
