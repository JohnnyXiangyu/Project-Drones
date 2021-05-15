//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Unity.Entities;

//public class CameraFollow : MonoBehaviour
//{
//    public static CameraFollow instance = null;

//    private void Awake() {
//        if (instance == null)
//            instance = this;
//        else if (instance != this)
//            Destroy(gameObject);
//    }

//    public void SetObjPosition(Vector3 pos) {
//        transform.position = pos + new Vector3(0, 80, 0);
//    }
//}
