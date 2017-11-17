using System;
using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/Transitions/TransitionEatBowl")]
public class TransitionFactoryEatBowl : StateMachineTransitionFactory<TransitionEatBowlInitializer, TransitionEatBowl>
{
    public override bool IsInterruptable() {
        return true;
    }
}

public class TransitionEatBowl : StateMachineTransition<TransitionEatBowlInitializer>{

    public TransitionEatBowl(TransitionEatItemInitializer initializer, StateMachineLayer layer, Action<StateMachineTransition<TransitionEatItemInitializer>> OnDone, Action<StateMachineTransition<TransitionEatItemInitializer>> OnFailed) : base(initializer, layer, OnDone, OnFailed)
    {
    }

    public override bool Update(){

        var hands = agent.GetComponent<RPGHands>();

        if (hands.leftHandItem == "equippable/bowl_filled")
        {
            hands.leftHandItem = "equippable/bowl_empty";
            agent.GetComponent<RPGHunger>().EatItem("equippable/bowl_filled");
        }
        else {
            OnFailed(this);
        }

        return true;
    }
}

public class TransitionEatBowlInitializer
{

}