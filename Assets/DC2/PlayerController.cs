using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public GameObject marker;
    public Collider colliderComponent;

	// Use this for initialization
	void Start () {
        //collider = GetComponentInChildren<Collider>();
	}
	
	// Update is called once per frame
	void Update () {

        if( Input.GetAxis( "Fire1" ) > 0 ) {
            if(marker != null) {
                RaycastHit hit;
                if(colliderComponent.Raycast( Camera.main.ScreenPointToRay( Input.mousePosition ), out hit, 100F ) ){
                    marker.transform.position = hit.point;
                }
            }

        }

	}
}
