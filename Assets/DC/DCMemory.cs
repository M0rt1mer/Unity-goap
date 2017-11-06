using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCMemory : IReGoapMemory {

    protected ReGoapState state;
    private IReGoapSensor[] sensors;

    public DCMemory( IReGoapSensor[] sensors ) {
        this.sensors = sensors;
        state = new ReGoapState();
        foreach(var sensor in sensors) {
            sensor.Init( this );
        }
    }
    
    protected virtual void Update() {
        foreach(var sensor in sensors) {
            sensor.UpdateSensor();
        }
    }

    public virtual ReGoapState GetWorldState() {
        return state;
    }

}
