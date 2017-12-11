using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldStates {

    public static StateVarKey<Vector3> STATE_POSITION = new StateVarKey<Vector3>( "position" );// = "isAtPosition";
    public static StateVarKey<float> STATE_FLOAT_SATURATION = new StateVarKeyComparable<float,StateVariableLogicAtLeast>( "saturation" ); // "at least X saturation"
    public static StateVarKey<string> STATE_HAND_LEFT = new StateVarKey<string>( "hand_left" );
    public static StateVarKey<string> STATE_HAND_RIGHT = new StateVarKey<string>( "hand_right" );

}

public class WorldStateMinItem : StateVarKeyComparableDefaultable<int,StateVariableLogicAtLeast> {
    public readonly DBItem item;
    public WorldStateMinItem( DBItem item ) : base( "hasItem:" + item.name, 0 ) {
        this.item = item;
        statesList[item] = this;
    }

    public static Dictionary<DBItem, WorldStateMinItem> statesList = new Dictionary<DBItem, WorldStateMinItem>();

    public static WorldStateMinItem GetStateForItem( DBItem item ) {
        if( !statesList.ContainsKey( item ) ) {
            statesList.Add( item, new WorldStateMinItem( item ) );
        }
        return statesList[item];
    }

}

public class WorldStateMinItemCategory : StateVarKeyComparableDefaultable<int,StateVariableLogicAtLeast> {
    public readonly DBItemCategory category;
    public WorldStateMinItemCategory( DBItemCategory category ) : base( "hasItemCat:" + category.name, 0 ) {
        this.category = category;
    }

    public static Dictionary<DBItemCategory, WorldStateMinItemCategory> statesList = new Dictionary<DBItemCategory, WorldStateMinItemCategory>();

    public static WorldStateMinItemCategory GetStateForItem( DBItemCategory item ) {
        if(!statesList.ContainsKey( item )) {
            statesList.Add( item, new WorldStateMinItemCategory( item ) );
        }
        return statesList[item];
    }
}


#region IWorldState
/// <summary>
/// IWorldState is an observable state of the world, which NPC can use to reason about their actions.
/// This interface is for internal use, for defining actual world states use WorldState<>
/// </summary>
public interface IStateVarKey{

    IStateVariableLogic logic { get; }
    string name { get; }
    Type GetValueType();

}

public interface IStateVarKeyDefaultable {

    object GetDefaultValue();

}

/// <summary>
/// Marks a IWorldState 
/// </summary>
/// <typeparam name="InnerType"></typeparam>
public interface IStateVarKeyDefaultable<InnerType> : IStateVarKey, IStateVarKeyDefaultable {

    InnerType GetDefaultValueTyped();

}

/// <summary>
/// WorldState is an observable state of the world, which NPC can use to reason about their actions.
/// WorldState has a InnerType - all observations must be of this type (this is enforced by ReGoapStateExtended)
/// Used for WorldStates with simple logic - it doesn't need to be comparable (usually EQUAL logic)
/// </summary>
/// <typeparam name="InnerType"></typeparam>
public class StateVarKey<InnerType> : IStateVarKey {

    public virtual IStateVariableLogic logic { protected set; get; }
    public string name { protected set; get; }

    public StateVarKey( string name ){
        this.logic = StateVariableLogicFactory.GetWorldStateLogic<StateVariableLogicEquals>();
        this.name = name;
    }

    public override string ToString() {
        return string.Format( "WorldState[{0}]", typeof(InnerType).Name );
    }

    public Type GetValueType() {
        return typeof( InnerType );
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

    public StateVarKeyComparable( string name ) : base( name ){
        this.logic = StateVariableLogicFactory.GetWorldStateLogic<Logic>();
    }

    public override string ToString() {
        return string.Format( "WorldState{1}[{0}]", typeof(InnerType).Name, logic.ToString() );
    }

}

public class StateVarKeyComparableDefaultable<InnerType, Logic> : StateVarKeyComparable<InnerType, Logic>, IStateVarKeyDefaultable<InnerType>
            where InnerType : IComparable
            where Logic : StateVariableLogic<IComparable>, new() {

    InnerType defaultValue;

    public StateVarKeyComparableDefaultable( string name, InnerType defaultValue ) : base( name ){
        this.logic = StateVariableLogicFactory.GetWorldStateLogic<Logic>();
        this.defaultValue = defaultValue;
    }

    public object GetDefaultValue() {
        return defaultValue;
    }

    public InnerType GetDefaultValueTyped() {
        return defaultValue;
    }
}

#endregion
