using UnityEngine;

public class PickupMover : MonoBehaviour
{

    [SerializeField] private float speed = 3.5f;

    void Update()
    {
        transform.position += Vector3.back * speed * Time.deltaTime;
    }
}
