using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public Rigidbody rb;
    public float bulletSpeed = 10.0f;
    public float damage = 10.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            collision.transform.GetComponent<Target>().TakeDamage(damage);
        }
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.GetComponent<Target>().TakeDamage(damage);
        }

    }
}
