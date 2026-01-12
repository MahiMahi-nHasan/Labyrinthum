using UnityEngine.SceneManagement;

public class DemonKingEnemy : BattleNPC
{
    public override void OnDeath()
    {
        SceneManager.LoadScene("WinScreen");
    }
}