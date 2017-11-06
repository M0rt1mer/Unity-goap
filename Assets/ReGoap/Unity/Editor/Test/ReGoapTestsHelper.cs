﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UnityEngine;

public class ReGoapTestsHelper
{
    public static ReGoapTestAction GetCustomAction(GameObject gameObject, string name, Dictionary<string, bool> preconditionsBools,
        Dictionary<string, bool> effectsBools, int cost = 1)
    {
        var effects = new ReGoapState();
        var preconditions = new ReGoapState();
        var customAction = gameObject.AddComponent<ReGoapTestAction>();
        customAction.Name = name;
        customAction.Init();
        foreach (var pair in effectsBools)
            effects.Set(pair.Key, pair.Value);
        customAction.SetEffects(effects);
        foreach (var pair in preconditionsBools)
            preconditions.Set(pair.Key, pair.Value);
        customAction.SetPreconditions(preconditions);
        customAction.Cost = cost;
        return customAction;
    }

    public static ReGoapTestGoal GetCustomGoal(GameObject gameObject, string name, Dictionary<string, bool> goalState, int priority = 1)
    {
        var customGoal = gameObject.AddComponent<ReGoapTestGoal>();
        customGoal.Name = name;
        customGoal.SetPriority(priority);
        customGoal.Init();
        var goal = new ReGoapState();
        foreach (var pair in goalState)
        {
            goal.Set(pair.Key, pair.Value);
        }
        customGoal.SetGoalState(goal);
        return customGoal;
    }

    public static void ApplyAndValidatePlan(IReGoapGoal plan, ReGoapTestMemory memory)
    {
        foreach (var action in plan.GetPlan())
        {
            Assert.That(action.Action.GetPreconditions(plan.GetGoalState()).MissingDifference(memory.GetWorldState(), 1) == 0);
            foreach (var effectsPair in action.Action.GetEffects(plan.GetGoalState()).GetValues())
            {   // in a real game this should be done by memory itself
                //  e.x. isNearTarget = (transform.position - target.position).magnitude < minRangeForCC
                memory.SetValue(effectsPair.Key, effectsPair.Value);
            }
        }
        Assert.That(plan.GetGoalState().MissingDifference(memory.GetWorldState(), 1) == 0);
    }
}
