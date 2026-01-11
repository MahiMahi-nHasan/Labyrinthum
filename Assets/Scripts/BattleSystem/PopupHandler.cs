using UnityEngine;

public class PopupHandler : MonoBehaviour
{
    public static PopupHandler inst { get; private set; }
    public GameObject popupPrefab;

    private void Awake()
    {
        if (inst != null && inst != this)
        {
            Destroy(inst.gameObject);
            return;
        }
        inst = this;
    }

    public static void SpawnDamagePopup(BattleEntity entity, int damage, string tag, Color color)
    {
        if (inst == null || inst.popupPrefab == null)
        {
            Debug.LogWarning("PopupHandler not initialized or prefab missing.");
            return;
        }

        Vector3 pos = entity.transform.position + Vector3.up * 1.5f;
        GameObject obj = Instantiate(inst.popupPrefab, pos, Quaternion.identity, inst.transform);
        DamagePopup dp = obj.GetComponent<DamagePopup>();
        if (dp != null)
            dp.Initialize(damage, tag, color);
    }
}