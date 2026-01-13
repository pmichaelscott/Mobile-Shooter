using TMPro;
using UnityEngine;

public class BigZombie : MonoBehaviour
{
    [SerializeField] private float speed = 2.0f;

    [Header("UI")]
    [SerializeField] private TextMeshPro hitsText;

    private int _hitsLeft;

    private void Update()
    {
        transform.position += Vector3.back * speed * Time.deltaTime;
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
}
