using UnityEngine;

public class SoldierPickup : MonoBehaviour
{
    [SerializeField] private bool hasBeenTriggered = false;
    private PooledObject _pooled;

    private void Awake()
    {
        _pooled = GetComponent<PooledObject>();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Soldier") == false) return;
        if (hasBeenTriggered) return;
        hasBeenTriggered = true;

        // Debug.Log("SoldierPickup triggered by " + other.name);
        var squad = other.GetComponentInParent<SquadController>();
        if (squad == null) return;

        squad.GetComponent<SquadSoldierAdder>()?.AddSoldier();
        ReturnToPool();
    }

private void OnEnable()
{
    hasBeenTriggered = false;
}
    private void ReturnToPool()
    {
    if (_pooled == null || _pooled.Pool == null)
        {
            Debug.LogError($"{name} has no pool reference. Did you forget PooledObject on the prefab?");
            gameObject.SetActive(false);
            return;
        }
        _pooled.Pool.Release(gameObject);
    }
}

