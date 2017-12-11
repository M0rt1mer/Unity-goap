
public class ReGoapTestGoal : GoapGoal
{
    public void Init()
    {
        Awake();
    }

    public void SetGoalState(BGoapState goalState)
    {
        goal = goalState;
    }

    public void SetPriority(int priority)
    {
        this.Priority = priority;
    }
}