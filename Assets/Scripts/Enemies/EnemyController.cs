using UnityEngine;

public class EnemyController : MonoBehaviour, IUpdatable
{
    private Transform player;
    private float visionRange = 10f;
    private float speed = 2f;
    private bool isChasing = false;
    private CustomUpdateManager updateManager;
    
    
    public void Initialize()
    {
        player = GameObject.FindWithTag("Player").transform;
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

    private void OnDisable()
    {
        updateManager.Unregister(this);
    }
}