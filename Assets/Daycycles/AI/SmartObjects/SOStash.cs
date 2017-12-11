using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class SOStash : SmartObject {

    public Inventory inv { get; protected set; }
    private Vector3 cachedPosition;

    protected void Awake() {
        inv = GetComponent<Inventory>();
        cachedPosition = transform.position;
    }

    public Vector3 GetEntryPoint() {
        return cachedPosition;
    }

}