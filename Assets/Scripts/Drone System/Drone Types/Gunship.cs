using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Gunship : DroneBase {
    [SerializeField]
    float avoidRange;
    [SerializeField]
    float maxMovement; // fuel limitation
    
    float attackRange;
    
    // fuel management
    float remainingMovement;
    Vector3 positionLastFrame;

    // resource management
    bool running = true;

    NavMeshAgent myAgent;

    // message from owner //////////////////////////////////////////////////////////////////////////////////
    public override void Deploy(Command newCommand) {
        base.Deploy(newCommand);

        running = true;

        // update information from weapon system
        attackRange = GetComponent<DroneCanon>().range;

        // initialze fuel system
        // TODO: refactor this to a separate component
        positionLastFrame = transform.position;
        remainingMovement = maxMovement;
        myAgent = gameObject.GetComponent<NavMeshAgent>();

        // configure weapon system
        if (newCommand.type == Command.CommandType.CHASE) {
            GetComponent<DroneCanon>().SetTarget(newCommand.clickedObj);
            GetComponent<DroneCanon>().Reload();
        }
    }

    public override void Retract() {
        GetComponent<DroneCanon>().SetTarget(null);
    }


    // message from component //////////////////////////////////////////////////////////////////////////////
    public override void ReportDepletion() {
        running = false;
    }


    // drone Ai ////////////////////////////////////////////////////////////////////////////////////////////
    protected override void ChaseUpdate() {
        // gunship chases the target and try to shoot
        float distance = (transform.position - currentCommand.clickedObj.transform.position).magnitude;
        if (remainingMovement <= 0 || running == false) {
            myAgent.stoppingDistance = 0;

            // check mothership location
            // TODO: maybe change this to a shape cast?
            if ((transform.position - currentCommand.spawner.GetComponent<DroneDeployer>().GetRetractPoint()).magnitude <= 1) {
                currentCommand.spawner.GetComponent<DroneDeployer>().RetractDrone(gameObject);
            }

            // move back to mothership
            myAgent.SetDestination(currentCommand.spawner.GetComponent<DroneDeployer>().GetRetractPoint());
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
