using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region IWorldState

/// <summary>
/// IStateVarKey is an key for for BGoapState. Each Key/Value pair represent a statement about the NPC (e.g. "is at world position X"). 
/// </summary>
/// <typeparam name="ValueType">Type for values, which can be stored under this key. This is guaranteed only in BGoapState</typeparam>
public interface IStateVarKey<out ValueType>{

    IStateVariableLogic logic { get; }
    string name { get; }
    ValueType GetDefaultValue();
    float distance(object a, object b);

}

/// Used for WorldStates with simple logic - it doesn't need to be comparable (usually EQUAL logic)
/// </summary>
/// <typeparam name="ValueType"></typeparam>
public class StateVarKey<ValueType> : IStateVarKey<ValueType> {

    public virtual IStateVariableLogic logic { protected set; get; }
    public string name { protected set; get; }
    private ValueType defaultValue;
    protected Func<object, object, float> distanceFunc;

    public StateVarKey( string name, ValueType defaultValue ){
        this.logic = StateVariableLogicFactory.GetWorldStateLogic<StateVariableLogicEquals>();
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

    public float distance(object a, object b){
        if (distanceFunc == null)
            return 1;
        return distanceFunc(a, b);
    }
}

/// <summary>
/// Used for more advanced WorldStateLogics, that require IComparable value types
/// </summary>
/// <typeparam name="InnerType"></typeparam>
/// <typeparam name="Logic"></typeparam>
public class StateVarKeyComparable<InnerType,Logic> : StateVarKey<InnerType> 
            where InnerType : IComparable 
            where Logic : StateVariableLogic<IComparable>,new() {

    public override IStateVariableLogic logic { protected set; get; }

    public StateVarKeyComparable( string name, InnerType defaultValue ) : base( name, defaultValue ){
        this.logic = StateVariableLogicFactory.GetWorldStateLogic<Logic>();
    }

    public StateVarKeyComparable(string name, InnerType defaultValue, Func<object,object,float> distanceFnc ) : base(name, defaultValue){
        this.logic = StateVariableLogicFactory.GetWorldStateLogic<Logic>();
        this.distanceFunc = distanceFnc;
    }

    public override string ToString() {
        return string.Format( "WorldState{1}[{0}]", typeof(InnerType).Name, logic.ToString() );
    }

}

#endregion
