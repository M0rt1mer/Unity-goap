using NUnit.Framework;
using System;

public class ReGoapStateExtendedTest {

    WorldState<float> wsEqual = new WorldState<float>();

    WorldState<float> wsAtLeast = new WorldState<float>( WorldStateLogic.AT_LEAST );

    WorldState<float> wsAtMost = new WorldState<float>(WorldStateLogic.AT_MOST);

    [Test]
    public void plusOperator() {

        ReGoapStateExtended stateA = new ReGoapStateExtended();
        ReGoapStateExtended stateB = new ReGoapStateExtended();

        stateA.Set(wsEqual, 1f);
        stateB.Set(wsEqual, 2f);
        ReGoapStateExtended stateC;
        Assert.Throws<ArgumentException>( () => { stateC = stateA + stateB; }  );

        stateC = new ReGoapStateExtended();
        stateA.Set(wsAtLeast, 5f);
        stateC.Set(wsAtLeast, 9f);
        var stateD = stateA + stateC;
        Assert.Equals(stateD.Get(wsAtLeast),9f);

        stateB.Set(wsAtMost, 4f);
        stateD.Set(wsAtMost, 9f);
        var stateE = stateB + stateD;
        Assert.Equals(stateD.Get(wsAtMost), 4f);

    }

    [Test]
    public void effectCompatibility() {

        


    }




}