using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GOAP/PrototypeAction/PickItemRight")]
class PrototypeActionPickItemRight : PrototypeAction
{

    StateVarParameter<DBItem> itemTypeVariable;
    StateVarParameter<InGameItem> itemVariable;
    StateVarParameter<Vector3> locationVariable;

    protected override void Configure()
    {
        itemTypeVariable = new StateVarParameter<DBItem>();
        AddParametrizedEffect(WorldStates.STATE_HAND_RIGHT, itemTypeVariable);
        itemVariable = new StateVarParameter<InGameItem>();
        AddParametrizedEffect(WorldStates.STATE_HAND_RIGHT_ITEM, itemVariable);

        locationVariable = new StateVarParameter<Vector3>();
        AddParametrizedPrecondition(WorldStates.STATE_POSITION, locationVariable);
        AddStaticPrecondition(WorldStates.STATE_HAND_RIGHT_ITEM, null);
    }

    protected override IEnumerator<SimpleActionExecutionControlElements> Execute(PrototypeActionSettings settings, Action fail)
    {
        Debug.Log("picking item");
        //settings.
        GoapAgent agent = settings.agent as GoapAgent;
        Debug.Log(settings.effects.Get(WorldStates.STATE_HAND_RIGHT_ITEM));
        Debug.Log(agent.GetComponent<RPGHands>());
        agent.GetComponent<RPGHands>().PickItemRight( settings.effects.Get( WorldStates.STATE_HAND_RIGHT_ITEM ) );
        yield break;
    }

    protected override IEnumerable<BGoapState> GetPossibleEffectSets(BGoapState lockInValues, IReGoapAgent goapAgent)
    {
        if (lockInValues.HasKey(itemVariable) && lockInValues.Get(itemVariable) != null) //specific item locked in, fill in it's values
        {
            BGoapState assignment = new BGoapState();
            assignment.Set(itemTypeVariable, lockInValues.Get(itemVariable).sourceItem);
            assignment.Set(locationVariable, lockInValues.Get(itemVariable).positionCache);
            yield return assignment;
        }
        else if (lockInValues.HasKey(itemTypeVariable)) { //item type locked in, fill in all possible items

            DBItem requiredItemType = lockInValues.Get(itemTypeVariable);
            foreach (InGameItem item in (goapAgent.GetMemory() as GoapMemory).GetAvailableItemList() ) {
                if (item.sourceItem == requiredItemType)
                {
                    BGoapState assignment = new BGoapState();
                    assignment.Set(itemVariable, item);
                    assignment.Set(locationVariable, item.positionCache);
                    yield return assignment;
                }
            }

        }
        yield break;
    }
}

