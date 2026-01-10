using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

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

    //specials menu
    public GameObject specialsMenu;
    public Transform specialsButtonContainer;
    public GameObject specialButtonPrefab;
    public GameObject cancelButton;

    void Awake()
    {
        active = this;
        specialsMenu.SetActive(false);
        cancelButton.SetActive(false);
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

    public void OpenSpecialMenu()
    {
        // Hide base menu
        showActionMenu = false;
        Debug.Log("Selected entity is: " + selectedEntity);
        Debug.Log("Container is: " + specialsButtonContainer);
        Debug.Log("Prefab is: " + specialButtonPrefab);
        Debug.Log("Specials array: " + selectedEntity.specials);
        Debug.Log("Specials length: " + selectedEntity.specials?.Length);
        foreach (var special in selectedEntity.specials)
        {
            Debug.Log($"{special.Name}");

        }
        // Show cancel button
        cancelButton.SetActive(true);

        // Show the menu
        specialsMenu.SetActive(true);

        // Clear old buttons
        foreach (Transform child in specialsButtonContainer)
            Destroy(child.gameObject);

        // Build new buttons based on selectedEntity.specials
        for (int i = 0; i < selectedEntity.specials.Length; i++)
        {
            Special s = selectedEntity.specials[i];
            if (s == null) continue;

            GameObject btnObj = Instantiate(specialButtonPrefab, specialsButtonContainer);
            Button btn = btnObj.GetComponentInChildren<Button>();
            SpecialButtonUI ui = btnObj.GetComponent<SpecialButtonUI>();

            ui.label.text = s.Name;
            ui.manaCostText.text = s.manaCost.ToString();
            ui.SetElement(s.elem);
            bool canAfford = selectedEntity.Mana >= s.manaCost;

            btn.interactable = canAfford;

            Image bg = btnObj.GetComponent<Image>();
            if (bg != null)
                bg.color = canAfford ? Color.white : new Color(0.4f, 0.4f, 0.4f);
            ui.SetAffordable(canAfford);


            btn.onClick.AddListener(() =>
            {
                selectedEntity.chosenSpecial = s;
                StartCoroutine(Special_Coroutine());
                CloseSpecialMenu();
            });
        }
    }
    public void CloseSpecialMenu()
    {
        specialsMenu.SetActive(false);
        cancelButton.SetActive(false);
        showActionMenu = true;
        actionMenu.SetActive(true);
    }

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
        showActionMenu = false;

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