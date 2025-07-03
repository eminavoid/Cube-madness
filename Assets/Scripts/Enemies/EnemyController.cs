using System;
using UnityEngine;

public class EnemyController : MonoBehaviour, IUpdatable
{
    private Transform player;
    private float visionRange = 10f;
    private float speed = 6f;
    private bool isChasing = false;
    private CustomUpdateManager updateManager;
    DotController dot;


    public void Initialize()
    {
        dot = FindAnyObjectByType<DotController>();
        player = dot.gameObject.transform;
        updateManager = FindAnyObjectByType<CustomUpdateManager>();
        updateManager.Register(this);
    }

    public void Tick(float deltaTime)
    {
        if (player == null) return;

        if (!isChasing && Vector3.Distance(transform.position, player.position) < visionRange)
        {
            isChasing = true;
        }

        if (isChasing)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            transform.position += dir * (speed * deltaTime);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out MiniPlayer miniPlayer))
        {
            EnemyPool.Instance.ReturnEnemy(this);
            if (dot != null)
            {
                dot.RemoveMiniPlayer(miniPlayer);
            }
        }
    }

    private void OnDisable()
    {
        updateManager?.Unregister(this);
    }
}