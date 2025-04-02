using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bullet;
    public float bulletSpeed = 10.0f;
    public float fireRate = 0.1f;
    public Transform playerPostion; // needs to know so that it can tell if in range
    public float range = 10; // fire range
    public float nextTimeToFire = 0f;
    public bool inRange = false;

    void Update()
    {
        if((transform.position - playerPostion.position).magnitude <= range)
        {
            inRange = true;
        }
        else
        {
            inRange = false; 
        }
        if (inRange && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Fire();
        }
    }


    private void Fire()
    {
        GameObject _bullet = Instantiate(bullet, firePoint.position, bullet.transform.rotation);
        Rigidbody bulletRB = _bullet.GetComponent<Rigidbody>();

        bulletRB.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
