using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] private float speed = 3.5f;

    private PooledObject _pooled;
    private bool _hitSomething;

    private void Awake()
    {
        _pooled = GetComponent<PooledObject>();
    }

    void Update()
    {
        transform.position += Vector3.back * speed * Time.deltaTime;
    }

   private void OnTriggerEnter(Collider other)
{
    if (_hitSomething) return;

    // Bullet hit: both go back to pools
    var bullet = other.GetComponent<Bullet>();
    if (bullet != null)
    {
        _hitSomething = true;
        bullet.ReturnToPool();
        ReturnToPool();
        return;
    }

    // Soldier hit: remove ONLY that soldier
    var soldier = other.GetComponentInParent<SoldierMarker>();
    if (soldier != null)
    {
        _hitSomething = true;

        var squad = soldier.GetComponentInParent<SquadSoldierAdder>();
        if (squad != null)
            squad.RemoveSoldier(soldier);

        ReturnToPool();
        return;
    }
}
private void OnEnable()
{
    _hitSomething = false;
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
