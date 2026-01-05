using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pickupPrefab;
    [SerializeField] private float pickupsPerSecond = 20f;
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX =  8f;

    private float _cooldown;

    void Update()
    {
        _cooldown -= Time.deltaTime;

        float interval = 1f / pickupsPerSecond;
        while (_cooldown <= 0f)
        {
            Spawn();
            _cooldown += interval;
        }
    }

    void Spawn()
    {
        Vector3 pos = new Vector3(transform.position.x, 1f, transform.position.z);
        Instantiate(pickupPrefab, pos, Quaternion.identity);
    }
}
