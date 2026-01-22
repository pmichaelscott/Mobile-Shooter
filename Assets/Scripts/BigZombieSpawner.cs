using System;
using System.Security.Cryptography;
using UnityEditor.EditorTools;
using UnityEngine;

public class BigZombieSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bigZombiePrefab;

    [Header("Spawn Timing")]
    [SerializeField] private int maxAlive = 2;
    [SerializeField] private float spawnEverySeconds = 10f;
    
    [Header("Initial Delay")]
    [SerializeField] private float initialDelaySeconds = 5f;

    [Header("Spawn Area")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX =  8f;
    [SerializeField] private float y = 0.5f;

    [Header("Hit Range")]
    [Tooltip("Over how many seconds to ramp up from minHits to maxHits")]
    [SerializeField] private float rampDurationSeconds = 180f;
    [SerializeField] private int startMinHits = 30;
    [SerializeField] private int startMaxHits = 70;
    [SerializeField] private int endMinHits = 30;
    [SerializeField] private int endMaxHits = 70;
    [SerializeField] private int hitIncrement = 15;

    [Tooltip("Shape the ramp (0..1 input time)")]
    [SerializeField] private AnimationCurve rampCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

[Tooltip("Size Range")]
    [SerializeField] private Vector3 startSizeRange = new Vector3(1f, 3f);
    [SerializeField] private Vector3 endSizeRange = new Vector3(1f, 3f);

    private float _spawnTimer;
    private float _delayTimer;
    private float _rampTime;
    [SerializeField]private float _sizeModifier;

    private void Start()
    {
        _delayTimer = initialDelaySeconds;
        _spawnTimer = spawnEverySeconds;
        _rampTime = 0f;
        _sizeModifier = 1f;
    }

    private void OnValidate()
    {
        startMaxHits = Mathf.Max(startMinHits, startMaxHits);
        endMaxHits = Mathf.Max(endMinHits, endMaxHits); 
    }

    void Update()
    {
        if (_delayTimer >0f)
        {
            _delayTimer -= Time.deltaTime;
            return;
        }

        _rampTime += Time.deltaTime;

        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0f)
        {
            TrySpawn();
            _spawnTimer = spawnEverySeconds;
        }
    }

   private void TrySpawn()
    {
        if(CountAliveBigZombies() >= maxAlive)
            return;

        float x = UnityEngine.Random.Range(minX, maxX);
        Vector3 pos = new Vector3(x,y, transform.position.z);

        var go = Instantiate(bigZombiePrefab, pos, Quaternion.identity);
        go.transform.localScale = Vector3.one * _sizeModifier;

        GetCurrentHitRange(out int minHitsNow, out int maxHitsNow);
        int hits = RollHits(minHitsNow, maxHitsNow, hitIncrement);

        var big = go.GetComponent<BigZombie>();
        if (big != null)
            big.InitializeHits(hits);
        else
            Debug.LogError("BigZombieSpawner: prefab missing BigZombie component");
    }

    private void GetCurrentHitRange(out int minHitsNow, out int maxHitsNow)
    {
        float t01 = Mathf.Clamp01(_rampTime/rampDurationSeconds);
        float shaped = Mathf.Clamp01(rampCurve.Evaluate(t01));

        // Lerp the ranges
        float minF = Mathf.Lerp(startMinHits, endMinHits, shaped);
        float maxF = Mathf.Lerp(startMaxHits, endMaxHits, shaped);

        // Snap to increment
        minHitsNow = SnapToIncrement(Mathf.RoundToInt(minF), hitIncrement);
        maxHitsNow = SnapToIncrement(Mathf.RoundToInt(maxF), hitIncrement);

        if (maxHitsNow < minHitsNow)
            maxHitsNow = minHitsNow;

        _sizeModifier = Mathf.Lerp(startSizeRange.y, endSizeRange.y, shaped);
    }

    private static int SnapToIncrement(int value, int step)
    {
        step = Mathf.Max(1, step);

        // Round to the nearest multiple of step
        return Mathf.Max(step, Mathf.RoundToInt(value/(float)step) * step);
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

        int k = UnityEngine.Random.Range(0, steps + 1); // inclusive upper by using +1
        int value = min + k * step;

        // Safety clamp in case of weird inputs
        return Mathf.Clamp(value, min, max);
    }
}
