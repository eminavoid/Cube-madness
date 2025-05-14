using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int initialSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    public static EnemyPool Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        for (int i = 0; i < initialSize; i++)
            AddEnemyToPool();
    }

    private void AddEnemyToPool()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform);
        enemy.SetActive(false);
        pool.Enqueue(enemy);
    }

    public GameObject GetEnemy()
    {
        if (pool.Count == 0)
            AddEnemyToPool();

        GameObject enemy = pool.Dequeue();
        enemy.SetActive(true);
        return enemy;
    }

    public void ReturnEnemy(EnemyController enemy)
    {
        if (pool != null)
        {
            enemy.gameObject.SetActive(false);
            pool.Enqueue(enemy.gameObject);
        }
    }
}