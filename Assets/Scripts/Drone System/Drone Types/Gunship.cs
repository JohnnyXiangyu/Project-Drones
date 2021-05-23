using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Gunship : DroneBase {
    /// <summary>
    /// distance this drone can travel per deployment
    /// </summary>
    [SerializeField]
    float maxMovement; // fuel limitation
    /// <summary>
    /// radius around click position for drone to tell arrival
    /// </summary>
    [SerializeField]
    float positionErrorBound;
    /// <summary>
    /// times the drone will try to chase target before it gives up (used in invasion and secure)
    /// </summary>
    [SerializeField]
    float maxLostSeconds;
    
    float attackRange;
    
    // fuel management
    float remainingMovement;
    Vector3 positionLastFrame;

    NavMeshAgent myAgent;

    // invasion utility
    struct TempTarget {
        public GameObject target;
        public float lostTime;
        public bool activated;
    }

    /// <summary>
    /// the temporary target for invasion and secure
    /// </summary>
    TempTarget tempTarget = new TempTarget() { activated = false };

    // used for invasion only
    bool returning = false;

    // message from owner //////////////////////////////////////////////////////////////////////////////////
    public override void Deploy(Command newCommand) {
        base.Deploy(newCommand);

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
        else if (newCommand.type == Command.CommandType.INVADE) {
            returning = false;
            tempTarget.activated = false;
        }
    }

    public override void Retract() {
        GetComponent<DroneCanon>().SetTarget(null);
    }


    // drone Ai ////////////////////////////////////////////////////////////////////////////////////////////
    protected override void ChaseUpdate() {
        // gunship chases the target and try to shoot
        float distance = GetDistanceFromTargetObj();
        if (remainingMovement <= 0) {
            ReportDepletion();
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
        if (!TemporaryChase()) {
            // if the target is lost: attempt to move to desitination
            float distance = GetDistanceFromTargetPos();
            myAgent.SetDestination(currentCommand.position);
            myAgent.stoppingDistance = 0;

            // if arrived, return to mothership
            if (distance <= positionErrorBound) {
                ReportDepletion();
            }

            // search for enemy
            Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

            // find closest target
            float closestDistance = float.MaxValue;
            GameObject closestObj = null;
            foreach (Collider cd in hits) {
                if (Vector3.Distance(cd.transform.position, transform.position) < closestDistance && cd.GetComponent<AttackableBase>() != null) {
                    closestObj = cd.gameObject;
                    closestDistance = Vector3.Distance(cd.transform.position, transform.position);
                }
            }

            // update temporary target
            if (closestObj) {
                tempTarget.activated = true;
                tempTarget.lostTime = maxLostSeconds;
                tempTarget.target = closestObj;
            }
        }
    }

    protected override void SecureUpdate() {
        throw new System.NotImplementedException();
    }

    protected override void ReturnUpdate() {
        myAgent.stoppingDistance = 0;

        // check mothership location
        // TODO: maybe change this to a shape cast?
        if ((transform.position - currentCommand.spawner.GetComponent<DroneDeployer>().GetRetractPoint()).magnitude <= 1) {
            currentCommand.spawner.GetComponent<DroneDeployer>().RetractDrone(gameObject);
        }
        else {
            // move back to mothership
            myAgent.SetDestination(currentCommand.spawner.GetComponent<DroneDeployer>().GetRetractPoint());
        }
    }

    /// <summary>
    /// The internal action of chasing a temporary target, with the possibility of 
    /// </summary>
    /// <returns></returns>
    private bool TemporaryChase() {
        if (!tempTarget.activated || tempTarget.lostTime > maxLostSeconds) {
            tempTarget.activated = false;

            // cancel aim
            GetComponent<DroneCanon>().SetTarget(null);

            return false;
        }
        else {
            // check lost status
            if (Vector3.Distance(tempTarget.target.transform.position, transform.position) > attackRange) {
                tempTarget.lostTime += Time.deltaTime;
            }

            // move to target
            myAgent.SetDestination(tempTarget.target.transform.position);
            myAgent.stoppingDistance = attackRange;

            // aim
            GetComponent<DroneCanon>().SetTarget(tempTarget.target);

            return true;
        }
    }

    private float GetDistanceFromTargetObj() {
        return (transform.position - currentCommand.clickedObj.transform.position).magnitude;
    }

    private float GetDistanceFromTargetPos() {
        return (transform.position - currentCommand.position).magnitude;
    }
}
