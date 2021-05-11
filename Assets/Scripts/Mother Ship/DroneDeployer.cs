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
    List<GameObject> dronePrefabs = new List<GameObject>();
    List<List<GameObject>> droneInstances = new List<List<GameObject>>();
    
    protected override void Update() {
        // switch drone slot
        // loop each possible drone selection
        for (int i = 0; i < dronePrefabs.Count; i ++) {
            // take input (alpha1 + i => the i_th drone preset)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
                currentDrone = i;
            }
        }

        // TODO: drone manufacturing
        if (Input.GetKeyDown(makeKey)) {
            // not implemented
        }

        base.Update();
    }

    protected override void PrimaryFire(Vector3 point, GameObject obj) {
        // spawn a drone and give it attack-ish order
        if (droneInstances[currentDrone].Count > 0) {
            GameObject newDeploy = droneInstances[currentDrone][-1];

            Command newCommand = new Command();
            newCommand.clickedObj = obj;
            newCommand.position = point;
            newCommand.motherShip = gameObject;

            if (obj.GetComponent<IAttackable>() != null) {
                newCommand.type = Command.CommandType.CHASE;
            }
            
            newDeploy.GetComponent<DroneBase>().Deploy(newCommand);
            newDeploy.transform.position = spawnPoint.position;
        }
    }

    protected override void SecondaryFire(Vector3 point, GameObject obj) {
        // spawn a drone and give it secure order
        if (droneInstances[currentDrone].Count > 0) {
            GameObject newDeploy = droneInstances[currentDrone][-1];

            Command newCommand = new Command() {
                clickedObj = obj,
                position = point,
                motherShip = gameObject,
                type = Command.CommandType.SECURE
            };

            newDeploy.GetComponent<DroneBase>().Deploy(newCommand);
            newDeploy.transform.position = spawnPoint.position;
        }
    }
}
