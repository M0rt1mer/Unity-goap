using NUnit.Framework;
using System;
using UnityEngine.TestTools;

public class ReGoapStateExtendedTest {

    WorldState<float> wsEqual = new WorldState<float>();

    WorldState<float> wsAtLeast = new WorldStateComparable<float,WorldStateLogicAtLeast>();

    WorldState<float> wsAtMost = new WorldStateComparable<float,WorldStateLogicAtMost>();

    [Test]
    public void _1_plusOperator() {

        ReGoapStateExtended stateA = new ReGoapStateExtended();
        ReGoapStateExtended stateB = new ReGoapStateExtended();

        stateA.Set(wsEqual, 1f);
        stateB.Set(wsEqual, 2f);
        ReGoapStateExtended stateC;
        Assert.Throws<ArgumentException>( () => { stateC = stateA + stateB; }, "Two conflicting states" );

        stateB.Clear();
        stateA.Clear();
        stateA.Set(wsAtLeast, 5f);
        stateB.Set(wsAtLeast, 9f);
        stateC = stateA + stateB;
        Assert.AreEqual(9f,stateC.Get(wsAtLeast), "Two AT_LEAST states");

        stateA.Clear();
        stateB.Clear();
        stateA.Set(wsAtMost, 4f);
        stateB.Set(wsAtMost, 9f);
        stateC = stateA + stateB;
        Assert.AreEqual( 4f, stateC.Get(wsAtMost), "Two AT_MOST states" );

    }

    [Test]
    public void _2_hasConflict() {

        ReGoapStateExtended goal = new ReGoapStateExtended();
        ReGoapStateExtended prec = new ReGoapStateExtended();
        ReGoapStateExtended eff = new ReGoapStateExtended();

        goal.Set(wsEqual, 5);
        eff.Set(wsAtLeast, 4);

        Assert.IsFalse( goal.HasConflict( prec, eff ) ); // no common state - no conflict

        goal.Clear(); prec.Clear(); eff.Clear();

        goal.Set(wsAtLeast, 5);
        eff.Set(wsAtLeast, 7);
        Assert.IsFalse(goal.HasConflict(prec, eff), "at least, effect higher"); // effect ensures higher "at least" bound, no conflict

        goal.Clear(); prec.Clear(); eff.Clear();
        goal.Set(wsAtLeast, 9);
        eff.Set(wsAtLeast, 4);
        Assert.IsFalse(goal.HasConflict(prec, eff), "at least, effect lower"); // effect ensures lower "at least" bound, is a conflict



    }




}