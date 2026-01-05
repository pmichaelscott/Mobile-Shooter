using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] private float speed = 3.5f;

    void Update()
    {
        transform.position += Vector3.back * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bullet>() != null)
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
            return;
        }

        // If zombie touches a soldier, player loses immediately
        if (other.GetComponentInParent<SoldierMarker>() != null)
        {
            GameManager.Instance.Lose();
        }
    }
}
