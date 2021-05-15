using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    public float speed;
    
    // Update is called once per frame
    void Update()
    {
        // fly to the target
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
