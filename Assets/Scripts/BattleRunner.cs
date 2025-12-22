using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleRunner : MonoBehaviour
{
    public static BattleRunner active;


    private Battle battle;

    public Dictionary<GameObject, Vector3> entityObjects;

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

    public IEnumerator StartBattle(List<OverworldEntity> playersInBattle, List<OverworldEntity> enemiesInBattle)
    {
        Debug.Log("Starting battle");

        AsyncOperation task = SceneManager.LoadSceneAsync(battleSceneName);

        while (!task.isDone)
            yield return new WaitForEndOfFrame();

        for (int i = 0; i < playersInBattle.Count; i++)
        {
            OverworldEntity oe = playersInBattle[i];

            // Determine placement on screen
            RectTransform spawnArea = GameObject.FindGameObjectWithTag("PlayerArea").GetComponent<RectTransform>();
            float posY = i / (float)playersInBattle.Count * spawnArea.sizeDelta.y - spawnArea.sizeDelta.y / 2;

            // Instantiate battle prefab there
            Instantiate(oe.battlePrefab, new Vector3(0, posY), Quaternion.identity, spawnArea);

            // Add entity component of battle prefab to players list
            players.Add(oe.battlePrefab.GetComponent<Entity>());
        }
        for (int i = 0; i < enemiesInBattle.Count; i++)
        {
            OverworldEntity oe = enemiesInBattle[i];
            
            // Determine placement on screen
            RectTransform spawnArea = GameObject.FindGameObjectWithTag("EnemyArea").GetComponent<RectTransform>();
            float posY = i / (float)enemiesInBattle.Count * spawnArea.sizeDelta.y - spawnArea.sizeDelta.y / 2;

            // Instantiate battle prefab there
            Instantiate(oe.battlePrefab, new Vector3(0, posY), Quaternion.identity, spawnArea);

            // Add entity component of battle prefab to enemies list
            enemies.Add(oe.battlePrefab.GetComponent<Entity>());
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
            if (player.state.alive)
                allDead = false;
        if (allDead)
            gameState = GameState.LOSE;

        allDead = true;
        foreach (Entity enemy in enemies)
            if (enemy.state.alive)
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
        foreach (KeyValuePair<GameObject, Vector3> entry in entityObjects)
            Instantiate(entry.Key, entry.Value, Quaternion.identity);
    }

    // Used when spawning an entity for the first time
    public static void SpawnEntity(
        GameObject prefab,
        Vector3 worldPosition = new(),
        Quaternion rotation = new(),
        Transform parent = null
    )
    {
        Instantiate(prefab, worldPosition, rotation, parent);
        active.entityObjects.Add(prefab, worldPosition);
    }

    public static void DespawnEntity(GameObject entity)
    {
        active.entityObjects.Remove(entity);
        Destroy(entity);
    }
}