using UnityEngine;

public class MiniPlayer : MonoBehaviour, IUpdatable
{
    [SerializeField]public float followSpeed;
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
        transform.position = Vector3.MoveTowards(transform.position, target.position, followSpeed * deltaTime);
    }

    public void ReturnToPool()
    {
        
    }
}
