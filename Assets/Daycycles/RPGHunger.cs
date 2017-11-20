using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGHunger : GoapSensor {

    public float saturationMultiplier = 1;
    public float saturationLevel = 0;

    public override void UpdateSensor() {
        GetMemory().GetWorldState().Set( WorldStates.STATE_FLOAT_HUNGER, saturationLevel );
    }

    public void EatItem( string item ) {
        saturationLevel = Mathf.Max(saturationLevel + GetSingleItemFoodLevel(), 1);
    }

    public static float GetSingleItemFoodLevel() {
        return 0.34f;
    }
}
