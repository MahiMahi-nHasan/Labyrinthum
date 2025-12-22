using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInterface : MonoBehaviour
{
    public static BattleInterface active;

    Entity selectedEntity;

    public List<Entity> players = new();
    public List<Entity> npcs = new();
    
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

    public void SelectMove(Entity.Move move)
    {
        moveSelected = true;
        selectedEntity.SelectMove(move);
    }

    public void SelectAttack() => StartCoroutine(Attack_Coroutine());
    public void SelectSpecial() => StartCoroutine(Special_Coroutine());
    public void SelectDefend() => SelectMove(Entity.Move.DEFEND);
    public void SelectRecharge() => SelectMove(Entity.Move.RECHARGE);

    public IEnumerator Attack_Coroutine()
    {
        yield return StartCoroutine(SetTarget());
        SelectMove(Entity.Move.ATTACK);
    }

    public IEnumerator Special_Coroutine()
    {
        yield return StartCoroutine(SetTarget());
        SelectMove(Entity.Move.SPECIAL);
    }

    public void SelectTarget(Entity e)
    {
        targetSelected = true;
        selectedEntity.state.target = e;
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

            Debug.Log("Selecting move for " + selectedEntity.entityName);
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

            Debug.Log("Setting move for " + selectedEntity.entityName);

            Entity.Move move = ((BattleNPC)selectedEntity).GetDecidedMove();
            SelectMove(move);

            if (move == Entity.Move.ATTACK || move == Entity.Move.SPECIAL)
            {
                SelectTarget(players[Random.Range(0, players.Count)]);
            }
        }

        onComplete();
    }
}