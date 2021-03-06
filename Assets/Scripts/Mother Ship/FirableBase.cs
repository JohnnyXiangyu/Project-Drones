using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class FirableBase : MonoBehaviour {
    bool weaponEnabled = false;
    
    protected virtual void Update() {
        if (weaponEnabled) {
            // handle clicking
            if (Input.GetMouseButtonDown(0)) {
                if (EventSystem.current.IsPointerOverGameObject()) return;

                Camera cam = CameraRegister.cam;

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit)) {
                    Vector3 point = hit.point;
                    GameObject obj = hit.collider.gameObject;

                    PrimaryFire(point, obj);

                    Debug.Log("Weapon System: primary fire activated to " + hit.point.ToString());
                }
                else {
                    Debug.Log("Weapon System: primary fire failed to get target");
                }
            }
            else if (Input.GetMouseButtonDown(1)) {
                Camera cam = CameraRegister.cam;

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit)) {
                    Vector3 point = hit.point;
                    GameObject obj = hit.collider.gameObject;

                    SecondaryFire(point, obj);
                }
            }
        }
    }


    /// <summary>
    /// Prepare for enabling.
    /// </summary>
    public virtual void Enable() {
        weaponEnabled = true;
    }


    /// <summary>
    /// Prepare for disabling.
    /// </summary>
    public virtual void Disable() {
        weaponEnabled = false;
    }


    /// <summary>
    /// Behavior associated with mouse left click.
    /// </summary>
    protected abstract void PrimaryFire(Vector3 point, GameObject obj);


    /// <summary>
    /// Behavior associated with mouse right click.
    /// </summary>
    protected abstract void SecondaryFire(Vector3 point, GameObject obj);
}
