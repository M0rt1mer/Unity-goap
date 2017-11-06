using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class SOStash : MonoBehaviour, SmartObjectBehaviorTemplate {

    Inventory inv;

    protected void Awake() {
        inv = GetComponent<Inventory>();
    }

    public IEnumerable<SmartObjectBehavior> GenerateAllBehaviors() {
        return inv.items.Select( item => (SmartObjectBehavior) new SoStashPossibleAction( item, inv ) );
    }

}

public class SoStashPossibleAction : SmartObjectBehavior {

    public string item;
    public Inventory inv;

    public SoStashPossibleAction( string item, Inventory inv ) {
        effects.Set( "hasItem:" + item, true );
        preconditions.Set( "isAtPosition", inv.transform );
        this.item = item;
        this.inv = inv;
    }

    public override bool PerformAction( GameObject agent ) {
        inv.items.Remove( item );
        agent.GetComponent<Inventory>().items.Add( item );
        return true;
    }

}
