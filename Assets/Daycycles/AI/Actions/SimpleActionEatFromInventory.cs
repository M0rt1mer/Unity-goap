using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleActionEatFromInventory<Settings> : SimpleAction<Settings> where Settings : SimpleActionSettings, new(){

    public DBItemCategory foodCategory;

    protected override void InitializePreconditionsAndEffects( BGoapState staticEffects, ref BGoapState parametrizedEffects,  BGoapState staticPreconditions){
        parametrizedEffects.Set( WorldStates.STATE_FLOAT_SATURATION, 0 ); //we know nothing about current saturation, so we can't promise anything
        parametrizedEffects.Set( WorldStateMinItemCategory.GetStateForItem( foodCategory ), 0 ); //probably not required, set default to 0
    }

    protected override IEnumerator<SimpleActionExecutionControlElements> Execute(Settings settings, Action fail){

        yield break;
    }

    protected override BGoapState GetPreconditionsFromGoal(BGoapState goalState, Settings settings){
        BGoapState returnState = new BGoapState();
        returnState.Set( WorldStates.STATE_FLOAT_SATURATION, Mathf.Max( goalState.Get( WorldStates.STATE_FLOAT_SATURATION ) - RPGHunger.GetSingleItemFoodLevel(), 0 ) ); //require less saturation - will be increased
        returnState.Set( WorldStateMinItemCategory.GetStateForItem( foodCategory ), goalState.Get( WorldStateMinItemCategory.GetStateForItem( foodCategory ) ) + 1 ); //require one more food item
        return returnState;
    }
   
}

[CreateAssetMenu(menuName = "GOAP/SimpleActions/EatFromInventory")]
public class SimpleActionEatFromInventory : SimpleActionEatFromInventory<SimpleActionSettings> { }
