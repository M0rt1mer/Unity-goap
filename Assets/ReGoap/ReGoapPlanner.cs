using System;
using System.Collections.Generic;

public class ReGoapPlanner : IGoapPlanner
{
    private IReGoapAgent goapAgent;
    private IReGoapGoal currentGoal;
    public bool Calculated;
    private readonly AStar<BGoapState> astar;
    private readonly ReGoapPlannerSettings settings;

    public ReGoapPlanner(ReGoapPlannerSettings settings = null)
    {
        this.settings = settings ?? new ReGoapPlannerSettings();
        astar = new AStar<BGoapState>(this.settings.MaxNodesToExpand);
    }

    public IReGoapGoal Plan(IReGoapAgent agent, IReGoapGoal blacklistGoal = null, Queue<ReGoapActionState> currentPlan = null, Action<IReGoapGoal> callback = null)
    {
        ReGoapLogger.Log("[ReGoalPlanner] Starting planning calculation for agent: " + agent);
        goapAgent = agent;
        Calculated = false;
        currentGoal = null;
        var possibleGoals = new List<IReGoapGoal>();
        foreach (var goal in goapAgent.GetGoalsSet())
        {
            if (goal == blacklistGoal)
                continue;
            goal.Precalculations(this);
            if (goal.IsGoalPossible()) //goal.GetPriority() > bestPriority && 
                possibleGoals.Add(goal);
        }
        possibleGoals.Sort((x, y) => x.GetPriority().CompareTo(y.GetPriority()));

        while (possibleGoals.Count > 0)
        {
            currentGoal = possibleGoals[possibleGoals.Count - 1];
            possibleGoals.RemoveAt(possibleGoals.Count - 1);
            var goalState = currentGoal.GetGoalState();

            var leaf = (BGoapNode)astar.Run(
                new BGoapNode(this, goalState, null, null), goapAgent.GetMemory().GetWorldState(), settings.MaxIterations, settings.PlanningEarlyExit);
            if (leaf == null)
            {
                currentGoal = null;
                continue;
            }
            var path = leaf.CalculatePath();
            if (currentPlan != null && currentPlan == path)
            {
                currentGoal = null;
                break;
            }
            currentGoal.SetPlan(path);
            break;
        }
        Calculated = true;

        if (callback != null)
            callback(currentGoal);
        if (currentGoal != null)
            ReGoapLogger.Log(string.Format("[ReGoapPlanner] Calculated plan for goal '{0}', plan length: {1}", currentGoal, currentGoal.GetPlan().Count));
        else
            ReGoapLogger.LogWarning("[ReGoapPlanner] Error while calculating plan.");
        return currentGoal;
    }

    public IReGoapGoal GetCurrentGoal()
    {
        return currentGoal;
    }

    public IReGoapAgent GetCurrentAgent()
    {
        return goapAgent;
    }

    public bool IsPlanning()
    {
        return !Calculated;
    }

    public ReGoapPlannerSettings GetSettings()
    {
        return settings;
    }
}