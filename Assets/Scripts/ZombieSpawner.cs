using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private ObjectPool zombiePool;
    [SerializeField] private float zombiesPerSecond = 20f;
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX =  8f;

    private float _cooldown;

    void Update()
    {
        _cooldown -= Time.deltaTime;

        float interval = 1f / zombiesPerSecond;
        while (_cooldown <= 0f)
        {
            Spawn();
            _cooldown += interval;
        }
    }

    void Spawn()
    {
        float x = Random.Range(minX, maxX);
        Vector3 pos = new Vector3(x, 0.5f, transform.position.z);
        zombiePool.Get(pos, Quaternion.identity);
    }
}
