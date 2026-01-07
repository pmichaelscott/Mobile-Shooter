using UnityEngine;

public class SquadSoldierAdder : MonoBehaviour
{
    [SerializeField] private GameObject soldierPrefab;
    [SerializeField] private ObjectPool bulletPool;   // assign in inspector
    [SerializeField] private float spacing = 1.0f;

    private void Awake()
    {
        // Wire up any starting soldiers already in the hierarchy
        foreach (var shooter in GetComponentsInChildren<Shooter>(includeInactive: true))
            shooter.SetBulletPool(bulletPool);

        LayoutAll();
    }

    public int SoldierCount => GetComponentsInChildren<SoldierMarker>().Length;

    public void AddSoldier()
    {
        var soldierGO = Instantiate(soldierPrefab, transform);

        var shooter = soldierGO.GetComponentInChildren<Shooter>();
        if (shooter != null)
            shooter.SetBulletPool(bulletPool);

        LayoutAll();
    }

    public void RemoveSoldier(SoldierMarker soldier)
    {
        if (soldier == null) return;

        Destroy(soldier.gameObject); // removes only that soldier
        // Re-layout after the object is actually removed
        Invoke(nameof(LayoutAllAndCheckGameOver), 0f);
    }

    private void LayoutAllAndCheckGameOver()
    {
        LayoutAll();

        if (SoldierCount <= 0)
            GameManager.Instance.Lose();
    }

    private void LayoutAll()
    {
        var soldiers = GetComponentsInChildren<SoldierMarker>();
        int count = soldiers.Length;
        float offset = (count - 1) * spacing * 0.5f;

        for (int i = 0; i < count; i++)
            soldiers[i].transform.localPosition = new Vector3(i * spacing - offset, 0f, 0f);
    }
}
