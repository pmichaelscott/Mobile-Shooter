using UnityEngine;

public class DespawnZone : MonoBehaviour
{
    [SerializeField] private bool loseIfZombieEnters = false;

    private void OnTriggerEnter(Collider other)
    {
        // End game if a zombie crosses this line
        if (loseIfZombieEnters && (other.GetComponentInParent<Zombie>() != null ||
            other.GetComponentInParent<BigZombie>() != null))
        {
            GameManager.Instance.Lose();
            return;
        }

        // Return pooled objects to their pool
        var pooled = other.GetComponentInParent<PooledObject>();
        if (pooled != null && pooled.Pool != null)
        {
            pooled.Pool.Release(pooled.gameObject);
            return;
        }

        Destroy(other.gameObject);
    }
}
