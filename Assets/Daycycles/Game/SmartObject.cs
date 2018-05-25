using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SmartObject : MonoBehaviour {

    public Vector3 positionCache;

    protected void Update()
    {
        positionCache = transform.position;
    }


}
