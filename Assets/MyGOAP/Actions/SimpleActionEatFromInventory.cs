using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SimpleActionEatFromInventory<Settings> : SimpleAction<Settings> where Settings : SimpleActionSettings, new(){

    public DBItemCategory foodCategory;

    protected override void InitializePreconditionsAndEffects(ref ReGoapState staticEffects, ref List<IWorldState> parametrizedEffects, ref ReGoapState staticPreconditions){
        staticPreconditions.Set(WorldStateHasItemCategory.GetStateForItem( foodCategory ), true);
        staticEffects.Set(WorldStateHasItemCategory.GetStateForItem( foodCategory ), false);
        parametrizedEffects.Add(WorldStates.STATE_FLOAT_HUNGER);
    }

    protected override IEnumerator<SimpleActionExecutionControlElements> Execute(Settings settings, Action fail){

        yield break;
    }

    protected override ReGoapState GetPreconditionsFromGoal(ReGoapState goalState, Settings settings){
        ReGoapState returnState = new ReGoapState();
        returnState.Set( WorldStates.STATE_FLOAT_HUNGER, Mathf.Max( goalState.Get( WorldStates.STATE_FLOAT_HUNGER) - RPGHunger.GetSingleItemFoodLevel() ) );
        return returnState;
    }

    
}

[CreateAssetMenu(menuName = "GOAP/SimpleActions/EatFromInventory")]
public class SimpleActionEatFromInventory : SimpleActionEatFromInventory<SimpleActionSettings> { }
