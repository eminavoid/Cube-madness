using System;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int enemiesToSpawn = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnEnemies();
            gameObject.SetActive(false);
        }
    }

    private void SpawnEnemies()
    {
        float radius = .75f; // Distancia del centro a cada enemigo
        Vector3 center = transform.position;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // �ngulo en radianes para distribuir los enemigos en c�rculo
            float angle = i * Mathf.PI * 2 / enemiesToSpawn;

            // Calcular la posici�n radial
            Vector3 spawnPos = new Vector3(
                center.x + Mathf.Cos(angle) * radius,
                center.y,
                center.z + Mathf.Sin(angle) * radius
            );

            GameObject enemy = EnemyPool.Instance.GetEnemy();
            enemy.transform.position = spawnPos;
            enemy.transform.rotation = Quaternion.identity;
            
            // Resetear movimiento
            Rigidbody rb = enemy.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            EnemyController controller = enemy.GetComponent<EnemyController>();
            controller.Initialize();
        }
    }
}