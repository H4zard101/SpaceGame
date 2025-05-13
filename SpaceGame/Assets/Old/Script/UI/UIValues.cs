using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIValues : MonoBehaviour
{
    public SpaceShip spaceShip;
    public Target shipTarget;

    public TMP_Text shieldValue;
    public TMP_Text healthValue;
    public TMP_Text boostValue;
    public TMP_Text speedValue;
    public GameObject playerShip;

    public TMP_Text numberOfEnemies;


    public List<GameObject> enemies = new List<GameObject>();


    public void Awake()
    {
        UpdateEnemies();
    }
    void Update()
    {
        UpdateUIValues();
    }

    void UpdateUIValues()
    {
        if (playerShip != null)
        {
            shieldValue.text = "Shield: " + shipTarget.shield.ToString();
            healthValue.text = "Health: " + shipTarget.health.ToString();

            // GET RID OF THE DECIMAL PLACES
            float newBoost = Mathf.Round(spaceShip.currentBoostAmount * Mathf.Pow(10, 0)) / Mathf.Pow(10, 0);
            boostValue.text = "Boost: " + newBoost.ToString();


            // GET RID OF THE DECIMAL PLACES
            float newspeed = Mathf.Round((playerShip.GetComponent<Rigidbody>().velocity.z * 100) * Mathf.Pow(10, 0)) / Mathf.Pow(10, 0);
            speedValue.text = "Speed: " + newspeed.ToString();

            numberOfEnemies.text = "Enemies Left: " + enemies.Count.ToString();
        }
        else
        {
            return;
        }

    }

    public void UpdateEnemies()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemies.Add(enemy);
        }
    }

}
