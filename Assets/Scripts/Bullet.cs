using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 3f;

    private float _lifeLeft;
    private PooledObject _pooled;

    private void Awake()
    {
        _pooled = GetComponent<PooledObject>();
    }

    private void OnEnable()
    {
        _lifeLeft = lifetime;
    }

    void Update()
    {
        transform.position += Vector3.forward * speed * Time.deltaTime;

        _lifeLeft -= Time.deltaTime;
        if (_lifeLeft <= 0f)
        {
            ReturnToPool();
        }
    }

    public void ReturnToPool()
    {
    if (_pooled == null || _pooled.Pool == null)
        {
            Debug.LogError($"{name} has no pool reference. Did you forget PooledObject on the prefab?");
            gameObject.SetActive(false);
            return;
        }
        _pooled.Pool.Release(gameObject);
    }
}
