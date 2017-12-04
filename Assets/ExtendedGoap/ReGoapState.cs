using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ReGoapState : ICloneable, IEnumerable<IWorldState>, IEnumerable<KeyValuePair<IWorldState, object>> {
    // can change to object
    private volatile Dictionary<IWorldState, object> values;

    public ReGoapState(ReGoapState old)
    {
        lock (old.values)
            values = new Dictionary<IWorldState, object>(old.values);
    }

    public ReGoapState()
    {
        values = new Dictionary<IWorldState, object>();
    }

    /// <summary>
    /// Adds two extended states together. returns new extended states. respects WorldStateLogic
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static ReGoapState operator +(ReGoapState a, ReGoapState b)
    {
        ReGoapState result;
        lock (a.values)
        {
            result = new ReGoapState(a);
        }
        lock (b.values)
        {
            foreach(var pair in b.values) {
                if(a.values.ContainsKey( pair.Key )) { //value is contained in both a and b
                    result.values[pair.Key] = pair.Key.logic.Add(a.values[pair.Key], b.values[pair.Key]); // use this key's logic to combine the two values
                } else result.values[pair.Key] = pair.Value; //value was only in b
            }
            return result;
        }
    }

    public int Count
    {
        get { return values.Count; }
    }

    /// <summary>
    /// Tests for conflict with action that has given preconditions and effects. A conflict arises when effect sets an incompatible value, OR precondition sets an incompatible value and effect doesn't change it.
    /// If there is no conflict, then "this.Difference(effect) + precond" returns a valid state
    /// </summary>
    /// <param name="precond"></param>
    /// <param name="effects"></param>
    /// <returns></returns>
    public bool HasConflict(ReGoapState precond, ReGoapState effects) {

        //check effects
        foreach (var key in values.Keys) {
            if(effects.HasKey( key )) {
                if(key.logic.IsConflict( values[key], effects.values[key] ))
                    return true;
            } else if(precond.HasKey( key )) { //only check precond, if this value was not changed 
                if(key.logic.IsConflict( values[key], precond.values[key] ))
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Calculates difference between this and another state. 
    /// </summary>
    /// <param name="subtractor"></param>
    /// <returns></returns>
    public ReGoapState Difference(ReGoapState subtractor) {
        return CalculateDifference( subtractor, false );
    }

    public ReGoapState TryDifference( ReGoapState subtractor ) {
        return CalculateDifference( subtractor, true );
    }

    private ReGoapState CalculateDifference( ReGoapState subtractor, bool ignoreFailuje ) {
        ReGoapState newState = new ReGoapState();
        lock(values) lock(subtractor.values) {
                foreach(var key in values.Keys) {
                    if(subtractor.HasKey( key )) { //value exists
                        var difference = key.logic.Difference( values[key], subtractor.values[key], ignoreFailuje );
                        if(difference != null)
                            newState.values.Add( key, difference );
                    } else if(subtractor is IWorldStateDefaultable) { //value doesn't exists BUT subtractor has default value
                        var difference = key.logic.Difference( values[key], (subtractor as IWorldStateDefaultable).GetDefaultValue(), ignoreFailuje );
                        if(difference != null)
                            newState.values.Add( key, difference );
                    } else //value doesn't exist - don't subtract
                        newState.values.Add( key, values[key] );
                }
            }
        return newState;
    }
    /// <summary>
    /// Tests if this state has at least one value that fulfills goal's variable
    /// </summary>
    /// <param name="goal"></param>
    /// <returns></returns>
    public bool DoesFullfillGoal(ReGoapState goal)
    {
        lock (values) lock (goal.values)
        {
            foreach ( var pair in goal.values ){
                    if( goal.HasKey( pair.Key ) )
                        if( pair.Key.logic.Difference( goal.values[pair.Key], pair.Value, true ) == null ) //if variable gets canceled out, it means that it fulfilled goal
                            return true;
            }
            return false;
        }
    }

    public object Clone()
    {
        var clone = new ReGoapState(this);
        return clone;
    }

    public override string ToString()
    {
        lock (values)
        {
            var result = "";
            foreach (var pair in values)
                result += string.Format("'{0}': {1}, ", pair.Key.name, pair.Value);
            return result;
        }
    }

    public T Get<T>(WorldState<T> key)
    {
        lock (values)
        {
            if (!values.ContainsKey(key))
                return default(T);
            return (T)values[key];
        }
    }

    public void Set<T>( WorldState<T> key, T value)
    {
        lock (values)
        {
            values[key] = value;
        }
    }

    public void Remove<T>( WorldState<T> key )
    {
        lock (values)
        {
            values.Remove(key);
        }
    }

    public void SetFrom( IWorldState state, ReGoapState otherState ) {
        values[state] = otherState.values[state];
    }

    public bool IsEmpty() {
        return values.Count == 0;
    }

    public bool HasKey( IWorldState key )
    {
        lock (values)
            return values.ContainsKey(key);
    }

    public void Clear()
    {
        values.Clear();
    }

    public IEnumerator<IWorldState> GetEnumerator() {
        return values.Keys.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return values.Keys.GetEnumerator();
    }

    IEnumerator<KeyValuePair<IWorldState, object>> IEnumerable<KeyValuePair<IWorldState, object>>.GetEnumerator() {
        return ((IEnumerable<KeyValuePair<IWorldState, object>>)values).GetEnumerator();
    }
}