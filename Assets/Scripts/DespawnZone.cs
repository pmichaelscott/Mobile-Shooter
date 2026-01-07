using UnityEngine;

public class DespawnZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Return pooled objects to their pool
        var pooled = other.GetComponentInParent<PooledObject>();
        if (pooled != null && pooled.Pool != null)
        {
            Debug.Log("DespawnZone returning " + other.name + " to pool.");
            pooled.Pool.Release(pooled.gameObject);
            return;
        }

        // Fallback: non-pooled objects get destroyed
        Destroy(other.gameObject);
    }
}
