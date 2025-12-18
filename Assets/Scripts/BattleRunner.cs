using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRunner : MonoBehaviour
{
    public Battle battle;
    public BattleInterface battleInterface;

    public List<Entity> players;
    public List<BattleNPC> enemies;
    private List<Entity> entities;

    private enum GameState
    {
        PLAYING,
        WIN,
        LOSE
    }
    private GameState gameState;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartBattle();
    }

    void StartBattle()
    {
        entities = new List<Entity>();
        for (int i = 0; i < players.Count + enemies.Count; i++)
            entities.Add(i < players.Count ? players[i] : enemies[i - players.Count]);

        battleInterface.players = players;
        battleInterface.npcs = enemies;

        battle = new(entities);

        StartCoroutine(Run());
    }

    public IEnumerator Run()
    {
        yield return StartCoroutine(battleInterface.SetMoves(() =>
        {
            Debug.Log("Simulating round");
            battle.SimulateRound();
        }));

        bool allDead = true;
        foreach (Entity player in players)
            if (player.state.alive)
                allDead = false;
        if (allDead)
            gameState = GameState.LOSE;

        allDead = true;
        foreach (BattleNPC enemy in enemies)
            if (enemy.state.alive)
                allDead = false;
        if (allDead)
            gameState = GameState.WIN;

        if (gameState == GameState.PLAYING)
            StartCoroutine(Run());
    }

    void Update()
    {
        // Do something
    }
}