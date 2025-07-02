using UnityEngine;

public abstract class BaseHazard : MonoBehaviour
{
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IHazardHandler handler))
        {
            handler.HandleHazard(this);
        }
    }
}

