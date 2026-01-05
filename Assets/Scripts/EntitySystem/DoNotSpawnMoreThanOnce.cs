using UnityEngine;

public class DoNotSpawnMoreThanOnce : MonoBehaviour
{
    public static bool loadedBefore = false;

    void Awake()
    {
        if (loadedBefore)
            Destroy(gameObject);
        else
            loadedBefore = true;
    }
}