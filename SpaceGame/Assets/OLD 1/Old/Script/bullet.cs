using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public Rigidbody rb;
    public float bulletSpeed = 10.0f;
    public float damage = 10.0f;

    public AudioClip hitClip;
    public AudioSource hitSource;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        hitSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.forward * bulletSpeed * Time.deltaTime);
        StartCoroutine(bulletTravel());
    }

    public IEnumerator bulletTravel()
    {
        yield return new WaitForSeconds(3.0f);

        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Destroy(this.gameObject);
            collision.transform.GetComponent<Target>().TakeDamage(damage);
            hitSource.clip = hitClip;
            hitSource.Play();
        }
        if (collision.gameObject.tag == "Player")
        {
            Destroy(this.gameObject);
            collision.transform.GetComponent<Target>().TakeDamage(damage);
            hitSource.clip = hitClip;
            hitSource.Play();
        }
        if (collision.gameObject.tag == "Asteroid")
        {
            Destroy(this.gameObject);
            collision.transform.GetComponent<Target>().TakeDamage(damage);
            hitSource.clip = hitClip;
            hitSource.Play();

        }
        else
        {
            Destroy(this.gameObject);
        }

    }
}
