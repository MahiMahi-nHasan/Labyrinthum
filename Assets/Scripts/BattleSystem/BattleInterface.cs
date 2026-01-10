using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInterface : MonoBehaviour
{
    public static BattleInterface active;

    BattleEntity selectedEntity;

    public List<BattleEntity> players = new();
    public List<BattleEntity> npcs = new();
    
    bool moveSelected = false;
    bool targetSelected = false;
    public bool targeting = false;
    public GameObject actionMenu;
    public bool showActionMenu = true;

    void Awake()
    {
        active = this;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    void Update()
    {
        actionMenu.SetActive(showActionMenu);
    }

    public void SelectMove(BattleEntity.Move move)
    {
        moveSelected = true;
        selectedEntity.SelectMove(move);
    }

    public void SelectAttack() => StartCoroutine(Attack_Coroutine());
    public void SelectSpecial() => StartCoroutine(Special_Coroutine());
    public void SelectDefend() => SelectMove(BattleEntity.Move.DEFEND);
    public void SelectRecharge() => SelectMove(BattleEntity.Move.RECHARGE);

    public IEnumerator Attack_Coroutine()
    {
        yield return StartCoroutine(SetTarget());
        SelectMove(BattleEntity.Move.ATTACK);
    }

    public IEnumerator Special_Coroutine()
    {
        Special s = selectedEntity.chosenSpecial;

        if (s == null)
        {
            Debug.LogError("No special selected!");
            yield break;
        }

        switch (s.targetingType)
        {
            case Special.TargetingType.SingleEnemy:
            case Special.TargetingType.AllEnemies:
            case Special.TargetingType.SingleAlly:
                yield return StartCoroutine(SetTarget());
                break;
            case Special.TargetingType.AllAllies:
                selectedEntity.state.target = null;
                break;

            case Special.TargetingType.Self:
                selectedEntity.state.target = selectedEntity;
                break;
        }

        SelectMove(BattleEntity.Move.SPECIAL);
    }

    public void SelectTarget(BattleEntity e)
    {
        targetSelected = true;
        selectedEntity.state.target = e;
        Debug.Log("SelectTarget fired on: " + e.name);
    }

    public IEnumerator SetTarget()
    {
        targeting = true;
        targetSelected = false;

        Debug.Log("Waiting for target to be selected");

        while (!targetSelected)
            yield return new WaitForEndOfFrame();

        Debug.Log("Target selected!");
        targeting = false;
    }

    public IEnumerator SetMoves(System.Action onComplete)
    {
        Debug.Log("Setting moves for all entities");

        showActionMenu = true;

        for (int i = 0; i < players.Count; i++)
        {
            selectedEntity = players[i];

            if (selectedEntity.state.dead) continue;

            Debug.Log("Selecting move for " + selectedEntity.baseEntity.entityName);
            moveSelected = false;

            while (!moveSelected)
                yield return new WaitForEndOfFrame();

            BattleNPC.UpdateMoveSelectionMatrix(selectedEntity);
        }

        showActionMenu = false;

        for (int i = 0; i < npcs.Count; i++)
        {
            if (selectedEntity.state.dead) continue;

            selectedEntity = npcs[i];

            Debug.Log("Setting move for " + selectedEntity.baseEntity.entityName);

            BattleEntity.Move move = ((BattleNPC)selectedEntity).GetDecidedMove();
            SelectMove(move);

            if (move == BattleEntity.Move.ATTACK)
            {
                SelectTarget(players[Random.Range(0, players.Count)]);
            } else if (move == BattleEntity.Move.SPECIAL)
            {
                Special s = selectedEntity.specials[Random.Range(0, selectedEntity.specials.Length)];
                selectedEntity.chosenSpecial = s;
                switch(s.targetingType) 
                {

                    case Special.TargetingType.AllEnemies:
                    case Special.TargetingType.SingleEnemy:
                        SelectTarget(players[Random.Range(0, players.Count)]);
                        break;

                    case Special.TargetingType.SingleAlly:
                        SelectTarget(npcs[Random.Range(0, npcs.Count)]);
                        break;

                    case Special.TargetingType.Self:
                        SelectTarget(selectedEntity);
                        break;
                    case Special.TargetingType.AllAllies:
                        // No target needed
                        selectedEntity.state.target = null;
                        break;

                }

            }
        }

        onComplete();
    }
}