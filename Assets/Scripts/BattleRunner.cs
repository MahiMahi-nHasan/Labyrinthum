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
        active = this;
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

    public void StartBattleInternal(List<int> playersInBattle, List<int> enemiesInBattle)
    {
        Debug.Log(string.Format(
            "Starting battle:\nPlayer count: {0}\nEnemy count: {1}",
            playersInBattle.Count,
            enemiesInBattle.Count
        ));

        for (int i = 0; i < playersInBattle.Count; i++)
        {
            GameObject battlePrefab = EntityManager.entities[playersInBattle[i]].battlePrefab;

            // Determine placement on screen
            RectTransform spawnArea = GameObject.FindGameObjectWithTag("PlayerArea").GetComponent<RectTransform>();
            float posY = i / (float)playersInBattle.Count * spawnArea.sizeDelta.y - spawnArea.sizeDelta.y / 2;

            // Instantiate battle prefab there
            GameObject instance = Instantiate(battlePrefab, spawnArea);
            ((RectTransform)instance.transform).localPosition = new Vector2(0, posY);
            // Add entity component of battle prefab to players list
            players.Add(battlePrefab.GetComponent<Entity>());
        }
        for (int i = 0; i < enemiesInBattle.Count; i++)
        {
            GameObject battlePrefab = EntityManager.entities[enemiesInBattle[i]].battlePrefab;

            // Determine placement on screen
            RectTransform spawnArea = GameObject.FindGameObjectWithTag("EnemyArea").GetComponent<RectTransform>();
            float posY = i / (float)enemiesInBattle.Count * spawnArea.sizeDelta.y - spawnArea.sizeDelta.y / 2;

            // Instantiate battle prefab there
            GameObject instance = Instantiate(battlePrefab, new Vector3(0, posY), Quaternion.identity, spawnArea);
            ((RectTransform)instance.transform).localPosition = new Vector2(0, posY);

            // Add entity component of battle prefab to players list
            enemies.Add(battlePrefab.GetComponent<Entity>());
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

        bool allDead = true;
        foreach (Entity player in players)
            if (!player.state.dead)
                allDead = false;
        if (allDead)
            gameState = GameState.LOSE;

        allDead = true;
        foreach (Entity enemy in enemies)
            if (!enemy.state.dead)
                allDead = false;
        if (allDead)
            gameState = GameState.WIN;

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

    public void OnPlayerWin()
    {
        // Do something
        LoadGameScene();
    }

    public void OnPlayerLose()
    {
        // Do something
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);

        entitiesInBattle = new();
        foreach (int id in EntityManager.entities.Keys)
            EntityManager.SpawnExistingEntity(id);
    }
}