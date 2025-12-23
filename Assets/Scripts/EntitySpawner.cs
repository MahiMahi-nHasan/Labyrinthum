using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    // Singleton system
    public static EntitySpawner instance;

    public GameObject[] preexistingEntities;

    void Awake()
    {
        #region Singleton code
        // Ensure only one instance of this object exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        #endregion

        foreach (GameObject e in preexistingEntities)
        {
            EntityManager.CreateEntity(e, e.transform.position, e.transform.rotation);
            Destroy(e);
        }
    }
}