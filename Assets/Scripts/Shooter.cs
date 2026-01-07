using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private ObjectPool bulletPool;   
    [SerializeField] private float shotsPerSecond = 6f;
    [SerializeField] private Transform muzzle;

    private float _cooldown;

    public void SetBulletPool(ObjectPool pool)
    {
        bulletPool = pool;
    }

    void Update()
    {
        if (bulletPool == null) return; // safety if not assigned yet

        _cooldown -= Time.deltaTime;
        if (_cooldown <= 0f)
        {
            Fire();
            _cooldown = 1f / shotsPerSecond;
        }
    }

    private void Fire()
    {
        Vector3 spawnPos = muzzle ? muzzle.position : transform.position + Vector3.forward * 0.6f;
        bulletPool.Get(spawnPos, Quaternion.identity);
    }
}
