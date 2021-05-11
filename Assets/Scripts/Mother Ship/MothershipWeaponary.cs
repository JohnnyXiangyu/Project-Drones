using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Weapon system for ship unit.
/// Both drone deployer and weapons are abstracted as IFireable.
/// </summary>
public class MothershipWeaponary : MonoBehaviour {
    [SerializeField]
    GameObject deployPoint;

    int currentWeapon = 0;
    List<FirableBase> weapons = new List<FirableBase>();

    private void Update() {
        int oldWeapon = 0;

        // switch weapons
        if (Input.GetAxis("Mouse ScrollWheel") > float.Epsilon) {
            Debug.Log("switch up");

            if (weapons.Count > 0) {
                currentWeapon++;
                currentWeapon %= weapons.Count;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < -float.Epsilon) {
            Debug.Log("switch down");

            if (weapons.Count > 0) {
                currentWeapon--;

                if (currentWeapon < 0) {
                    currentWeapon = weapons.Count - 1;
                }
            }
        }

        if (currentWeapon != oldWeapon) {
            weapons[oldWeapon].Disable();
            weapons[currentWeapon].Enable();
        }
    }
}
