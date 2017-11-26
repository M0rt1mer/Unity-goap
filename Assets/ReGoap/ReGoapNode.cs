using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReGoapNode : INode<ReGoapState>
{
    private readonly float cost;
    private readonly IGoapPlanner planner;
    private readonly ReGoapNode parent;
    private readonly IReGoapAction action;
    private readonly IReGoapActionSettings actionSettings;
    private readonly ReGoapState goal;
    private readonly float g;
    private readonly float h;

    private readonly float heuristicMultiplier = 1;

    public ReGoapNode(IGoapPlanner planner, ReGoapState parentGoal, ReGoapNode parent, ReGoapActionState actionState)
    {
        this.planner = planner;
        this.parent = parent;
        if(actionState != null) {
            this.action = actionState.Action;
            this.actionSettings = actionState.Settings;
        }

        if (this.parent != null){
            g = parent.GetPathCost();
        }

        var nextAction = parent == null ? null : parent.action;
        if(action != null) {


            //first step - subtract effects of action
            var effects = action.GetEffects( parentGoal, actionSettings, nextAction );
            try {
                goal = parentGoal.Difference( effects );

            } catch(ArgumentException e) {
                Debug.Log( e );
            }
            //then add preconditions to the current goal state
            var preconditions = action.GetPreconditions( parentGoal, actionSettings, nextAction );
            goal = goal + preconditions;

            
            g += action.GetCost( parentGoal, actionSettings, nextAction );

        } else goal = parentGoal;
        h = goal.Count;
        // f(node) = g(node) + h(node)
        cost = g + h * heuristicMultiplier;
    }

    public float GetPathCost(){
        return g;
    }

    public float GetHeuristicCost(){
        return h;
    }

    public ReGoapState GetState(){
        return goal;
    }

#if DEBUG
    public IEnumerator<ReGoapActionState> GetPossibleActionsEnumerator( bool includeInvalidAction = false ) {
#else
    public IEnumerator<ReGoapActionState> GetPossibleActionsEnumerator(){
#endif
        var agent = planner.GetCurrentAgent();
        var actions = agent.GetActionsSet();
        foreach (var possibleAction in actions) {
            IReGoapActionSettings settings = possibleAction.Precalculations(agent, goal);
            var precond = possibleAction.GetPreconditions(goal, settings, action);
            var effects = possibleAction.GetEffects(goal, settings, action);
            if (possibleAction == action)
                continue;
            if(effects.DoesFullfillGoal( goal ) && // any effect is the current goal
                !goal.HasConflict( precond, effects ) &&
                possibleAction.CheckProceduralCondition( agent, settings, goal, parent != null ? parent.action : null )) {
#if DEBUG
                yield return new ReGoapActionState( possibleAction, settings ) { preconditions = precond, effects = effects };
#else
                yield return new ReGoapActionState( possibleAction, settings );
#endif
            }
                #if DEBUG
            else if(includeInvalidAction){
                if(!effects.DoesFullfillGoal( goal ))
                    yield return new ReGoapActionState( possibleAction, settings ) { isValid = false, reason = ReGoapActionState.InvalidReason.EFFECTS_DONT_HELP, preconditions = precond, effects = effects };
                if(goal.HasConflict( precond, effects ))
                    yield return new ReGoapActionState( possibleAction, settings ) { isValid = false, reason = ReGoapActionState.InvalidReason.CONFLICT, preconditions = precond, effects = effects };
                if(possibleAction.CheckProceduralCondition( agent, settings, goal, parent != null ? parent.action : null ))
                    yield return new ReGoapActionState( possibleAction, settings ) { isValid = false, reason = ReGoapActionState.InvalidReason.PROCEDURAL_CONDITION, preconditions = precond, effects = effects };
            }
#endif
        }
    }

    public List<INode<ReGoapState>> Expand(){
        var result = new List<INode<ReGoapState>>();
        var possibleActions = GetPossibleActionsEnumerator();
        while (possibleActions.MoveNext())
        {
            result.Add(
                new ReGoapNode(
                    planner,
                    goal,
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

    /// <summary>
    /// Indicates this node is goal (meaning it's action is first in the action list). This is true if current world state fulfills all goal variable
    /// </summary>
    /// <param name="goal"></param>
    /// <returns></returns>
    public bool IsGoal(ReGoapState goal)
    {
        return goal.TryDifference( planner.GetCurrentAgent().GetMemory().GetWorldState() ).IsEmpty();
    }

    public float Priority { get; set; }
    public long InsertionIndex { get; set; }
    public int QueueIndex { get; set; }
}