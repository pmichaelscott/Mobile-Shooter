using UnityEngine;

public class SquadSoldierAdder : MonoBehaviour
{
    [SerializeField] private GameObject soldierPrefab;
    [SerializeField] private float spacing = 1.0f;

    public void AddSoldier()
    {
        int count = GetComponentsInChildren<SoldierMarker>().Length;
        // Place them centered: -1, 0, +1 style
        float offset = (count - 1) * spacing * 0.5f;

        var soldier = Instantiate(soldierPrefab, transform);
        LayoutAll();
    }

    private void LayoutAll()
    {
        var soldiers = GetComponentsInChildren<SoldierMarker>();
        int count = soldiers.Length;
        float offset = (count - 1) * spacing * 0.5f;

        for (int i = 0; i < count; i++)
        {
            Transform t = soldiers[i].transform;
            t.localPosition = new Vector3(i * spacing - offset, 1f, 0f);
        }
    }
}
