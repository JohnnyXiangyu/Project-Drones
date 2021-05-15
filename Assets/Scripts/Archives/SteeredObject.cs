using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeredObject : MonoBehaviour
{
    [SerializeField] Transform goal;
    [SerializeField] float angularVelocity;
    [SerializeField] float linearVelocity;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!GoalCheck()) {
            NextPosition();
        }
    }

    // TODO: figure out if the current position is the goal position
    bool GoalCheck() {
        return (transform.position - goal.position).magnitude <= 5;
    }

    // TODO: update position and rotation
    void NextPosition() {
        // rotate towards the object
        Vector3 target = goal.position;
        float angle = AngleBetween(transform.forward, target - transform.position);
        float maxRotation = angularVelocity * Time.deltaTime;

        float actualRotation = Mathf.Min(Mathf.Abs(angle), maxRotation);

        Vector3 axis = Vector3.Cross(transform.forward, target - transform.position);

        if (transform.forward == (target - transform.position).normalized * -1) {
            axis = transform.up;
        }

        transform.Rotate(axis, actualRotation, Space.World);

        // move forward
        transform.position += transform.forward * Time.deltaTime * linearVelocity;
    }

    float AngleBetween(Vector3 src, Vector3 dest) {
        float angle = Mathf.Acos(Vector3.Dot(src.normalized, dest.normalized));
        
        //// edge case: in the back
        //if (angle == 0 && Vector3.Dot(dest, src) < 0) {
        //    angle = 180;
        //}

        Debug.Log(angle);
        return angle / Mathf.PI * 180;
    }
}
