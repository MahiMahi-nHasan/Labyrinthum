using UnityEngine;

public class OverworldPlayer : OverworldEntity
{
    public AudioSource audioSource;
    public AudioClip start;
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered enemy hitbox");
        audioSource.Play();
        BattleRunner.active.StartBattle(party, other.transform.parent.GetComponent<OverworldEntity>().party);
        
    }
}