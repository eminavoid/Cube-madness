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
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ; // Prevent rotation in Y and Z
            rb.isKinematic = false;
        }
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
        DotController dotController = FindAnyObjectByType<DotController>();
        gameObject.SetActive(false);
        if (pool != null)
        {
            pool.ReturnObject(this);
            dotController.numberOfMiniPlayers--;
        }
    }
}