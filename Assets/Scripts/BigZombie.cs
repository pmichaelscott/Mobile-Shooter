using TMPro;
using UnityEngine;

public class BigZombie : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHits = 50;
    [SerializeField] private float speed = 2.5f;

    [Header("UI")]
    [SerializeField] private TextMeshPro counterText; // TextMeshPro (3D), not TMPUGUI
    [SerializeField] private Vector3 textOffset = new Vector3(0f, 1.5f, 0f);

    private int _hitsLeft;
    private bool _dead;

    private void Start()
    {
        _hitsLeft = maxHits;
        UpdateCounter();
    }

    private void Update()
    {
        if (_dead) return;

        // Same movement as regular zombie (down screen)
        transform.position += Vector3.back * speed * Time.deltaTime;

        // Keep the counter floating above the zombie
        if (counterText != null)
            counterText.transform.position = transform.position + textOffset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_dead) return;

        // Bullet hit
        var bullet = other.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.ReturnToPool();
            TakeHit(1);
            return;
        }

        // Soldier hit (same rule as your normal zombies)
        var soldier = other.GetComponentInParent<SoldierMarker>();
        if (soldier != null)
        {
            var squad = soldier.GetComponentInParent<SquadSoldierAdder>();
            if (squad != null)
                squad.RemoveSoldier(soldier);

            // Optional design choice:
            // If big zombie collides with a soldier, should it die or keep going?
            // If you want it to keep going, do nothing here.
            // If you want it removed on contact, call Die().
            return;
        }
    }

    private void TakeHit(int amount)
    {
        _hitsLeft -= amount;
        if (_hitsLeft <= 0)
        {
            Die();
        }
        else
        {
            UpdateCounter();
        }
    }

    private void UpdateCounter()
    {
        if (counterText != null)
            counterText.text = _hitsLeft.ToString();
    }

    private void Die()
    {
        _dead = true;
        Destroy(gameObject);
    }
}
