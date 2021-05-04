using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAgent : MonoBehaviour
{
    public static bool wDown;
    public static bool sDown;
    public static bool aDown;
    public static bool dDown;

    private void Update() {
        wDown = Input.GetKey(KeyCode.W);
        aDown = Input.GetKey(KeyCode.A);
        sDown = Input.GetKey(KeyCode.S);
        dDown = Input.GetKey(KeyCode.D);
    }
}
