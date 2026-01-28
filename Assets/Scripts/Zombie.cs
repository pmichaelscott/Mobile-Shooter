using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 3.5f;
    [SerializeField] private float speedPerSoldier = 0.05f;
    [SerializeField] private float sprintSpeed = 10f;
    
    [Tooltip("degrees/sec")]
    [SerializeField] private float turnSpeed = 720f; // degrees/sec

    private PooledObject _pooled;
    private bool _hitSomething;

    // Used for the sprint attack toward a soldier
    private bool _isSprinting;
    private SoldierMarker _targetSoldier;
    private SquadSoldierAdder _targetSquad;
    [SerializeField] private float _speedMultiplier = 1f;
    private SquadSoldierAdder _squad;

    private void Awake()
    {
        _pooled = GetComponent<PooledObject>();
    }

    void Update()
    {
     if (_isSprinting)
        {
            SprintTowardsTarget();
            return;
        }

        transform.rotation = Quaternion.RotateTowards(
        transform.rotation,
        Quaternion.LookRotation(Vector3.back, Vector3.up),
        turnSpeed * Time.deltaTime
);

        transform.position += Vector3.back * (baseSpeed * _speedMultiplier) * Time.deltaTime;
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

            // Zombie has expended itself after taking a soldier
            ReturnToPool();
            return;
        }
    }
    private void OnEnable()
    {
        _hitSomething = false;
        _isSprinting = false;
        _targetSoldier = null;
        _targetSquad = null;
        
        HookSquadEvents();
    
    }


    private void OnDisable()
    {

        UnhookSquadEvents();

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

    public void TriggerSprintAttack(SquadSoldierAdder squad)
    {
        if (_isSprinting) return;

        _targetSquad = squad;
        _targetSoldier = squad.GetNearestSoldier(transform.position);

        if (_targetSoldier == null)
        {
            // The game manager is already handling the case when we're out of soldiers
            return;
        }

        _isSprinting = true;

        // Once sprinting, ignore "end line" re-triggers and don't despawn automatically
        _hitSomething = false;
    }

    private void SprintTowardsTarget()
    {
        
        if (_targetSoldier == null)
        {
            // Try reacquire
            if (_targetSquad != null)
            {
                _targetSoldier = _targetSquad.GetNearestSoldier(transform.position);   
            }
            else
            {
                ReturnToPool();
                return;
            }

            if (_targetSoldier == null)
            {
                // No soldiers left
                ReturnToPool();
                return;
            }
        }

        Vector3 toTarget = _targetSoldier.transform.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.01f)
            return;

        // Turn toward soldier
        Quaternion desired = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, desired, turnSpeed * Time.deltaTime);

        // Move quickly toward soldier
        transform.position += transform.forward * (sprintSpeed * _speedMultiplier) * Time.deltaTime;
    }

    private void OnSoldierCountChanged(int count)
    {
        _speedMultiplier = 1f + count * speedPerSoldier;
    }

    private void HookSquadEvents()
        {
            // Unhook first in case this zombie got reused weirdly
            UnhookSquadEvents();

            _squad = GameManager.Instance != null ? GameManager.Instance.Squad : null;

            // Fallback if GameManager hasn’t registered squad yet
            if (_squad == null)
                _squad = FindFirstObjectByType<SquadSoldierAdder>();

            if (_squad != null)
            {
                _squad.SoldierCountChanged += OnSoldierCountChanged;
                OnSoldierCountChanged(_squad.CurrentSoldierCount); // initialize immediately
            }
        }

    private void UnhookSquadEvents()
        {
            if (_squad != null)
                _squad.SoldierCountChanged -= OnSoldierCountChanged;

            _squad = null;
        }

}
