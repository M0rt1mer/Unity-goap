﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.VR;

[RequireComponent(typeof(Inventory))]
public class ActionEatFromInventory : ActionExecuteTransition<ActionSettingsEatFromInventory,TransitionFactoryEatItem,TransitionEatItem,TransitionEatItemInitializer>
{

    public override void Precalculations(IReGoapAgent goapAgent, ReGoapState goalState)
    {
        effects.Clear();
        effects.Set( WorldStates.STATE_FLOAT_HUNGER, goalState.Get( WorldStates.STATE_FLOAT_HUNGER ) );
        effects.Set( WorldStateHasItemCategory.GetStateForItem("food"), false);
        preconditions.Clear();
        preconditions.Set( WorldStateHasItemCategory.GetStateForItem( "food" ), true);
        base.Precalculations(goapAgent, goalState);
    }

    public override float GetCost(ReGoapState goalState, IReGoapAction next = null)
    {
        return base.GetCost(goalState, next) + 1;
    }

}

public class ActionSettingsEatFromInventory : TransitionEatItemInitializer, IReGoapActionSettings {
}