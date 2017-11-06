using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCAgentSensor : IDCSensor {

    void IDCSensor.UpdateWorld( ReGoapState state, DCAgent agent ) {
        state.Set( "position", agent.transform.position );
    }

}
