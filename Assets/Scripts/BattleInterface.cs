using System.Collections;
using UnityEngine;

public class BattleInterface : MonoBehaviour
{
    // Needs to transfer data between scenes
    // Receive all entity objects somehow
    Entity selectedEntity;

    Entity[] players;
    bool moveSelected;

    BattleNPC[] npcs;

    public void SelectMove(Entity.Move move)
    {
        moveSelected = true;
        selectedEntity.SelectMove(move);
    }

    public void SelectAttack() => SelectMove(Entity.Move.ATTACK);
    public void SelectSpecial() => SelectMove(Entity.Move.SPECIAL);
    public void SelectDefend() => SelectMove(Entity.Move.DEFEND);
    public void SelectRecharge() => SelectMove(Entity.Move.RECHARGE);

    public IEnumerator SetMoves()
    {
        for (int i = 0; i < players.Length; i++)
        {
            selectedEntity = players[i];

            while (!moveSelected)
                yield return new WaitForEndOfFrame();

            BattleNPC.UpdateMoveSelectionMatrix(selectedEntity);
        }

        for (int i = 0; i < npcs.Length; i++)
        {
            selectedEntity = npcs[i];

            SelectMove(npcs[i].SelectMove());
        }
    }
}