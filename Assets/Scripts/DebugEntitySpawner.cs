using UnityEngine;

public class DebugEntitySpawner : MonoBehaviour
{
    public GameObject prefab;

    void Awake()
    {
        EntityManager.CreateEntity(prefab, transform.position, transform.rotation);
    }
}