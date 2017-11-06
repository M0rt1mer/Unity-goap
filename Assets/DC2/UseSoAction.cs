using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.VR;

[RequireComponent( typeof( SmsUseSO ) )]
public class UseSoAction : GoapAction {

    SmsUseSO sms;

    protected override void Awake() {
        base.Awake();

        sms = GetComponent<SmsUseSO>();
    }

    public override void Precalculations( IReGoapAgent goapAgent, ReGoapState goalState ) {
        ReGoapState difference = new ReGoapState();
        goalState.MissingDifference( goapAgent.GetMemory().GetWorldState(), ref difference );
        List<SmartObjectBehavior> behaviorList = new List<SmartObjectBehavior>();
        foreach(SmartObject SO in goapAgent.GetMemory().GetWorldState().Get<SmartObject[]>("soList") ) {
            foreach(SmartObjectBehaviorTemplate template in SO.GetTemplates()) {
                foreach(SmartObjectBehavior beh in template.GenerateAllBehaviors()) {
                    if(beh.effects.HasAny( goalState ) & !goalState.HasAnyConflict( beh.effects ) & !goalState.HasAnyConflict( beh.preconditions )) {
                        behaviorList.Add( beh );
                    }
                }
            }
        }
        // pick random for now, horrible solution
        settings = new UseSoSettings {
            behavior = behaviorList[UnityEngine.Random.Range( 0, behaviorList.Count )]
        };
        base.Precalculations( goapAgent, goalState );
    }

    public override void Run( IReGoapAction previous, IReGoapAction next, IReGoapActionSettings settings, ReGoapState goalState, Action<IReGoapAction> done, Action<IReGoapAction> fail ) {
        base.Run( previous, next, settings, goalState, done, fail );

        UseSoSettings soSettings = (UseSoSettings) settings;
        sms.UseBehavior( soSettings.behavior, delegate() { done( this ); }, delegate() { fail( this ); } );
    }

    public override ReGoapState GetEffects( ReGoapState goalState, IReGoapAction next = null ) {
        UseSoSettings soSettings = (UseSoSettings)settings;
        return soSettings.behavior.effects;
    }

    public override ReGoapState GetPreconditions( ReGoapState goalState, IReGoapAction next = null ) {
        UseSoSettings soSettings = (UseSoSettings)settings;
        return soSettings.behavior.preconditions;
    }

    public override float GetCost( ReGoapState goalState, IReGoapAction next = null ) {
        return base.GetCost( goalState, next ) + Cost;
    }

}

internal class UseSoSettings : IReGoapActionSettings {
    public SmartObjectBehavior behavior { get; set; }
}
