using System;
using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/Transitions/TransitionEatItem")]
public class TransitionFactoryEatItem : StateMachineTransitionFactory<TransitionEatItemInitializer, TransitionEatItem>
{
    public override bool IsInterruptable() {
        return true;
    }
}

public class TransitionEatItem : StateMachineTransition<TransitionEatItemInitializer>{

    public TransitionEatItem(TransitionEatItemInitializer initializer, StateMachineLayer layer, Action<StateMachineTransition<TransitionEatItemInitializer>> OnDone, Action<StateMachineTransition<TransitionEatItemInitializer>> OnFailed) : base(initializer, layer, OnDone, OnFailed)
    {
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

public class TransitionEatItemInitializer {

}