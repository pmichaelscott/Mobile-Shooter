using UnityEngine;

public class SoldierPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("SoldierPickup triggered by " + other.name);
        var squad = other.GetComponentInParent<SquadController>();
        if (squad == null) return;

        squad.GetComponent<SquadSoldierAdder>()?.AddSoldier();
        Destroy(gameObject);
    }
}
