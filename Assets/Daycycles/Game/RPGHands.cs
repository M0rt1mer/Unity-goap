using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGHands : GoapSensor {

    public InGameItem leftHandItem;
    public InGameItem rightHandItem;

    public override void UpdateSensor() {
        GetMemory().GetWorldState().Set( WorldStates.STATE_HAND_LEFT, leftHandItem.sourceItem );
        GetMemory().GetWorldState().Set( WorldStates.STATE_HAND_RIGHT, rightHandItem.sourceItem );
    }

}
