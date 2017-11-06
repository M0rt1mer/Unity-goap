using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSensor : GoapSensor {

    public override void UpdateSensor() {

        this.GetMemory().GetWorldState().Set( "soList", GameObject.FindObjectsOfType<SmartObject>() );

    }

}
