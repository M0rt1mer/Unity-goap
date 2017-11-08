using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayeredStateMachine : MonoBehaviour {

    public StateMachineLayer[] layers;
    [HideInInspector]
    public string[] layerState;

    void Awake() {
        layerState = new string[layers.Length];
    }


}
