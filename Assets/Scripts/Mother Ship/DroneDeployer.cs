using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneDeployer : FirableBase {
    // settings
    [SerializeField]
    KeyCode makeKey = KeyCode.Space;
    [SerializeField]
    Transform spawnPoint;

    int currentDrone = 0;
    [SerializeField]
    List<GameObject> dronePrefabs/* = new List<GameObject>()*/;
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
            newCommand.motherShip = gameObject;

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

    // debug functions /////////////////////////////////////////////////////////////////////////
    public void AddDummyDrone() {
        GameObject newDrone = Instantiate(dronePrefabs[0]);
        newDrone.SetActive(false);
        droneInstances[0].Add(newDrone);
    }
}
