using UnityEngine;

public class OverworldPlayer : OverworldEntity
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered enemy hitbox");

        BattleRunner.active.StartBattle(party, other.transform.parent.GetComponent<OverworldEntity>().party);
    }
}