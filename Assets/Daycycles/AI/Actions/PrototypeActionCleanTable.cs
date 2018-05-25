using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GOAP/PrototypeAction/CleanTable")]
class PrototypeActionCleanTable : PrototypeAction
{

    StateVarParameter<Vector3> locationVariable;
    StateVarParameter<SmartObject> table;

    protected override void Configure()
    {

        locationVariable = new StateVarParameter<Vector3>();
        table = new StateVarParameter<SmartObject>();

        AddParametrizedPrecondition(WorldStates.STATE_POSITION, locationVariable);
        AddStaticPrecondition(WorldStates.STATE_HAND_RIGHT_ITEM, null);
        AddStaticGenericEffect(WorldStates.TABLE_SWEEPED, table, true);
    }

    protected override IEnumerator<SimpleActionExecutionControlElements> Execute(PrototypeActionSettings settings, Action fail)
    {
        Debug.Log("sweeping table");
        (settings.genericAssignments.Get(table) as RPGSOTable).isStateOn = true;
        yield break;
    }

    protected override IEnumerable<BGoapState> GetPossibleVariableCombinations(BGoapState lockInValues, IReGoapAgent goapAgent, BGoapState genericAssignments)
    {
        BGoapState assignment = new BGoapState();
        assignment.Set(locationVariable, genericAssignments.Get(table).positionCache );
        yield return assignment;
    }

    protected override IEnumerable<BGoapState> GetPossibleGenericVariableCombinations(IReGoapAgent goapAgent)
    {
        foreach (SmartObject so in (goapAgent.GetMemory() as GoapMemory).GetAvailableSoList())
        {
            if (so is RPGSOTable) {
                BGoapState resultState = new BGoapState();
                resultState.Set(table, so);
                yield return resultState;
            }
        }
    }
}

