﻿using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ReGoapStateExtended : ICloneable
{
    // can change to object
    private volatile Dictionary<IWorldState, object> values;

    public ReGoapStateExtended(ReGoapStateExtended old)
    {
        lock (old.values)
            values = new Dictionary<IWorldState, object>(old.values);
    }

    public ReGoapStateExtended()
    {
        values = new Dictionary<IWorldState, object>();
    }

    public static ReGoapStateExtended operator +(ReGoapStateExtended a, ReGoapStateExtended b)
    {
        ReGoapStateExtended result;
        lock (a.values)
        {
            result = new ReGoapStateExtended(a);
        }
        lock (b.values)
        {
            foreach (var pair in b.values)
                result.values[pair.Key] = pair.Value;
            return result;
        }
    }

    public int Count
    {
        get { return values.Count; }
    }
    public bool DoesFullfillGoal(ReGoapStateExtended goal)
    {
        lock (values) lock (goal.values)
        {
            foreach (var pair in goal.values)
            {
                object thisValue;
                values.TryGetValue(pair.Key, out thisValue);
                var otherValue = pair.Value;
                if (thisValue == otherValue || (thisValue != null && thisValue.Equals(pair.Value)))
                    return true;
            }
            return false;
        }
    }
    public bool HasAnyConflict(ReGoapStateExtended other) // used only in backward for now
    {
        lock (values) lock (other.values)
        {
            foreach (var pair in other.values)
            {
                object thisValue;
                values.TryGetValue(pair.Key, out thisValue);
                var otherValue = pair.Value;
                if (otherValue == null || otherValue.Equals(false)) // backward search does NOT support false preconditions
                    continue;
                if (thisValue != null && !otherValue.Equals(thisValue))
                    return true;
            }
            return false;
        }
    }

    public int MissingDifference(ReGoapStateExtended other, int stopAt = int.MaxValue)
    {
        ReGoapStateExtended nullGoap = null;
        return MissingDifference(other, ref nullGoap, stopAt);
    }

    // write differences in "difference"
    public int MissingDifference(ReGoapStateExtended other, ref ReGoapStateExtended difference, int stopAt = int.MaxValue, Func<KeyValuePair<IWorldState, object>, object, bool> predicate = null, bool test = false)
    {
        lock (values)
        {
            var count = 0;
            foreach (var pair in values)
            {
                var add = false;
                var valueBool = pair.Value as bool?;
                object otherValue;
                other.values.TryGetValue(pair.Key, out otherValue);
                if (valueBool.HasValue)
                {
                    // we don't need to check otherValue type since every key is supposed to always have same value type
                    var otherValueBool = otherValue == null ? false : (bool)otherValue;
                    if (valueBool.Value != otherValueBool)
                        add = true;
                }
                else // generic version
                {
                    var valueVector3 = pair.Value as Vector3?;
                    if (valueVector3 != null && test)
                    {
                        add = true;
                    }
                    else if ((pair.Value == null && otherValue != null) || (!pair.Value.Equals(otherValue)))
                    {
                        add = true;
                    }
                }
                if (add && (predicate == null || predicate(pair, otherValue)))
                {
                    count++;
                    if (difference != null)
                        difference.values[pair.Key] = pair.Value;
                    if (count >= stopAt)
                        break;
                }
            }
            return count;
        }
    }

    public object Clone()
    {
        var clone = new ReGoapStateExtended(this);
        return clone;
    }

    public override string ToString()
    {
        lock (values)
        {
            var result = "GoapState: ";
            foreach (var pair in values)
                result += string.Format("'{0}': {1}, ", pair.Key, pair.Value);
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

    public void SetFrom( IWorldState state, ReGoapStateExtended otherState ) {
        values[state] = otherState.values[state];
    }

    public Dictionary<IWorldState, object> GetValues()
    {
        lock (values)
            return values;
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
}