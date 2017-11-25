using NUnit.Framework;
using System;
using UnityEngine.TestTools;

public class ReGoapStateTest {

    WorldState<float> wsEqual = new WorldState<float>("equal");

    WorldState<float> wsAtLeast = new WorldStateComparable<float,WorldStateLogicAtLeast>("at least");

    WorldState<float> wsAtMost = new WorldStateComparable<float,WorldStateLogicAtMost>("at most");

    [Test]
    public void _1_plusOperator() {

        ReGoapState stateA = new ReGoapState();
        ReGoapState stateB = new ReGoapState();

        stateA.Set(wsEqual, 1f);
        stateB.Set(wsEqual, 2f);
        ReGoapState stateC;
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

        ReGoapState goal = new ReGoapState();
        ReGoapState prec = new ReGoapState();
        ReGoapState eff = new ReGoapState();

        goal.Set(wsEqual, 5);
        eff.Set(wsAtLeast, 4);

        Assert.IsFalse( goal.HasConflict( prec, eff ) ); // no common state - no conflict

        goal.Clear(); prec.Clear(); eff.Clear();

        goal.Set(wsAtLeast, 5);
        eff.Set(wsAtLeast, 7);
        Assert.IsFalse(goal.HasConflict(prec, eff), "at least, effect higher"); // effect ensures higher "at least" bound, no conflict

        goal.Set( wsAtLeast, 7 );
        eff.Set( wsAtLeast, 5 );
        Assert.IsFalse( goal.HasConflict( prec, eff ), "at least, goal higher" ); // effect ensures higher "at least" bound, no conflict

        goal.Set( wsAtMost, 5 );
        eff.Set( wsAtMost, 7 );
        Assert.IsFalse( goal.HasConflict( prec, eff ), "at most, effect higher" ); // effect ensures higher "at least" bound, no conflict

        goal.Set( wsAtMost, 5 );
        eff.Set( wsAtMost, 7 );
        Assert.IsFalse( goal.HasConflict( prec, eff ), "at most, goal higher" ); // effect ensures higher "at least" bound, no conflict


        goal.Clear(); prec.Clear(); eff.Clear();
        goal.Set( wsEqual, 10 );
        prec.Set( wsEqual, 9 );
        Assert.IsTrue( goal.HasConflict( prec, eff ), "precond and goal different results" );


        goal.Clear(); prec.Clear(); eff.Clear();
        goal.Set( wsEqual, 10 );
        eff.Set( wsEqual, 10 );
        prec.Set( wsEqual, 8 );

        Assert.IsFalse( goal.HasConflict( prec, eff ), "precond and effect share key" );

    }

    [Test]
    public void _3_Difference() {

        ReGoapState eff = new ReGoapState();
        ReGoapState goal = new ReGoapState();
        ReGoapState difference;

        eff.Set( wsEqual, 4 );
        goal.Set( wsEqual, 4 );
        difference = goal.Difference( eff );
        Assert.IsFalse( difference.HasKey( wsEqual ), "subtracting two identical EQUAL values" );

        eff.Clear();
        eff.Set( wsEqual, 5 );

        Assert.Throws<ArgumentException>( () => goal.Difference(eff), "subtracting two different EQUAL values" );

        eff.Clear(); goal.Clear();

        goal.Set( wsAtLeast, 10 );
        eff.Set( wsAtLeast, 5 );

        difference = goal.Difference( eff );
        Assert.AreEqual( 10, difference.Get( wsAtLeast ), "At least, 10 \\ 5" );

        eff.Set( wsAtLeast, 15 );

        difference = goal.Difference( eff );
        Assert.IsFalse( difference.HasKey( wsAtLeast ), "At least, 10 \\ 5" );

        goal.Clear();
        eff.Clear();
        goal.Set( wsAtMost, 10 );

        eff.Set( wsAtMost, 5 );
        difference = goal.Difference( eff );
        Assert.IsFalse( difference.HasKey( wsAtMost ), "At most, 10 \\ 5" );

        eff.Set( wsAtMost, 15 );
        difference = goal.Difference( eff );
        Assert.AreEqual( 10, difference.Get( wsAtMost ), "At most, 10 \\ 5" );

    }

    [Test]
    public void _4_ChainOperations() {

        ReGoapState goal = new ReGoapState();

        goal.Set( wsEqual, 10 );
        goal.Set( wsAtLeast, 25 );
        goal.Set( wsAtMost, 4 );

        ReGoapState eff4 = new ReGoapState();
        eff4.Set( wsAtLeast, 20 );
        eff4.Set( wsAtMost, 2 );
        var preEff4 = goal.Difference( eff4 );

        ReGoapState eff3 = new ReGoapState();
        eff3.Set( wsEqual, 10 );
        var preEff3 = preEff4.Difference( eff3 );

        ReGoapState eff2 = new ReGoapState();
        eff2.Set( wsAtLeast, 40 );
        eff2.Set( wsAtMost, 15 );

        var preEff2 = preEff3.Difference( eff2 );

        Assert.IsFalse( preEff2.HasKey( wsAtMost ), "initial set contains wsAtMost" );
        Assert.IsFalse( preEff2.HasKey( wsAtLeast ), "initial set contains wsAtLeast" );
        Assert.IsFalse( preEff2.HasKey( wsEqual ), "initial set contains wsEqual" );

        var reconstructedGoal = preEff2 + eff2 + eff3 + eff4;

        Assert.AreEqual( 10, reconstructedGoal.Get( wsEqual ), "reconstructed goal EQUAL test" );
        Assert.IsTrue( reconstructedGoal.Get( wsAtLeast ) >= 25, "reconstructed goal AT_LEAST test" );
        Assert.IsTrue( reconstructedGoal.Get( wsAtMost ) <= 4, "reconstructed goal AT_MOST test" );
    }



}