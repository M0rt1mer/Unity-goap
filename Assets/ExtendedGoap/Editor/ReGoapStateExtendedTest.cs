using NUnit.Framework;
using System;
using UnityEngine.TestTools;

public class ReGoapStateExtendedTest {

    WorldState<float> wsEqual = new WorldState<float>();

    WorldState<float> wsAtLeast = new WorldStateComparable<float>( WorldStateLogic.AT_LEAST );

    WorldState<float> wsAtMost = new WorldStateComparable<float>(WorldStateLogic.AT_MOST);

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
    public void _2_effectCompatibility() {

        


    }




}