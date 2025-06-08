using static UnityEngine.Rendering.DebugUI.Table;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour
{
    public float health = 100;

    public float shield = 100;
    public float maxShield = 100f;
    public float shieldRegenRate = 10f; // units per second
    public float shieldRegenDelay = 5f; // seconds after taking damage

    public Transform position;

    public ParticleSystem shieldParticle;
    public ParticleSystem healthParticle;



    private Coroutine regenCoroutine;
    private float lastDamageTime;
    private bool isRegenerating = false;

    public HealthBar healthBar;

    private void Start()
    {
        healthBar.UpdateHealthBar(health, health);
        healthBar.UpdateShieldhBar(maxShield, shield);
    }
    public void TakeDamage(float amount)
    {
        lastDamageTime = Time.time;

        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            isRegenerating = false;
        }

        if (shield > 0)
        {
            shield -= amount;
            healthBar.UpdateShieldhBar(maxShield, shield);
            shieldParticle.Play();

            if (shield < 0)
            {
                health += shield; // Apply overflow damage to health
                shield = 0;
                healthParticle.Play();
                healthBar.UpdateHealthBar(health, health);
            }
        }
        else
        {
            health -= amount;
            healthBar.UpdateHealthBar(health, health);
            healthParticle.Play();
        }

        if (health < 0f)
        {
            healthBar.UpdateHealthBar(health, health);
            Die();
        }
        else if (!isRegenerating)
        {
            regenCoroutine = StartCoroutine(RegenerateShield());
        }
    }

    IEnumerator RegenerateShield()
    {
        isRegenerating = true;

        // Wait until X amount of seconds after last hit
        yield return new WaitForSeconds(shieldRegenDelay);

        while (Time.time - lastDamageTime < shieldRegenDelay)
            yield return null;

        while (shield < maxShield)
        {
            shield += shieldRegenRate * Time.deltaTime;
            healthBar.UpdateShieldhBar(maxShield, shield);
            shield = Mathf.Min(shield, maxShield);
      
            yield return null;
        }
        shield = Mathf.Round(shield * 10f) / 10f;
        isRegenerating = false;
    }

    void Die()
    {
        if (gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
            
        }

        if (gameObject.tag == "Player")
            Destroy(gameObject);

        if (gameObject.tag == "Asteroid")
            Destroy(gameObject);
    }
}
