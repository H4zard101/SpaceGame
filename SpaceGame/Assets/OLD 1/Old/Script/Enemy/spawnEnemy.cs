using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnEnemy : MonoBehaviour
{
    public int numberOfEnemies = 1;
    public float spawnRadius = 10.0f;

    // this is the only ship so far, when others are made we can add them here
    public GameObject corvetteShip;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    private void OnEnable()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
            randomPosition += transform.position;

            Instantiate(corvetteShip, randomPosition, Quaternion.identity);

        }
    }
}
