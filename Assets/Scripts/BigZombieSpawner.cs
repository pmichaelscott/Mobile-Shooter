using UnityEngine;

public class BigZombieSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bigZombiePrefab;
    [SerializeField] private float spawnEverySeconds = 10f;
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX =  8f;
    [SerializeField] private int maxAlive = 2;

    private float _t;

    void Update()
    {
        _t -= Time.deltaTime;
        if (_t <= 0f)
        {
            Spawn();
            _t = spawnEverySeconds;
        }
    }

    private void Spawn()
    {
        // Prevents us from spawning too many big zombies at once
        if (FindObjectsByType<BigZombie>(FindObjectsSortMode.None).Length >= maxAlive)
            return;

        float x = Random.Range(minX, maxX);
        Vector3 pos = new Vector3(x, 2.0f, transform.position.z);
        Instantiate(bigZombiePrefab, pos, Quaternion.identity);
    }
}
