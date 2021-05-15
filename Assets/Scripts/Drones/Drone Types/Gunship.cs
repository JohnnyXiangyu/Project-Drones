using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Gunship : DroneBase {
    [SerializeField]
    float attackRange;
    [SerializeField]
    float avoidRange;
    [SerializeField]
    float maxMovement; // fuel limitation

    // fuel management
    float remainingMovement;
    Vector3 positionLastFrame; // TODO: make sure this value is initialized properly on initialization

    NavMeshAgent myAgent;

    public override void Deploy(Command newCommand) {
        base.Deploy(newCommand);

        positionLastFrame = transform.position;
        remainingMovement = maxMovement;
        myAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    protected override void ChaseUpdate() {
        // gunship chases the target and try to shoot
        float distance = (transform.position - currentCommand.clickedObj.transform.position).magnitude;
        if (remainingMovement <= 0) {
            // TODO: move back to mothership
        }
        else if (distance > attackRange) {
            // tell agent to move to the target
            myAgent.stoppingDistance = attackRange;
            myAgent.SetDestination(currentCommand.clickedObj.transform.position);
        }

        // subtract distance from current fuel (even when it's negative)
        float movement = (transform.position - positionLastFrame).magnitude;
        remainingMovement -= movement;
        positionLastFrame = transform.position;
    }

    protected override void InvadeUpdate() {
        throw new System.NotImplementedException();
    }

    protected override void SecureUpdate() {
        throw new System.NotImplementedException();
    }
}
