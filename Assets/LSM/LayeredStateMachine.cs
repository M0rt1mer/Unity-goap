using System;
using System.Collections.Generic;
using UnityEngine;

public class LayeredStateMachine : MonoBehaviour, ITransitionExecutor {

    public StateMachineLayer[] layers;

    [HideInInspector]
    public Dictionary<StateMachineLayer, StateMachineState> layerState;
    [HideInInspector]
    public Dictionary<StateMachineLayer, IStateMachineTransition> activeTransitions;

    void Awake() {
        layerState = new Dictionary<StateMachineLayer, StateMachineState>(layers.Length);
        activeTransitions = new Dictionary<StateMachineLayer, IStateMachineTransition>(layers.Length);
    }


    void ITransitionExecutor.ExecuteTransition(IStateMachineTransition transition ){

        if (activeTransitions.ContainsKey(transition.Layer)) {
            Debug.Log("Two transitions at once!!!!!");
        }
        else {
            activeTransitions[transition.Layer] = transition;
            transition.Initialize(gameObject);
        }
    }

    void Update() {

        foreach (var key in activeTransitions.Keys)
            if (activeTransitions[key].Update())
                activeTransitions.Remove(key);

    }

}


