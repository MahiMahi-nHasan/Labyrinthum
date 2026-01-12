using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleRunner : MonoBehaviour
{
    public static BattleRunner active;

    private List<BattleEntity> players = new();
    private List<BattleEntity> enemies = new();
    private List<BattleEntity> entitiesInBattle = new();
    public AudioSource audioSource;
    public AudioClip[] clips;

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
        Queue<BattleEntity> orderedTurns = GetOrderedEntities();
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
            StartCoroutine(SimulateRound());
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
    IEnumerator SimulateTurn(BattleEntity e, float waitTime)
    {
        switch (e.state.plannedMove)
        {
            case BattleEntity.Move.ATTACK:
                e.state.target.TakeDamage(e.BaseDamage(), e.state.element);
                audioSource.PlayOneShot(clips[0]);
                break;
            case BattleEntity.Move.DEFEND:
                e.Defend();
                audioSource.PlayOneShot(clips[1]);
                break;
            case BattleEntity.Move.RECHARGE:
                e.Recharge();
                audioSource.PlayOneShot(clips[2]);
                break;
            case BattleEntity.Move.SPECIAL:
                e.Special();
                if(e.chosenSpecial.type == Special.SpecialType.Heal|| e.chosenSpecial.type == Special.SpecialType.AoEHeal)
                {
                    audioSource.PlayOneShot(clips[7]);
                }
                else{
                    switch (e.baseEntity.element)
                    {
                        case BattleEntity.Element.PHYS:
                        audioSource.PlayOneShot(clips[5]);
                        break; 
                        case BattleEntity.Element.FIRE:
                        audioSource.PlayOneShot(clips[4]);
                        break;
                        case BattleEntity.Element.ICE:
                        audioSource.PlayOneShot(clips[5]);
                        break;
                        case BattleEntity.Element.WIND:
                        audioSource.PlayOneShot(clips[6]);
                        break;

                }
                }
                break;
                
        }

        yield return new WaitForSeconds(waitTime);
    }
    public IEnumerator SimulateRound()
    {
        // Reset all entities to default state
        foreach (BattleEntity e in entitiesInBattle)
            e.isDefending = false;

        Queue<BattleEntity> orderedEntities = GetOrderedEntities();

        while (orderedEntities.Count > 0)
        {
            BattleEntity e = orderedEntities.Dequeue();

            if (e.state.dead)
            {
                Debug.Log(e.baseEntity.entityName + " is dead");
                continue;
            }

            yield return SimulateTurn(e, 0.5f);
        }
    }
    public Queue<BattleEntity> GetOrderedEntities()
    {
        List<BattleEntity> bkp = new(entitiesInBattle);
        // Sort entities in order of speed
        Queue<BattleEntity> orderedEntities = new();
        while (entitiesInBattle.Count > 0)
        {
            int iMaxSpeed = 0;
            for (int i = 1; i < entitiesInBattle.Count; i++)
                if (entitiesInBattle[i].Speed > entitiesInBattle[iMaxSpeed].Speed)
                    iMaxSpeed = i;

            orderedEntities.Enqueue(entitiesInBattle[iMaxSpeed]);
            entitiesInBattle.RemoveAt(iMaxSpeed);
        }
        entitiesInBattle = bkp;

        return orderedEntities;
    }
}