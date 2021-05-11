using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneDeployer : FirableBase {
    int currentDrone = 0;
    List<GameObject> dronePrefabs = new List<GameObject>();
    List<List<GameObject>> droneInstances = new List<List<GameObject>>();
    
    protected override void Update() {
        base.Update();

        // TODO: switch drone slot
    }

    protected override void PrimaryFire(Vector3 point, GameObject obj) {
        
    }

    protected override void SecondaryFire(Vector3 point, GameObject obj) {
        
    }
}
