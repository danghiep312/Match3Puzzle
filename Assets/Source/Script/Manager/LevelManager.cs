using System;
using Sirenix.OdinInspector;
using UnityEngine;


public class LevelManager : Singleton<LevelManager>
{
    [ShowInInspector]
    public static int MAX_LEVEL;
    public int currentLevel;

    private void Start()
    {
        this.RegisterListener(EventID.PlayGame, (param) => OnPlayGame((int)param));
        this.RegisterListener(EventID.Win, (param) => OnWinGame());
        MAX_LEVEL = PlayerPrefs.GetInt("MaxLevel", 1);
        currentLevel = MAX_LEVEL;
    }

    private void OnPlayGame(int level)
    {
        currentLevel = level;
    }

    private void OnWinGame()
    {
        if (currentLevel + 1 > MAX_LEVEL)
        {
            MAX_LEVEL = currentLevel + 1;
            PlayerPrefs.SetInt("MaxLevel", MAX_LEVEL);
        }
    }
}
