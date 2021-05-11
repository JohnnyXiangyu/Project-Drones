using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRegister : MonoBehaviour
{
    public static Camera cam;

    private void Awake() {
        cam = gameObject.GetComponent<Camera>();
    }
}
