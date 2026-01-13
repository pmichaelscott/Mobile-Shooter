using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int prewarmCount = 50;
    [SerializeField] private bool canGrow = true;

    private readonly Queue<GameObject> _inactive = new();

    private void Awake()
    {
        for (int i = 0; i < prewarmCount; i++)
        {
            var obj = CreateNew();
            Release(obj);
        }
    }

private GameObject CreateNew()
{
    var obj = Instantiate(prefab, transform);

    var po = obj.GetComponent<PooledObject>();
    if (po == null)
    {
        Debug.LogError($"Prefab {prefab.name} is missing PooledObject. Add it to the prefab.", prefab);
    }
    else
    {
        po.Pool = this;
    }

    obj.SetActive(false);
    return obj;
}

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject obj;

        if (_inactive.Count > 0)
        {
            obj = _inactive.Dequeue();
        }
        else
        {
            if (!canGrow) return null;
            obj = CreateNew();
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);

        
        var rb = obj.GetComponent<Rigidbody>();
        if (rb != null && rb.isKinematic == false)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            
        }

        return obj;
    }

    public void Release(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        _inactive.Enqueue(obj);
    }
}
