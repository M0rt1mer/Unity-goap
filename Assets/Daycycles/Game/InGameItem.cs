using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameItem : MonoBehaviour  {

    public DBItem sourceItem;
    public Vector3 positionCache;

    public void Update()
    {
        positionCache = transform.position;
    }

}
