using TMPro;
using UnityEngine;
using System.Collections;

public class BigZombie : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 2.0f;
    [SerializeField] private float speedPerSoldier = 0.03f;

    [Header("UI")]
    [SerializeField] private TextMeshPro hitsText;

    [Header("Stomp Attack")]
    [SerializeField] private float leapUpHeight = 4f;
    [SerializeField] private float leapUpTime = 0.35f;
    [SerializeField] private float leapDownTime = 0.25f;

    private int _hitsLeft;
    private bool _isStomping;
    private SquadSoldierAdder _squad;
    private float _speedMultiplier = 1f;


    private void Update()
    {
        if (_isStomping) return;
        transform.position += Vector3.back * (baseSpeed * _speedMultiplier) * Time.deltaTime;
    }

    // Called by spawner right after Instantiate
    public void InitializeHits(int hits)
    {
        _hitsLeft = Mathf.Max(1, hits);
        UpdateText();
    }

    private void OnTriggerEnter(Collider other)
    {
        var bullet = other.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.ReturnToPool();
            TakeHit(1);
            return;
        }

        var soldier = other.GetComponentInParent<SoldierMarker>();
        if (soldier != null)
        {
            var squad = soldier.GetComponentInParent<SquadSoldierAdder>();
            if (squad != null)
                squad.RemoveSoldier(soldier);
        }
    }

    private void TakeHit(int amount)
    {
        _hitsLeft -= amount;
        if (_hitsLeft <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            UpdateText();
        }
    }

    private void UpdateText()
    {
        if (hitsText != null)
            hitsText.text = _hitsLeft.ToString();
    }

    public void TriggerStomp(SquadSoldierAdder squad)
    {
        if (_isStomping) return;
        _isStomping = true;
        _squad = squad;

        // Optional: stop collisions during leap
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        StartCoroutine(DoStomp());
    }

    private IEnumerator DoStomp()
    {
        // Stop normal movement while stomping
        Vector3 startPos = transform.position;

        // (Optional) rotate to face squad center
        Vector3 squadPos = _squad != null ? _squad.transform.position : startPos;
        Vector3 look = squadPos - startPos; look.y = 0f;
        if (look.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(look.normalized, Vector3.up);

        // Leap up
        float t = 0f;
        while (t < leapUpTime)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / leapUpTime);
            transform.position = new Vector3(startPos.x, startPos.y + Mathf.Lerp(0f, leapUpHeight, a), startPos.z);
            yield return null;
        }

        // Slam down (optionally snap X/Z to squad center for drama)
        Vector3 peakPos = transform.position;
        Vector3 landPos = new Vector3(squadPos.x, startPos.y, squadPos.z);

        t = 0f;
        while (t < leapDownTime)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / leapDownTime);
            transform.position = Vector3.Lerp(peakPos, landPos, a);
            yield return null;
        }

        // STOMP: remove whole squad
        if (_squad != null)
            _squad.RemoveAllSoldiers();

        // Optional: camera shake / VFX hook goes here

        Destroy(gameObject);
    }

    private void OnEnable()
    {
        _squad = GameManager.Instance.Squad;
        if (_squad != null)
        {
            _squad.SoldierCountChanged += OnSoldierCountChanged;
            OnSoldierCountChanged(_squad.CurrentSoldierCount);
        }
    }

    private void OnDisable()
    {
        if (_squad != null)
            _squad.SoldierCountChanged -= OnSoldierCountChanged;
    }

    private void OnSoldierCountChanged(int count)
    {
        _speedMultiplier = 1f + count * speedPerSoldier;
    }

}


