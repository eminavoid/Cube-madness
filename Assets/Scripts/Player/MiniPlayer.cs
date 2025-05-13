using UnityEngine;

public class MiniPlayer : MonoBehaviour, IUpdatable
{
    public float followSpeed = 5f;
    private Transform target;
    private MiniPlayerPool pool;

    public void Initialize(Transform targetTransform, MiniPlayerPool ownerPool)
    {
        target = targetTransform;
        pool = ownerPool;
        gameObject.SetActive(true);
    }

    public void Tick(float deltaTime)
    {
        if (target == null)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, followSpeed * deltaTime);
    }

    public void ReturnToPool()
    {
        gameObject.SetActive(false);
        if (pool != null)
        {
            pool.ReturnObject(this);
        }
    }
}
