
using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour {
    //
    // VARIABLES
    //

    public float panSpeed = 4.0f;       // Speed of the camera when being panned

    private Vector3 mouseOrigin;    // Position of cursor when mouse dragging starts
    private Plane basePlane = new Plane( Vector3.up, Vector3.zero );
    //
    // UPDATE
    //

    void Update() {


        // Get the right mouse button
        if(Input.GetMouseButtonDown( 1 )) {
            // Get mouse origin
            mouseOrigin = Input.mousePosition;
        } else if(Input.GetMouseButton( 1 )) {
            Vector3 lastMousePos = Raycast( mouseOrigin );
            Vector3 currentMousePos = Raycast( Input.mousePosition );
            
            transform.Translate( currentMousePos - lastMousePos, Space.World );
            mouseOrigin = Input.mousePosition;
        }
        
    }

    private Vector3 Raycast( Vector3 mousePos ) {
        float dist;
        Ray ray = Camera.main.ScreenPointToRay( mousePos );
        basePlane.Raycast( ray, out dist );
        return ray.origin + ray.direction * dist;
    }


}