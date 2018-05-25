using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RPGSmartObjectSimpleState : SmartObject {

    public bool isStateOn;
    /// <summary>
    /// If true, indicator is active when state is inactive
    /// </summary>
    public bool indicatorInversed;

    public GameObject indicator;
    public abstract AStateVarKey<bool> GetIndicatingWorldState();

	// Use this for initialization
	void Start () {
        isStateOn = false;
	}

    void Update()
    {
        base.Update();
        indicator.SetActive(isStateOn ^ indicatorInversed);
    }

}


