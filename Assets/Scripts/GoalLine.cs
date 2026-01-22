using UnityEngine;

public class GoalLine : MonoBehaviour
{
    [SerializeField] private SquadSoldierAdder squad;

    private void OnTriggerEnter(Collider other)
    {
        if (squad == null) return;

        // Normal zombie: sprint to nearest soldier
        var zombie = other.GetComponentInParent<Zombie>();
        if (zombie != null)
        {
            zombie.TriggerSprintAttack(squad);
            return;
        }

        // Big zombie: leap + stomp squad
        var big = other.GetComponentInParent<BigZombie>();
        if (big != null)
        {
            big.TriggerStomp(squad);
            return;
        }
    }
}
