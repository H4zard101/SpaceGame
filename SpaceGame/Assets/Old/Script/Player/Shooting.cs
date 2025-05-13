using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bullet;
    public float bulletSpeed = 10.0f;
    public float fireRate = 0.1f;

    public float nextTimeToFire = 0f;

    public AudioClip shootingClip;
    public AudioSource shootingSource;

    public void Start()
    {
        shootingSource = GetComponent<AudioSource>();
        
    }

    void Update()
    {
        if (Input.GetMouseButton(0) &&Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Fire();
        }
    }

    private void Fire()
    {
        GameObject _bullet = Instantiate(bullet, firePoint.position, Quaternion.LookRotation(firePoint.forward));
        Rigidbody bulletRB = _bullet.GetComponent<Rigidbody>();

        bulletRB.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
        shootingSource.clip = shootingClip;
        shootingSource.Play();
        
    }
}
