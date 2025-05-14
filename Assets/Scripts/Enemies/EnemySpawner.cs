using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int enemiesToSpawn = 5;
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Transform point = spawnPoints[i % spawnPoints.Length];
            GameObject enemy = EnemyPool.Instance.GetEnemy();
            enemy.transform.position = point.position;

            EnemyController controller = enemy.GetComponent<EnemyController>();
            controller.Initialize();
        }
    }
}