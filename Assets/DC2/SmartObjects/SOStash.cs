using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class SOStash : SmartObject {

    public Inventory inv { get; protected set; }

    protected void Awake() {
        inv = GetComponent<Inventory>();
    }

    public Vector3 GetEntryPoint() {
        return transform.position;
    }

}