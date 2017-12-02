using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGCharacterControllerPassive : MonoBehaviour {

    protected Animator animator;
    private UnityEngine.AI.NavMeshAgent agent;
 

    void Start() {
        //set the animator component
        animator = GetComponentInChildren<Animator>();
        agent = GetComponentInParent<UnityEngine.AI.NavMeshAgent>();
    }

    private void LateUpdate() {
        animator.SetFloat( "Velocity X", Vector3.Dot( agent.velocity, transform.right )/4 );
        animator.SetFloat( "Velocity Z", Vector3.Dot( agent.velocity, transform.forward )/4 );
        if(agent.velocity.sqrMagnitude > 0) {
            animator.SetBool( "Moving", true );
        } else {
            animator.SetBool( "Moving", false );
        }
    }
}
