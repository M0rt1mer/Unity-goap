using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GOAP/PrototypeAction/SweepGround")]
class PrototypeActionSweepGround : PrototypeAction
{

    public DBItem broomClass;
    StateVarParameter<Vector3> locationVariable;
    StateVarParameter<SmartObject> groundSpot;

    protected override void Configure()
    {

        locationVariable = new StateVarParameter<Vector3>();
        groundSpot = new StateVarParameter<SmartObject>();

        AddParametrizedPrecondition(WorldStates.STATE_POSITION, locationVariable);
        AddStaticPrecondition(WorldStates.STATE_HAND_RIGHT, broomClass);
        AddStaticGenericEffect(WorldStates.GROUND_SWEEPED, groundSpot, true);
    }

    protected override IEnumerator<SimpleActionExecutionControlElements> Execute(PrototypeActionSettings settings, Action fail)
    {
        Debug.Log("sweeping ground");
        (settings.genericAssignments.Get(groundSpot) as RPGSOGround).isStateOn = true;
        yield break;
    }

    protected override IEnumerable<BGoapState> GetPossibleVariableCombinations(BGoapState lockInValues, IReGoapAgent goapAgent, BGoapState genericAssignments)
    {
        BGoapState assignment = new BGoapState();
        assignment.Set(locationVariable, genericAssignments.Get(groundSpot).positionCache);
        yield return assignment;
    }

    protected override IEnumerable<BGoapState> GetPossibleGenericVariableCombinations(IReGoapAgent goapAgent)
    {
        foreach (SmartObject so in (goapAgent.GetMemory() as GoapMemory).GetAvailableSoList())
        {
            if (so is RPGSOGround)
            {
                BGoapState resultState = new BGoapState();
                resultState.Set(groundSpot, so);
                yield return resultState;
            }
        }
    }
}

