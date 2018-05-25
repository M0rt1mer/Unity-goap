using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GOAP/PrototypeAction/DropItemRight")]
class PrototypeActionDropItemRight : PrototypeAction
{

    protected override void Configure()
    {
        AddStaticEffect(WorldStates.STATE_HAND_RIGHT, null);
        AddStaticEffect(WorldStates.STATE_HAND_RIGHT_ITEM, null);
    }

    protected override IEnumerator<SimpleActionExecutionControlElements> Execute(PrototypeActionSettings settings, Action fail)
    {
        Debug.Log("picking item");
        //settings.
        GoapAgent agent = settings.agent as GoapAgent;
        agent.GetComponent<RPGHands>().DropItemRight();
        yield break;
    }

}

