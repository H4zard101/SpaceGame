using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidsGenerator : MonoBehaviour
{
    public GameObject asteroid;
    public int numberOfAsteroids = 10;
    public float spawnRadius = 50f;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

    }

    private void OnEnable()
    {
        SpawnItems();
    }

    void SpawnItems()
    {
        for (int i = 0; i < numberOfAsteroids; i++) 
        {
            Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
            randomPosition += transform.position;

            Instantiate(asteroid, randomPosition, Quaternion.identity);
            
        }
    }
}
