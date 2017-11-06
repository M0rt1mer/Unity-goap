using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SmartObjectBehaviorTemplate {

    IEnumerable<SmartObjectBehavior> GenerateAllBehaviors();

}

public abstract class SmartObjectBehavior {

    public ReGoapState preconditions;
    public ReGoapState effects;

    public SmartObjectBehavior() {
        preconditions = new ReGoapState();
        effects = new ReGoapState();
    }

    public abstract bool PerformAction( GameObject agent );

}
