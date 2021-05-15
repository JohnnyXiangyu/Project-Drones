using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneCanon : MonoBehaviour
{
    public int maxAmmo;
    public float fireInterval;
    public float range;

    [SerializeField]
    GameObject bullet;

    int ammo;
    float nextFireTime = -1;
    GameObject target;

    public void Reload() {
        ammo = maxAmmo;
    }

    public void SetTarget(GameObject newTarget) {
        target = newTarget;
    }

    // Update is called once per frame
    void Update()
    {
        if (ammo <= 0) {
            GetComponent<DroneBase>().ReportDepletion();
        }
        else if ((transform.position - target.transform.position).magnitude <= range && target != null && Time.time >= nextFireTime) {
            // increment timer
            nextFireTime = Time.time + fireInterval;
            // use ammo
            ammo--;

            GameObject newbullet = Instantiate(bullet, transform.position, Quaternion.identity);
            newbullet.SetActive(false);
            newbullet.transform.LookAt(target.transform);
            newbullet.SetActive(true);
        }
    }
}
