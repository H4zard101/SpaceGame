using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 100;
    public float shield = 100;
    public Transform position; // use this to place the partical affects when hit;

    public ParticleSystem shieldParticle;
    public ParticleSystem healthParticle;

    public UIValues uiValues;


    public void Awake()
    {
        uiValues = GameObject.FindObjectOfType<UIValues>();
    }
    public void TakeDamage (float amount)
    {
        if(shield > 0 )
        {
            shield -= amount;
            shieldParticle.Play();
        }
        else
        {
            health -= amount;
            healthParticle.Play();
        }
        
        if (health < 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if(gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
            uiValues.enemies.Remove(this.gameObject);
        }
        if(gameObject.tag == "Player")
            Destroy(gameObject);
        if(gameObject.tag == "Asteroid")
            Destroy(gameObject); 
    }

}
