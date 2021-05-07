using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDrone : MonoBehaviour
{
    [SerializeField] GameObject dronePrefab;
    [SerializeField] int droneTag;
    [SerializeField] int times;
    [SerializeField] bool unlimited;

    public static SpawnDrone instance = null;

    public bool newDroneThisFrame = false;
    public int newDroneTag = -1;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        newDroneThisFrame = false;
        
        if (times <= 0 && !unlimited) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            Instantiate(dronePrefab, new Vector3(20, 0, 20), Quaternion.identity);
            times--;
            newDroneThisFrame = true;
            newDroneTag = droneTag;
        }
    }
}
