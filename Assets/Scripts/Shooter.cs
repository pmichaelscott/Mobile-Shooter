using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shotsPerSecond = 6f;
    [SerializeField] private Transform muzzle; // optional

    private float _cooldown;

    void Update()
    {
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
        Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
    }
}
