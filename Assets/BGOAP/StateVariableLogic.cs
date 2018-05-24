using System;
using System.Collections.Generic;


/// <summary>
/// WorldStateLogic defines operations, that are performed on values of this WorldState. It allows world state to represent bouned values, like "has at least 4 food items", instead of exact values
/// When defining custom logic, inherit from WorldStateLogic<>, as that is used in WorldState. IWorldStateLogic is for inner use in ReGoapStateExtended, to eliminate excessive use of generics
/// </summary>
public interface IStateVariableLogic {

    /// <summary>
    /// Adds two actions together. If IsConflict is false, this must complete without exceptions
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
    bool IsConflict( object goal, object effect );
    /// <summary>
    /// Returns true, if what satisfies whom
    /// </summary>
    /// <param name="what"></param>
    /// <param name="whom"></param>
    /// <returns>Null if the variable should</returns>
    bool Satisfies( object what, object whom );

}

/// <summary>
/// Creates singletons for all implementations of IWorldStateLogic. This allows WorldState<,> to define logic as generic, and check the logic's generic type constraints
/// </summary>
public abstract class StateVariableLogicFactory {
    private static Dictionary<Type, IStateVariableLogic> singletons = new Dictionary<Type, IStateVariableLogic>();
    public static T GetWorldStateLogic<T>() where T : class, IStateVariableLogic, new() {
        if(!singletons.ContainsKey( typeof( T ) ))
            singletons.Add( typeof( T ), new T() );
        return singletons[typeof( T )] as T;
    }
}

/// <summary>
/// Main implementation of IWorldStateLogic, defines operations, that are performed on values of this WorldState. It allows world state to represent bouned values, like "has at least 4 food items", instead of exact values
/// It's methods are used inside ReGoapStateExtended, to determine validity, and effects, of actions
/// </summary>
/// <typeparam name="DataType"></typeparam>
public abstract class StateVariableLogic<DataType> : IStateVariableLogic {

    public abstract object Add( object a, object b );
    public abstract bool IsConflict( object goal, object effect );
    public abstract bool Satisfies( object what, object whom );

}

/// <summary>
/// Basic WorldStateLogic, indicates that values need to be matched exactly. Adding two states with different values will throw ArgumentException
/// </summary>
public class StateVariableLogicEquals : StateVariableLogic<object> {

    /// <summary>
    /// If both are equal, returns one of them, otherwise throws exception (as in that case InConflict is true, therefore this should not be computed)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public override object Add( object a, object b ) {
        if( IsConflict(a,b) )
            throw new ArgumentException( "Trying to add conflicting states" );
        return a;
    }

    /// <summary>
    /// returns true if values are different
    /// </summary>
    /// <param name="goal"></param>
    /// <param name="effect"></param>
    /// <returns></returns>
    public override bool IsConflict( object goal, object effect ) {
        if(goal == null && effect == null)
            return false;
        if(goal == null)
            return false;
        if(goal != effect && !goal.Equals( effect )) //if at least one of them equals, it's fine
            return true;
        return false;
    }

    public override bool Satisfies( object what, object whom ) {
        return ( what==null && whom==null ) || (what != null && whom != null && (what == whom || what.Equals( whom ) ) );
    }
}

/// <summary>
/// This state's value represents "at least" boud, e.g. "has at least X items"
/// </summary>
public class StateVariableLogicAtLeast : StateVariableLogic<IComparable> {

    public override object Add( object a, object b ) { //we know that b>=a, because there is no conflict
        return b;
    }

    public override bool IsConflict( object a, object b ) {
        return ((IComparable)a).CompareTo(b) < 0;
    }

    public override bool Satisfies( object what, object whom ) {
        return (what != null && whom != null && ((IComparable)what).CompareTo( whom ) >= 0 ); //satisfies, IF bigger than or equal to
    }

}

/// <summary>
/// This states value represents "at most" bound, e.g. "has at most 5 damage"
/// </summary>
public class StateVariableLogicAtMost : StateVariableLogic<IComparable> {
    public override object Add( object a, object b ) {
        return b;
    }

    public override bool IsConflict( object goal, object effect ) {
        return ((IComparable)goal).CompareTo(effect) > 0;
    }

    public override bool Satisfies( object what, object whom ) {
        return (what != null && whom != null && ((IComparable)what).CompareTo( whom ) <= 0);
    }
}