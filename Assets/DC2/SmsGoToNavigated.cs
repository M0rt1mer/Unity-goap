using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// generic goto state, can be used in most games, override Tick and Enter if you are using 
//  a navmesh / pathfinding library 
//  (ex. tell the library to search a path in Enter, when done move to the next waypoint in Tick)
[RequireComponent( typeof( StateMachine ) )]
[RequireComponent( typeof( SmsIdle ) )]
[RequireComponent( typeof( NavMeshAgent ) )]
public class SmsGoToNavigated : SmState {
    private Vector3 objective;
    protected Transform objectiveTransform;
    private Action onDoneMovementCallback;
    private Action onFailureMovementCallback;

    protected enum GoToState {
        Disabled, Pulsed, Active, Success, Failure
    }
    protected GoToState currentState;
    private Rigidbody body;

    public bool WorkInFixedUpdate;
    // when the magnitude of the difference between the objective and self is <= of this then we're done
    public float MinDistanceToObjective = 0.2f;

    // additional feature, check for stuck, userful when using rigidbody or raycasts for movements
    private Vector3 lastStuckCheckUpdatePosition;
    private float stuckCheckCooldown;
    public bool CheckForStuck;
    public float StuckCheckDelay = 1f;
    public float MaxStuckDistance = 0.1f;

    NavMeshAgent agent;
    protected override void Awake() {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }

    #region Work
    protected override void FixedUpdate() {
        base.FixedUpdate();
        if(!WorkInFixedUpdate) return;
        Tick();
    }

    protected override void Update() {
        base.Update();
        if(WorkInFixedUpdate) return;
        Tick();
    }

    protected virtual void Tick() {
        var objectivePosition = objectiveTransform != null ? objectiveTransform.position : objective;
        MoveTo( objectivePosition );
    }

    protected virtual void MoveTo( Vector3 position ) {
        var delta = position - transform.position;
        if(delta.magnitude <= MinDistanceToObjective) {
            currentState = GoToState.Success;
        }
        if(CheckForStuck && CheckIfStuck()) {
            currentState = GoToState.Failure;
        }
    }

    private bool CheckIfStuck() {
        if(Time.time > stuckCheckCooldown) {
            stuckCheckCooldown = Time.time + StuckCheckDelay;
            if((lastStuckCheckUpdatePosition - transform.position).magnitude < MaxStuckDistance) {
                ReGoapLogger.Log( "[SmsGoToNavigated] '" + name + "' is stuck." );
                return true;
            }
            lastStuckCheckUpdatePosition = transform.position;
        }
        return false;
    }

    #endregion

    #region StateHandler
    public override void Init( StateMachine stateMachine ) {
        base.Init( stateMachine );
        var transistion = new SmTransistion( GetPriority(), Transistion );
        var doneTransistion = new SmTransistion( GetPriority(), DoneTransistion );
        stateMachine.GetComponent<SmsIdle>().Transistions.Add( transistion );
        Transistions.Add( doneTransistion );
    }

    private Type DoneTransistion( ISmState state ) {
        if(currentState != GoToState.Active)
            return typeof( SmsIdle );
        return null;
    }

    private Type Transistion( ISmState state ) {
        if(currentState == GoToState.Pulsed)
            return typeof( SmsGoToNavigated );
        return null;
    }

    public void GoTo( Vector3 position, Action onDoneMovement, Action onFailureMovement ) {
        objective = position;
        GoTo( onDoneMovement, onFailureMovement );
    }

    public void GoTo( Transform transform, Action onDoneMovement, Action onFailureMovement ) {
        objectiveTransform = transform;
        GoTo( onDoneMovement, onFailureMovement );
    }

    void GoTo( Action onDoneMovement, Action onFailureMovement ) {
        currentState = GoToState.Pulsed;
        onDoneMovementCallback = onDoneMovement;
        onFailureMovementCallback = onFailureMovement;
    }

    public override void Enter() {
        base.Enter();
        currentState = GoToState.Active;
        if(objectiveTransform != null)
            agent.SetDestination( objectiveTransform.position );
        else
            agent.SetDestination( objective );
    }

    public override void Exit() {
        base.Exit();
        if(currentState == GoToState.Success)
            onDoneMovementCallback();
        else
            onFailureMovementCallback();
    }
    #endregion
}