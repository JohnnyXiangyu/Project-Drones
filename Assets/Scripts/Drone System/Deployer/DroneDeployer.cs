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

    // parameteres
    [SerializeField]
    float deployInterval;
    float nextDeployTime = -1; // negative initial value is crucial

    // inventory system
    int currentDrone = 0;

    /// <summary>
    /// All the prefabs accessable from this mothership (including locked)
    /// </summary>
    [SerializeField]
    List<GameObject> dronePrefabs;

    // TODO: let player choose between a couple of prefabs
    /// <summary>
    /// Drone models that are selected currently.
    /// </summary>
    List<GameObject> selectedPrefabs;

    /// <summary>
    /// The inventory corresponding to all the drones (all types of drones share a common buffer)
    /// </summary>
    Dictionary<GameObject, Queue<GameObject>> droneInventory = new Dictionary<GameObject, Queue<GameObject>>();


    private void Start() {
        // make sure there are exactly n inventory lists for n types of drones
        foreach (var prefab in dronePrefabs) {
            // droneInstances.Add(new List<GameObject>());
            droneInventory.Add(prefab, new Queue<GameObject>());
        }
    }

    protected override void Update() {
        // switch drone slot
        // loop each possible drone selection
        for (int i = 0; i < dronePrefabs.Count; i++) {
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
        if (Time.time < nextDeployTime) {
            return;
        }

        // spawn a drone and give it attack-ish order
        if (/*droneInstances[currentDrone].Count*/droneInventory[dronePrefabs[currentDrone]].Count > 0) {
            // prepare command
            Command newCommand = new Command();
            newCommand.clickedObj = obj;
            newCommand.position = point;
            newCommand.spawner = gameObject;

            // choose command type based on input
            if (obj.GetComponent<AttackableBase>() != null) {
                newCommand.type = Command.CommandType.CHASE;
            }
            else {
                newCommand.type = Command.CommandType.INVADE;
            }

            // prepare new drone
            GameObject newDeploy = TakeDrone(dronePrefabs[currentDrone]);
            if (newDeploy != null) {
                // deploy drone
                newDeploy.GetComponent<DroneBase>().Deploy(newCommand);
                newDeploy.transform.position = spawnPoint.position;
                newDeploy.transform.rotation = transform.rotation;
                newDeploy.SetActive(true);

                // update timer
                nextDeployTime = Time.time + deployInterval;
            }
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

    // drone production system /////////////////////////////////////////////////////////////////
    private void AddDrone(GameObject prefab) {
        GameObject newDrone = Instantiate(prefab);
        newDrone.SetActive(false);
        newDrone.GetComponent<DroneBase>().modelPrefab = prefab;
        droneInventory[prefab].Enqueue(newDrone);
    }

    private GameObject TakeDrone(GameObject prefab) {
        if (droneInventory[dronePrefabs[currentDrone]].Count > 0) {
            return droneInventory[dronePrefabs[currentDrone]].Dequeue();
        }
        else {
            return null;
        }
    }

    // message from drones /////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Take the drone back. Should be called from a drone
    /// </summary>
    /// <param name="drone">the drone object to be taken back</param>
    public void RetractDrone(GameObject drone) {
        drone.GetComponent<DroneBase>().Retract();
        drone.SetActive(false);

        // TODO: put the drone into the inventory corresponding to its type
        droneInventory[drone.GetComponent<DroneBase>().modelPrefab].Enqueue(drone);
    }

    public Vector3 GetRetractPoint() {
        return recyclePoint.position;
    }

    // debug functions /////////////////////////////////////////////////////////////////////////
    public void AddDummyDrone() {
        AddDrone(dronePrefabs[currentDrone]);
    }
}
