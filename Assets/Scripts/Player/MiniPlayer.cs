using UnityEngine;

public class MiniPlayer : MonoBehaviour, IUpdatable
{
    public float followSpeed = 5f;
    private Vector3 targetPosition; // Changed to Vector3
    private MiniPlayerPool pool;
    private Rigidbody rb;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();
    }

    public void Initialize(MiniPlayerPool ownerPool)
    {
        pool = ownerPool;
        gameObject.SetActive(true);
    }

    public void SetTarget(Vector3 target) // Changed parameter type to Vector3
    {
        targetPosition = target;
    }

    public void Tick(float deltaTime)
    {
        if (rb == null) return;

        // Move towards the target position
        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.linearVelocity = direction * followSpeed; // Using linearVelocity for Unity 6+
    }

    public void ReturnToPool()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        DotController dotController = FindAnyObjectByType<DotController>();
        gameObject.SetActive(false);
        if (pool != null)
        {
            pool.ReturnObject(this);
            if (dotController != null)
            {
                dotController.numberOfMiniPlayers--;
            }
        }
    }
}