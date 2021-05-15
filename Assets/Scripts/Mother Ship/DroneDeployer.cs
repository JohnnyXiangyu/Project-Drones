using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneDeployer : FirableBase {
    // key binding
    [SerializeField]
    KeyCode makeKey = KeyCode.Space;

    // location settings
    [SerializeField]
    Transform spawnPoint;
    [SerializeField]
    Transform recyclePoint;

    // inventory system
    int currentDrone = 0;
    [SerializeField]
    List<GameObject> dronePrefabs;
    List<List<GameObject>> droneInstances = new List<List<GameObject>>();

    private void Start() {
        // make sure there are exactly n inventory lists for n types of drones
        foreach (var prefab in dronePrefabs) {
            droneInstances.Add(new List<GameObject>());
        }
    }

    protected override void Update() {
        // switch drone slot
        // loop each possible drone selection
        for (int i = 0; i < dronePrefabs.Count; i ++) {
            // take input (alpha1 + i => the i_th drone preset)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
                currentDrone = i;
                Debug.Log("drone deployer: selected " + i.ToString());
            }
        }

        // TODO: drone manufacturing
        if (Input.GetKeyDown(makeKey)) {
            // TODO: make this an actual creation procedure
        }

        base.Update();
    }

    protected override void PrimaryFire(Vector3 point, GameObject obj) {
        // spawn a drone and give it attack-ish order
        if (droneInstances[currentDrone].Count > 0) {
            Command newCommand = new Command();
            newCommand.clickedObj = obj;
            newCommand.position = point;
            newCommand.spawner = gameObject;

            if (obj.GetComponent<AttackableBase>() != null) {
                newCommand.type = Command.CommandType.CHASE;
            }
            else {
                newCommand.type = Command.CommandType.INVADE;
            }

            // prepare new drone
            GameObject newDeploy = droneInstances[currentDrone][0];
            droneInstances[currentDrone].RemoveAt(0);
            newDeploy.GetComponent<DroneBase>().Deploy(newCommand);
            newDeploy.transform.position = spawnPoint.position;
            newDeploy.SetActive(true);
        }
    }

    // TODO: don't put it in yet
    protected override void SecondaryFire(Vector3 point, GameObject obj) {
        //// spawn a drone and give it secure order
        //if (droneInstances[currentDrone].Count > 0) {
        //    // take out a drone from inventory
        //    GameObject newDeploy = droneInstances[currentDrone][-1];
        //    droneInstances[currentDrone].RemoveAt(-1);

        //    // set the new command
        //    Command newCommand = new Command() {
        //        clickedObj = obj,
        //        position = point,
        //        motherShip = gameObject,
        //        type = Command.CommandType.SECURE
        //    };

        //    // deploy drone
        //    newDeploy.GetComponent<DroneBase>().Deploy(newCommand);
        //    newDeploy.transform.position = spawnPoint.position;
        //}
    }

    // message from drones /////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Take the drone back. Should be called from a drone
    /// </summary>
    /// <param name="drone">the drone object to be taken back</param>
    public void ReclaimDrone(GameObject drone) {
        drone.GetComponent<DroneBase>().Retract();
        drone.SetActive(false);
        
        // TODO: figure out what's the type of this drone
    }

    public Vector3 RecyclePoint() {
        return recyclePoint.position;
    }

    // debug functions /////////////////////////////////////////////////////////////////////////
    public void AddDummyDrone() {
        GameObject newDrone = Instantiate(dronePrefabs[0]);
        newDrone.SetActive(false);
        droneInstances[0].Add(newDrone);
    }
}
