using System;
using UnityEngine;

public class EnemyController : MonoBehaviour, IUpdatable
{
    private Transform player;
    private float visionRange = 10f;
    private float speed = 5f;
    private bool isChasing = false;
    private CustomUpdateManager updateManager;
    private DotController dotController;
    
    
    public void Initialize()
    {
        player = GameObject.FindWithTag("Player").transform;
        dotController = FindAnyObjectByType<DotController>();
        isChasing = false;
        updateManager = FindAnyObjectByType<CustomUpdateManager>();
        updateManager.Register(this);
    }

    public void Tick(float deltaTime)
    {
        if (!isChasing)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < visionRange)
            {
                isChasing = true;
            }
        }

        if (isChasing)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            transform.position += dir * (speed * deltaTime);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("MiniPlayer"))
        {
            EnemyPool.Instance.ReturnEnemy(this.gameObject.GetComponent<EnemyController>());
            dotController.RemoveMiniPlayer(other.gameObject.GetComponent<MiniPlayer>());
        }
    }

    private void OnDisable()
    {
        if (updateManager != null)
        {
            updateManager.Unregister(this);
        }
    }
}