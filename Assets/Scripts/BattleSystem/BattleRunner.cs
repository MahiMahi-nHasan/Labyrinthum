using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleRunner : MonoBehaviour
{
    public static BattleRunner active;


    private Battle battle;
    private List<BattleEntity> players = new();
    private List<BattleEntity> enemies = new();
    private List<BattleEntity> entitiesInBattle = new();

    public string battleSceneName = "BattleScene";
    public string gameSceneName = "OverworldScene";
    public string loseSceneName = "GameOverScene";

    public GameObject turnBarIconPrefab;
    public float iconSpacing;

    public enum GameState
    {
        PLAYING,
        WIN,
        LOSE,
        OVERWORLD
    }
    public GameState gameState = GameState.OVERWORLD;

    void Awake()
    {
        if (active == null)
        {
            active = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator CallAfterSceneLoad(string sceneName, System.Action callback, LoadSceneMode mode = LoadSceneMode.Single)
    {
        AsyncOperation task = SceneManager.LoadSceneAsync(sceneName, mode);
        while (!task.isDone)
            yield return new WaitForEndOfFrame();

        callback();
    }

    public void StartBattle(List<int> playersInBattle, List<int> enemiesInBattle)
    {
        StartCoroutine(CallAfterSceneLoad(
            battleSceneName,
            () => {
                StartBattleInternal(playersInBattle, enemiesInBattle);
            }
        ));
    }
    public void StartBattle(List<OverworldEntity> playersInBattle, List<OverworldEntity> enemiesInBattle)
    {
        List<int> playerIDs = new();
        foreach (OverworldEntity e in playersInBattle)
            playerIDs.Add(e.id);

        List<int> enemyIDs = new();
        foreach (OverworldEntity e in enemiesInBattle)
            enemyIDs.Add(e.id);

        StartBattle(playerIDs, enemyIDs);
    }

    private void StartBattleInternal(List<int> playersInBattle, List<int> enemiesInBattle)
    {
        gameState = GameState.PLAYING;

        Debug.Log(string.Format(
            "Starting battle:\nPlayer count: {0}\nEnemy count: {1}",
            playersInBattle.Count,
            enemiesInBattle.Count
        ));

        players = new();
        for (int i = 0; i < playersInBattle.Count; i++)
        {
            Debug.Log("Adding entity #" + playersInBattle[i] + " to players");
            GameObject battlePrefab = EntityManager.entities[playersInBattle[i]].baseEntity.battlePrefab;

            // Determine placement on screen
            RectTransform spawnArea = GameObject.FindGameObjectWithTag("PlayerArea").GetComponent<RectTransform>();
            float posY = i / (float)playersInBattle.Count * spawnArea.sizeDelta.y - spawnArea.sizeDelta.y / 2 + 87.5f;

            // Instantiate battle prefab there
            GameObject instance = Instantiate(battlePrefab, spawnArea);
            instance.GetComponent<RectTransform>().localPosition = new(0, posY);
            instance.GetComponent<BattleEntity>().id = playersInBattle[i];
            // Add entity component of instance to players list
            players.Add(instance.GetComponent<BattleEntity>());
        }

        enemies = new();
        for (int i = 0; i < enemiesInBattle.Count; i++)
        {
            Debug.Log("Adding entity id#" + enemiesInBattle[i] + " to enemies");
            GameObject battlePrefab = EntityManager.entities[enemiesInBattle[i]].baseEntity.battlePrefab;

            // Determine placement on screen
            RectTransform spawnArea = GameObject.FindGameObjectWithTag("EnemyArea").GetComponent<RectTransform>();
            float posY = i / (float)enemiesInBattle.Count * spawnArea.sizeDelta.y - spawnArea.sizeDelta.y / 2 + 87.5f;

            // Instantiate battle prefab there
            GameObject instance = Instantiate(battlePrefab, spawnArea);
            ((RectTransform)instance.transform).localPosition = new(0, posY);
            instance.GetComponent<BattleEntity>().id = enemiesInBattle[i];

            // Add entity component of instance to players list
            enemies.Add(instance.GetComponent<BattleEntity>());
        }

        entitiesInBattle = new();
        foreach (BattleEntity e in players) entitiesInBattle.Add(e);
        foreach (BattleEntity e in enemies) entitiesInBattle.Add(e);

        BattleInterface.active.players = players;
        BattleInterface.active.npcs = enemies;

        battle = new(entitiesInBattle);

        StartCoroutine(Run());
    }

    public IEnumerator Run()
    {
        // Display ordered turns

        /// Destroy existing markers
        GameObject[] existing = GameObject.FindGameObjectsWithTag("TurnIcon");
        foreach (GameObject go in existing)
            Destroy(go);
        
        /// Filter for living entities
        Queue<BattleEntity> orderedTurns = battle.GetOrderedEntities();
        Queue<BattleEntity> orderedTurnsAlive = new();
        while (orderedTurns.Count > 0)
        {
            BattleEntity next = orderedTurns.Dequeue();
            if (!next.state.dead)
                orderedTurnsAlive.Enqueue(next);
        }
        int count = orderedTurnsAlive.Count;
        RectTransform turnBarCenter = GameObject.FindGameObjectWithTag("TurnBarCenter").GetComponent<RectTransform>();

        /// Place markers
        for (int i = 0; i < count; i++)
        {
            Entity next = orderedTurnsAlive.Dequeue().baseEntity;
            GameObject go = Instantiate(turnBarIconPrefab, turnBarCenter);
            ((RectTransform)go.transform).localPosition = new Vector2(
                i * iconSpacing - (count - 1) * iconSpacing / 2,
                0
            );
            go.GetComponent<Image>().sprite = next.sprite;
        }
        
        yield return StartCoroutine(BattleInterface.active.SetMoves(() =>
        {
            Debug.Log("Simulating round");
            StartCoroutine(battle.SimulateRound());
        }));

        UpdateGameState();

        switch (gameState)
        {
            case GameState.PLAYING:
                StartCoroutine(Run());
                break;
            case GameState.WIN:
                OnPlayerWin();
                break;
            case GameState.LOSE:
                OnPlayerLose();
                break;
        }
    }

    void UpdateGameState()
    {
        bool allDead = true;
        foreach (BattleEntity e in players)
        {
            if (!e.state.dead)
                allDead = false;
        }
        if (allDead)
        {
            gameState = GameState.LOSE;
            return;
        }

        allDead = true;
        foreach (BattleEntity e in enemies)
        {
            if (!e.state.dead)
                allDead = false;
        }
        if (allDead)
            gameState = GameState.WIN;
    }

    public void OnPlayerWin()
    {
        // Do something
        Debug.Log("Player won");
        LoadGameScene();
    }

    public void OnPlayerLose()
    {
        // Do something
        Debug.Log("Player lost");
    }

    public void LoadGameScene()
    {
        Debug.Log(entitiesInBattle.Count);
        foreach (BattleEntity e in entitiesInBattle)
            Debug.Log(e.baseEntity.entityName + "#" + e.id);

        Debug.Log(players.Count);
        foreach (BattleEntity e in players)
            Debug.Log(e.baseEntity.entityName + "#" + e.id);

        Debug.Log(enemies.Count);
        foreach (BattleEntity e in enemies)
            Debug.Log(e.baseEntity.entityName + "#" + e.id);
        
        foreach (BattleEntity e in enemies)
        {
            Debug.Log("Removing enemy " + e.baseEntity.entityName + " with id#" + e.id);
            EntityManager.RemoveEntity(e.id);
        }

        Debug.Log("Removed dead entities");
        
        StartCoroutine(CallAfterSceneLoad(gameSceneName, () =>
        {
            Debug.Log("Scene loaded");

            EntityManager.SpawnAllEntities();

            Debug.Log("Spawned entities");

            gameState = GameState.OVERWORLD;
        }));
    }
    
}