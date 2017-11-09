using System;
using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/Transitions/TransitionEatItem")]
public class TransitionFactoryEatItem : StateMachineTransitionFactory<ActionSettingsEatFromInventory,TransitionEatItem>
{
    public override bool IsInterruptable() {
        return true;
    }

    public override TransitionEatItem MakeTransition(ActionSettingsEatFromInventory init, Action<StateMachineTransition> OnDone, Action<StateMachineTransition> OnFailed)
    {
        return new TransitionEatItem( Layer, OnDone, OnFailed);
    }
}

public class TransitionEatItem : StateMachineTransition{

    public TransitionEatItem(StateMachineLayer layer, Action<StateMachineTransition> OnDone, Action<StateMachineTransition> OnFailed) : base(layer, OnDone, OnFailed){
    }

    public override bool Update(){

        var inventory = agent.GetComponent<Inventory>();

        string pickedItem = null;
        foreach(var item in inventory.items) {
            if (item.StartsWith("food"))
                pickedItem = item;
        }
        if (pickedItem != null)
            inventory.items.Remove(pickedItem);
        agent.GetComponent<RPGHunger>().EatItem(pickedItem);

        return true;
    }
}