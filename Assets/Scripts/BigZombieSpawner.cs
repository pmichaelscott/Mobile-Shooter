using UnityEngine;

public class BigZombieSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bigZombiePrefab;
    [Header("Spawn Timing")]
    [SerializeField] private int maxAlive = 2;
    [SerializeField] private float spawnEverySeconds = 10f;
    
    [Header("Spawn Area")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX =  8f;

    [Header("Hit Range")]
    [SerializeField] private int minHits = 30;
    [SerializeField] private int maxHits = 70;
    [SerializeField] private int hitIncrement = 15;

    private float _t;


    private void OnValidate()
    {
        minHits = Mathf.Max(1, minHits);
        maxHits = Mathf.Max(minHits, maxHits);
        hitIncrement = Mathf.Max(1, hitIncrement);
    }

    void Update()
    {
        _t -= Time.deltaTime;
        if (_t <= 0f)
        {
            TrySpawn();
            _t = spawnEverySeconds;
        }
    }

   private void TrySpawn()
    {
        if (CountAliveBigZombies() >= maxAlive)
            return;

        float x = Random.Range(minX, maxX);
        Vector3 pos = new Vector3(x, transform.position.y, transform.position.z);

        var go = Instantiate(bigZombiePrefab, pos, Quaternion.identity);

        int hits = RollHits(minHits, maxHits, hitIncrement);

        var big = go.GetComponent<BigZombie>();
        if (big != null)
            big.InitializeHits(hits);
        else
            Debug.LogError("BigZombieSpawner: spawned prefab is missing BigZombie component.");
    }

    private int CountAliveBigZombies()
    {
        // Fine for "a couple at a time". If you ever scale up, track a list instead.
        return FindObjectsByType<BigZombie>(FindObjectsSortMode.None).Length;
    }

    private static int RollHits(int min, int max, int step)
    {
        min = Mathf.Max(1, min);
        max = Mathf.Max(min, max);
        step = Mathf.Max(1, step);

        // How many step-values are available between min..max (inclusive)?
        int range = max - min;
        int steps = range / step; // integer count of step intervals

        int k = Random.Range(0, steps + 1); // inclusive upper by using +1
        int value = min + k * step;

        // Safety clamp in case of weird inputs
        return Mathf.Clamp(value, min, max);
    }
}
