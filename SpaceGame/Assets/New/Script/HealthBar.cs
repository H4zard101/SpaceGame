using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarSprite;
    [SerializeField] private Image shieldBarSprite;
    [SerializeField] private Canvas canvas;

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        healthBarSprite.fillAmount = currentHealth / maxHealth;

    }
    public void UpdateShieldhBar(float maxShield, float currentShield)
    {
        shieldBarSprite .fillAmount = currentShield / maxShield;
    }

    public void Start()
    {
        canvas.gameObject.SetActive(false);
    }
}
