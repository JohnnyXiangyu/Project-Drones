using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDrone : MonoBehaviour
{
    [SerializeField] GameObject dronePrefab;

    [SerializeField] int times;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (times <= 0) {
            return;
        }

        Instantiate(dronePrefab, new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50)), Quaternion.identity);
        times--;
    }
}
