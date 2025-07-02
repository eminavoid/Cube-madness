using UnityEngine;

public class MiniPlayer : MonoBehaviour, IUpdatable, IHazardHandler
{
    public float followSpeed = 5f;
    private Vector3 targetPosition;
    private MiniPlayerPool pool;
    private Rigidbody rb;
    DotController dot;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();
        rb = GetComponent<Rigidbody>();
        dot = FindAnyObjectByType<DotController>();
    }

    public void Initialize(MiniPlayerPool ownerPool)
    {
        pool = ownerPool;
        gameObject.SetActive(true);
    }

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
    }

    public void Tick(float deltaTime)
    {
        if (rb == null) return;

        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.linearVelocity = direction * followSpeed;
    }

    public void HandleHazard(BaseHazard hazard)
    {
        switch (hazard)
        {
            case Lava lava:
                if (dot != null)
                {
                    dot.RemoveMiniPlayer(this);
                }
                break;

            case MathWall mathWall:
                if (!mathWall.enabled || mathWall.gameObject == null || mathWall.gameObject.activeSelf == false)
                    break;

                if (!mathWall.HasActivated)
                {
                    mathWall.HasActivated = true;

                    if (dot != null)
                    {
                        dot.ApplyMathOperation(mathWall.operationType, mathWall.operationValue);
                    }

                    Transform wallParent = mathWall.transform.parent;
                    if (wallParent != null)
                    {
                        foreach (Transform child in wallParent)
                        {
                            child.gameObject.SetActive(false);
                        }
                    }
                }
                break;

            default:
                Debug.LogWarning("Unhandled hazard type");
                break;
        }
    }

    public void ReturnToPool()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        gameObject.SetActive(false);
        pool?.ReturnObject(this);
    }
}