using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldStates {

    public static WorldState<Vector3> STATE_POSITION = new WorldState<Vector3>( "position" );// = "isAtPosition";
    public static WorldState<float> STATE_FLOAT_SATURATION = new WorldStateComparable<float,WorldStateLogicAtLeast>( "saturation" ); // "at least X saturation"
    public static WorldState<string> STATE_HAND_LEFT = new WorldState<string>( "hand_left" );
    public static WorldState<string> STATE_HAND_RIGHT = new WorldState<string>( "hand_right" );

}

public class WorldStateMinItem : WorldStateComparableDefaultable<int,WorldStateLogicAtLeast> {
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

public class WorldStateMinItemCategory : WorldStateComparableDefaultable<int,WorldStateLogicAtLeast> {
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
public interface IWorldState{

    IWorldStateLogic logic { get; }
    string name { get; }
    Type GetValueType();

}

public interface IWorldStateDefaultable {

    object GetDefaultValue();

}

/// <summary>
/// Marks a IWorldState 
/// </summary>
/// <typeparam name="InnerType"></typeparam>
public interface IWorldStateDefaultable<InnerType> : IWorldState, IWorldStateDefaultable {

    InnerType GetDefaultValueTyped();

}

/// <summary>
/// WorldState is an observable state of the world, which NPC can use to reason about their actions.
/// WorldState has a InnerType - all observations must be of this type (this is enforced by ReGoapStateExtended)
/// Used for WorldStates with simple logic - it doesn't need to be comparable (usually EQUAL logic)
/// </summary>
/// <typeparam name="InnerType"></typeparam>
public class WorldState<InnerType> : IWorldState {

    public virtual IWorldStateLogic logic { protected set; get; }
    public string name { protected set; get; }

