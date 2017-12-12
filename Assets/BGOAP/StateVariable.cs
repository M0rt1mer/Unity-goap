using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region IWorldState

/// <summary>
/// IStateVarKey is an key for for BGoapState. Each Key/Value pair represent a statement about the NPC (e.g. "is at world position X"). 
/// </summary>
/// <typeparam name="ValueType">Type for values, which can be stored under this key. This is guaranteed only in BGoapState</typeparam>
public interface IStateVarKey<ValueType>{

    IStateVariableLogic<ValueType> logic { get; }
    string name { get; }
    ValueType GetDefaultValue();
    float distance(ValueType a, ValueType b );

}

/// Used for WorldStates with simple logic - it doesn't need to be comparable (usually EQUAL logic)
/// </summary>
/// <typeparam name="ValueType"></typeparam>
public class StateVarKey<ValueType> : IStateVarKey<ValueType> {

    public IStateVariableLogic<ValueType> logic { protected set; get; }
    public string name { protected set; get; }
    private ValueType defaultValue;
    Func<ValueType, ValueType, float> distanceFunc;

    public StateVarKey( string name, ValueType defaultValue ){
        this.logic = (IStateVariableLogic<ValueType>) StateVariableLogicFactory.GetWorldStateLogic<StateVariableLogicEquals>();
        this.name = name;
        this.defaultValue = defaultValue;
    }

    public override string ToString() {
        return string.Format( "WorldState[{0}]", typeof(ValueType).Name );
    }

    public Type GetValueType() {
        return typeof( ValueType );
    }

    ValueType IStateVarKey<ValueType>.GetDefaultValue(){
        return defaultValue;
    }

    public float distance(ValueType a, ValueType b){
        if (distanceFunc == null)
            return 1;
        return distanceFunc( a, b );
    }

}

/// <summary>
/// Used for more advanced WorldStateLogics, that require IComparable value types
/// </summary>
/// <typeparam name="ValueType"></typeparam>
/// <typeparam name="Logic"></typeparam>
public class StateVarKeyComparable<ValueType,Logic> : StateVarKey<ValueType> 
            where ValueType : IComparable 
            where Logic : class,IStateVariableLogic<IComparable>,new() {

    public StateVarKeyComparable( string name, ValueType defaultValue ) : base( name, defaultValue ){
        logic = (IStateVariableLogic<ValueType>) StateVariableLogicFactory.GetWorldStateLogic<Logic>();
    }

    public override string ToString() {
        return string.Format( "WorldState{1}[{0}]", typeof(ValueType).Name, logic.ToString() );
    }

}

#endregion
