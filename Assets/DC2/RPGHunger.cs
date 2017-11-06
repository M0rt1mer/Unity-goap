using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGHunger : GoapSensor {

    public float saturationMultiplier = 1;
    public float saturationLevel = 0;

    public override void UpdateSensor() {
        GetMemory().GetWorldState().Set( "saturation", saturationLevel );
    }

    public void EatItem()

}