    public WorldState( string name ){
        this.logic = WorldStateLogicFactory.GetWorldStateLogic<WorldStateLogicEquals>();
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
public class WorldStateComparable<InnerType,Logic> : WorldState<InnerType> 
            where InnerType : IComparable 
            where Logic : WorldStateLogic<IComparable>,new() {

    public override IWorldStateLogic logic { protected set; get; }

    public WorldStateComparable( string name ) : base( name ){
        this.logic = WorldStateLogicFactory.GetWorldStateLogic<Logic>();
    }

    public override string ToString() {
        return string.Format( "WorldState{1}[{0}]", typeof(InnerType).Name, logic.ToString() );
    }

}

public class WorldStateComparableDefaultable<InnerType, Logic> : WorldStateComparable<InnerType, Logic>, IWorldStateDefaultable<InnerType>
            where InnerType : IComparable
            where Logic : WorldStateLogic<IComparable>, new() {

    InnerType defaultValue;

    public WorldStateComparableDefaultable( string name, InnerType defaultValue ) : base( name ){
        this.logic = WorldStateLogicFactory.GetWorldStateLogic<Logic>();
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

#region IWorldStateLogic
/// <summary>
/// WorldStateLogic defines operations, that are performed on values of this WorldState. It allows world state to represent bouned values, like "has at least 4 food items", instead of exact values
/// When defining custom logic, inherit from WorldStateLogic<>, as that is used in WorldState. IWorldStateLogic is for inner use in ReGoapStateExtended, to eliminate excessive use of generics
/// </summary>
public interface IWorldStateLogic {

    /// <summary>
    /// Adds two actions together. If IsConflict is true, this must complete without exceptions
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    object Add( object a, object b );
    /// <summary>
    /// Indicates two values are in conflict, therefore action with given effects would invalidate desired goal (if executed as last action)
    /// </summary>
    /// <param name="goal"></param>
    /// <param name="effect"></param>
    /// <returns></returns>
    bool IsConflict(object goal, object effect);
    /// <summary>
    /// Difference between original object and new object. Returns null if there is no difference (e.g. from is goal, what is effects, return null when effect fulfills goal)
    /// </summary>
    /// <param name="from"></param>
    /// <param name="what"></param>
    /// <returns>Null if the variable should</returns>
    object Difference(object from, object what, bool ignoreInvalid);

}

/// <summary>
/// Creates singletons for all implementations of IWorldStateLogic. This allows WorldState<,> to define logic as generic, and check the logic's generic type constraints
/// </summary>
public abstract class WorldStateLogicFactory {
    private static Dictionary<Type, IWorldStateLogic> singletons = new Dictionary<Type, IWorldStateLogic>();
    public static T GetWorldStateLogic<T>() where T : class, IWorldStateLogic, new(){
        if (!singletons.ContainsKey(typeof(T)))
            singletons.Add(typeof(T), new T());
        return singletons[typeof(T)] as T;
    }
}

/// <summary>
/// Main implementation of IWorldStateLogic, defines operations, that are performed on values of this WorldState. It allows world state to represent bouned values, like "has at least 4 food items", instead of exact values
/// It's methods are used inside ReGoapStateExtended, to determine validity, and effects, of actions
/// </summary>
/// <typeparam name="DataType"></typeparam>
public abstract class WorldStateLogic<DataType> : IWorldStateLogic {

    public abstract object Add(object a, object b);
    public abstract bool IsConflict(object goal, object effect);
    public abstract object Difference(object from, object what, bool ignoreInvalid = false );
}

/// <summary>
/// Basic WorldStateLogic, indicates that values need to be matched exactly. Adding two states with different values will throw ArgumentException
/// </summary>
public class WorldStateLogicEquals : WorldStateLogic<object>
{
    public override object Add(object a, object b){
        throw new ArgumentException("Trying to add conflicting states");
    }

    /// <summary>
    /// returns true if values are different
    /// </summary>
    /// <param name="goal"></param>
    /// <param name="effect"></param>
    /// <returns></returns>
    public override bool IsConflict(object goal, object effect){
        if (goal == null && effect == null)
            return false;
        if(goal == null)
            return false;
        if(goal != effect && !goal.Equals( effect )) //if at least one of them equals, it's fine
            return true;
        return false;
    }

    public override object Difference(object from, object what, bool ignoreInvalid ) {
        if (what == null || from == null)
            return from;
        if (what == from || what.Equals(from)) //if subtracting excatly the same value, result is null
            return null;
        if(ignoreInvalid) //if ignore invalid value, just return "from"
            return from;
        throw new ArgumentException( "Difference between two EQUAL objects cannot be computed" );
    }
}

/// <summary>
/// This state's value represents "at least" boud, e.g. "has at least X items"
/// </summary>
public class WorldStateLogicAtLeast : WorldStateLogic<IComparable>
{
    public override object Add(object a, object b){
        return Utils.Max((IComparable)a, (IComparable)b);
    }

    public override bool IsConflict(object goal, object effect){
        return false;
        //return ((IComparable)goal).CompareTo(effect) > 0;
    }

    public override object Difference(object from, object what, bool ignoreInvalid ) {
        if (what == null || from == null)
            return from;
        if (((IComparable)from).CompareTo(what) <= 0) //if subtracting bigger or equal, result is null. Otherwise original value
            return null;
        return from;
    }
}

/// <summary>
/// This states value represents "at most" bound, e.g. "has at most 5 damage"
/// </summary>
public class WorldStateLogicAtMost : WorldStateLogic<IComparable>
{
    public override object Add(object a, object b)
    {
        return Utils.Min((IComparable)a, (IComparable)b);
    }

    public override object Difference(object from, object what, bool ignoreInvalid )
    {
        if (what == null || from == null)
            return from;
        if (((IComparable)from).CompareTo(what) >= 0) //if subtracting smaller or equal, result is null. Otherwise original value
            return null;
        return from;
    }

    public override bool IsConflict(object goal, object effect)
    {
        return false;
        //return ((IComparable)goal).CompareTo(effect) < 0;
    }
}
#endregion