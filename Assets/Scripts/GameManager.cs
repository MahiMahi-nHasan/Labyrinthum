using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int Seed { get; private set; }
    public bool HasSeed { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GenerateNewSeed()
    {
        Seed = Random.Range(int.MinValue, int.MaxValue);
        HasSeed = true;

        Debug.Log("Generated Seed: " + Seed);
    }

    public void ClearSeed()
    {
        HasSeed = false;
    }
    public void StartGame()
    {
        GenerateNewSeed();
        SceneManager.LoadScene("OverworldScene");
    }
}
