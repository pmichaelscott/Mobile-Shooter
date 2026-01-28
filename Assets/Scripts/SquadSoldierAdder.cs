using System;
using System.Collections.Generic;
using UnityEngine;

public class SquadSoldierAdder : MonoBehaviour
{
    [SerializeField] private GameObject soldierPrefab;
    [SerializeField] private ObjectPool bulletPool;

    [Header("Cluster Placement")]
    [SerializeField] private float clusterRadius = 1.2f;
    [SerializeField] private float minSeparation = 0.7f;
    [SerializeField] private int maxAttemptsPerSoldier = 40;

    public int CurrentSoldierCount => GetComponentsInChildren<SoldierMarker>().Length;

    [SerializeField] private float soldierHeight = 0f;

    private int _nextId = 1;

    // Stable positions per soldier instance
    private readonly Dictionary<int, Vector3> _positions = new();
    private readonly HashSet<int> _alive = new();

    private SoldierInstance _anchor; // the centered soldier
    public event Action<int> SoldierCountChanged;

    private void Awake()
    {
        // If you start with a soldier already placed under SquadRoot, make it the anchor.
        _anchor = GetComponentInChildren<SoldierInstance>(includeInactive: true);

        if (_anchor != null)
        {
            EnsureWired(_anchor.gameObject);

            _anchor.Id = 0;                    // anchor always id 0
            _positions[_anchor.Id] = Vector3.zero;
            _alive.Add(_anchor.Id);
            _anchor.transform.localPosition = Vector3.zero;
        }
        else
        {
            // If no starting soldier, create one as the anchor
            SpawnAnchor();
        }

        // Wire up any other pre-placed soldiers. I haven't yet seen a case for this, but leaving it here
        foreach (var s in GetComponentsInChildren<SoldierInstance>(includeInactive: true))
        {
            if (s == _anchor) continue;
            EnsureWired(s.gameObject);

            if (s.Id == 0 && s != _anchor) s.Id = _nextId++;
            if (!_positions.ContainsKey(s.Id))
            {
                _positions[s.Id] = FindNonOverlappingPoint();
            }
            _alive.Add(s.Id);
            s.transform.localPosition = _positions[s.Id];
        }
    }

    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RegisterSquad(this);
        else
            Debug.LogError("No GameManager in scene.", this);
    }

    private void SpawnAnchor()
    {
        var go = Instantiate(soldierPrefab, transform);
        EnsureWired(go);

        _anchor = go.GetComponent<SoldierInstance>();
        if (_anchor == null) _anchor = go.AddComponent<SoldierInstance>();

        _anchor.Id = 0;
        _positions[0] = Vector3.zero;
        _alive.Add(0);
        _anchor.transform.localPosition = Vector3.zero;
    }

    public int SoldierCount => GetComponentsInChildren<SoldierMarker>().Length;

    public void AddSoldier()
    {
        // Ensure anchor exists
        if (_anchor == null) SpawnAnchor();

        var go = Instantiate(soldierPrefab, transform);
        EnsureWired(go);

        var inst = go.GetComponent<SoldierInstance>();
        if (inst == null) inst = go.AddComponent<SoldierInstance>();

        inst.Id = _nextId++;
        _alive.Add(inst.Id);

        Vector3 pos = FindNonOverlappingPoint();
        _positions[inst.Id] = pos;
        inst.transform.localPosition = pos;

        NotifySoldierCountChanged();
    }

    public void RemoveSoldier(SoldierMarker soldierMarker)
    {
        if (soldierMarker == null) return;

        var inst = soldierMarker.GetComponent<SoldierInstance>();
        if (inst == null)
        {
            Destroy(soldierMarker.gameObject);
            Invoke(nameof(CheckGameOver), 0f);
            return;
        }


        // If anchor dies, promote another soldier to center WITHOUT moving everyone else.
        if (inst.Id == 0)
        {
            Destroy(inst.gameObject);
            _alive.Remove(0);
            _positions.Remove(0);

            PromoteNewAnchor();
            Invoke(nameof(CheckGameOver), 0f);
            return;
        }

        Destroy(inst.gameObject);
        _alive.Remove(inst.Id);
        _positions.Remove(inst.Id);

        Invoke(nameof(CheckGameOver), 0f);


    }

    private void PromoteNewAnchor()
    {
        // If anyone left, pick one and snap it to center (this moves only the promoted one)
        var remaining = GetComponentsInChildren<SoldierInstance>();
        if (remaining.Length == 0)
        {
            _anchor = null;
            return;
        }

        // Choose first remaining
        var newAnchor = remaining[0];

        // Remove its old position record
        _positions.Remove(newAnchor.Id);
        _alive.Remove(newAnchor.Id);

        // Make it anchor id 0 at center
        newAnchor.Id = 0;
        _anchor = newAnchor;

        _positions[0] = Vector3.zero;
        _alive.Add(0);
        _anchor.transform.localPosition = Vector3.zero;
    }

    private void CheckGameOver()
    {
        NotifySoldierCountChanged();
        if (GetComponentsInChildren<SoldierMarker>().Length <= 0)
            GameManager.Instance.Lose();
    }

    private void EnsureWired(GameObject soldierGO)
    {
        // Ensure bullet pool assigned to shooter
        var shooter = soldierGO.GetComponentInChildren<Shooter>();
        if (shooter != null)
            shooter.SetBulletPool(bulletPool);
    }

    private Vector3 FindNonOverlappingPoint()
    {
        // Build a list of currently used positions
        var used = new List<Vector3>(_positions.Values);

        for (int attempt = 0; attempt < maxAttemptsPerSoldier; attempt++)
        {
            Vector2 p2 = UnityEngine.Random.insideUnitCircle * clusterRadius;

            // Cluster around anchor at local (0,0,0)
            Vector3 candidate = new Vector3(p2.x, soldierHeight, p2.y);

            // Don't place on the exact center (reserve for anchor)
            if (candidate.sqrMagnitude < 0.0001f) continue;

            bool ok = true;
            for (int i = 0; i < used.Count; i++)
            {
                if (Vector3.Distance(candidate, used[i]) < minSeparation)
                {
                    ok = false;
                    break;
                }
            }

            if (ok) return candidate;
        }

        // Fallback if packed too tight
        Vector2 fallback = UnityEngine.Random.insideUnitCircle * clusterRadius;
        return new Vector3(fallback.x, soldierHeight, fallback.y);
    }
    public SoldierMarker GetNearestSoldier(Vector3 fromWorldPos)
    {
        var soldiers = GetComponentsInChildren<SoldierMarker>();
        if (soldiers.Length == 0) return null;

        SoldierMarker best = null;
        float bestDistSq = float.PositiveInfinity;

        for (int i = 0; i < soldiers.Length; i++)
        {
            float d = (soldiers[i].transform.position - fromWorldPos).sqrMagnitude;
            if (d < bestDistSq)
            {
                bestDistSq = d;
                best = soldiers[i];
            }
        }

        return best;
    }

    // Used by the big zombie to remove all soldiers when it stomps
    public void RemoveAllSoldiers()
    {
        var soldiers = GetComponentsInChildren<SoldierMarker>();
        for (int i = 0; i < soldiers.Length; i++)
        {
            Destroy(soldiers[i].gameObject);
        }
        NotifySoldierCountChanged();
        Invoke(nameof(CheckGameOver), 0f);

    }

    private void NotifySoldierCountChanged()
    {
        SoldierCountChanged?.Invoke(CurrentSoldierCount);
    }

}
