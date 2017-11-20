using System;
using System.Collections.Generic;
using System.Linq;

public class ReGoapNode : INode<ReGoapState>
{
    private readonly float cost;
    private readonly IGoapPlanner planner;
    private readonly ReGoapNode parent;
    private readonly IReGoapAction action;
    private readonly IReGoapActionSettings actionSettings;
    private readonly ReGoapState state;
    private readonly ReGoapState goal;
    private readonly float g;
    private readonly float h;

    private readonly float heuristicMultiplier = 1;

    public ReGoapNode(IGoapPlanner planner, ReGoapState newGoal, ReGoapNode parent, ReGoapActionState actionState)
    {
        this.planner = planner;
        this.parent = parent;
        if(actionSettings != null) {
            this.action = actionState.Action;
            this.actionSettings = actionState.Settings;
        }

        if (this.parent != null)
        {
            state = this.parent.GetState();
            // g(node)
            g = parent.GetPathCost();
        }
        else
        {
            state = planner.GetCurrentAgent().GetMemory().GetWorldState();
        }

        var nextAction = parent == null ? null : parent.action;
        if (action != null)
        {
            // backward search does NOT support negative preconditions
            // since in backward search we relax the problem all preconditions are valid but are added to the current goal
            var preconditions = action.GetPreconditions(newGoal, actionSettings, nextAction);
            goal = newGoal + preconditions;

            var effects = action.GetEffects(newGoal, actionSettings, nextAction);
            state += effects;
            g += action.GetCost(newGoal, actionSettings, nextAction);

            // removing current action effects from goal, no need to do with to the whole state
            //  since the state is the sum of all the previous actions's effects.
            var missingState = new ReGoapState();
            goal.MissingDifference(effects, ref missingState);
            goal = missingState;

            // this is needed every step to make sure that any precondition is not already satisfied
            //  by the world state
            var worldMissingState = new ReGoapState();
            goal.MissingDifference(planner.GetCurrentAgent().GetMemory().GetWorldState(), ref worldMissingState);
            goal = worldMissingState;
        }
        else
        {
            var diff = new ReGoapState();
            newGoal.MissingDifference(state, ref diff);
            goal = diff;
        }
        h = goal.Count;
        // f(node) = g(node) + h(node)
        cost = g + h * heuristicMultiplier;
    }

    public float GetPathCost()
    {
        return g;
    }

    public float GetHeuristicCost()
    {
        return h;
    }

    public ReGoapState GetState()
    {
        return state;
    }

    public IEnumerator<ReGoapActionState> GetPossibleActionsEnumerator()
    {
        var agent = planner.GetCurrentAgent();
        var actions = agent.GetActionsSet();
        foreach (var possibleAction in actions) {
            IReGoapActionSettings settings = possibleAction.Precalculations(agent, goal);
            var precond = possibleAction.GetPreconditions(goal, settings, action);
            var effects = possibleAction.GetEffects(goal, settings, action);
            if (possibleAction == action)
                continue;
            if (effects.HasAny(goal) && // any effect is the current goal
                !goal.HasAnyConflict(effects) && // no effect is conflicting with the goal
                !goal.HasAnyConflict(precond) && // no precondition is conflicting with the goal
                possibleAction.CheckProceduralCondition(agent, settings, goal, parent != null ? parent.action : null))
                yield return new ReGoapActionState( possibleAction, settings );
        }
    }

    public List<INode<ReGoapState>> Expand()
    {
        var result = new List<INode<ReGoapState>>();
        var possibleActions = GetPossibleActionsEnumerator();
        while (possibleActions.MoveNext())
        {
            var newGoal = goal;
            result.Add(
                new ReGoapNode(
                    planner,
                    newGoal,
                    this,
                    possibleActions.Current));
        }
        return result;
    }

    private IReGoapAction GetAction()
    {
        return action;
    }

    public Queue<ReGoapActionState> CalculatePath()
    {
        var listResult = new List<ReGoapActionState>();
        var node = this;
        while (node.GetParent() != null)
        {
            listResult.Add(new ReGoapActionState(node.action, node.actionSettings));
            node = (ReGoapNode)node.GetParent();
        }
        var result = new Queue<ReGoapActionState>(listResult.Count);
        foreach (var thisActionState in listResult)
        {
            result.Enqueue(thisActionState);
        }
        return result;
    }

    public int CompareTo(INode<ReGoapState> other)
    {
        return cost.CompareTo(other.GetCost());
    }

    public float GetCost()
    {
        return cost;
    }

    public INode<ReGoapState> GetParent()
    {
        return parent;
    }

    public bool IsGoal(ReGoapState goal)
    {
        return h == 0;
    }

    public float Priority { get; set; }
    public long InsertionIndex { get; set; }
    public int QueueIndex { get; set; }
}