using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleRunner : MonoBehaviour
{
    public static BattleRunner active;


    private Battle battle;

    private List<Entity> players = new();
    private List<Entity> enemies = new();
    private List<Entity> entitiesInBattle = new();

    public string battleSceneName = "BattleScene";
    public string gameSceneName = "OverworldScene";
    public string loseSceneName = "GameOverScene";

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
            GameObject battlePrefab = EntityManager.entities[playersInBattle[i]].battlePrefab;

            // Determine placement on screen
            RectTransform spawnArea = GameObject.FindGameObjectWithTag("PlayerArea").GetComponent<RectTransform>();
            float posY = i / (float)playersInBattle.Count * spawnArea.sizeDelta.y - spawnArea.sizeDelta.y / 2;

            // Instantiate battle prefab there
            GameObject instance = Instantiate(battlePrefab, spawnArea);
            ((RectTransform)instance.transform).localPosition = new Vector2(0, posY);
            instance.GetComponent<Entity>().id = playersInBattle[i];
            // Add entity component of instance to players list
            players.Add(instance.GetComponent<Entity>());
        }

        enemies = new();
        for (int i = 0; i < enemiesInBattle.Count; i++)
        {
            Debug.Log("Adding entity id#" + enemiesInBattle[i] + " to enemies");
            GameObject battlePrefab = EntityManager.entities[enemiesInBattle[i]].battlePrefab;

            // Determine placement on screen
            RectTransform spawnArea = GameObject.FindGameObjectWithTag("EnemyArea").GetComponent<RectTransform>();
            float posY = i / (float)enemiesInBattle.Count * spawnArea.sizeDelta.y - spawnArea.sizeDelta.y / 2;

            // Instantiate battle prefab there
            GameObject instance = Instantiate(battlePrefab, new Vector3(0, posY), Quaternion.identity, spawnArea);
            ((RectTransform)instance.transform).localPosition = new Vector2(0, posY);
            instance.GetComponent<Entity>().id = enemiesInBattle[i];

            // Add entity component of instance to players list
            enemies.Add(instance.GetComponent<Entity>());
        }

        entitiesInBattle = new();
        foreach (Entity e in players) entitiesInBattle.Add(e);
        foreach (Entity e in enemies) entitiesInBattle.Add(e);

        BattleInterface.active.players = players;
        BattleInterface.active.npcs = enemies;

        battle = new(entitiesInBattle);

        StartCoroutine(Run());
    }

    public IEnumerator Run()
    {
        yield return StartCoroutine(BattleInterface.active.SetMoves(() =>
        {
            Debug.Log("Simulating round");
            battle.SimulateRound();
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
        foreach (Entity e in players)
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
        foreach (Entity e in enemies)
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
        foreach (Entity e in entitiesInBattle)
            Debug.Log(e.entityName + "#" + e.id);

        Debug.Log(players.Count);
        foreach (Entity e in players)
            Debug.Log(e.entityName + "#" + e.id);

        Debug.Log(enemies.Count);
        foreach (Entity e in enemies)
            Debug.Log(e.entityName + "#" + e.id);
        
        foreach (Entity e in enemies)
        {
            Debug.Log("Removing enemy " + e.entityName + " with id#" + e.id);
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