using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bullet;
    public float bulletSpeed = 10.0f;
    public float fireRate = 0.1f;
    public GameObject playerPostion; // needs to know so that it can tell if in range
    public float range = 10; // fire range
    public float nextTimeToFire = 0f;
    public bool inRange = false;
    public FieldOfView fieldOfView;

    public AudioClip shootingClip;
    public AudioSource shootingSource;

    public void Start()
    {
        shootingSource = GetComponent<AudioSource>();

    }

    public void Awake()
    {
        playerPostion = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        if (GameStateManager.Instance.state == GameStateManager.gameState.inGame)
        {
            if (fieldOfView.canSeePlayer && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Fire();
            }
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
