using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Security;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;


public class GameManager : Singleton<GameManager>
{
    public float horizontalMin;
    public float horizontalMax;

    public bool AreCombining;
    private DateTime lastTime;

    [Header("Game State")]
    public bool InHome;
    public bool Playing;
    public bool AcceptInput;
    public bool CanRevive;

    public bool CanUndo;
    [Header("Game Object")] 
    public GameObject queueZone;
    public GameObject blockInput;

    public bool vibrationOn;
    public bool tutorialComplete;
    public GameObject tutorial;

    public ParticleSystem combineParticle;
 
    private void Start()
    {
        Application.targetFrameRate = 60;
        Camera camera = Camera.main;
        float halfHeight = camera.orthographicSize;
        float halfWidth = camera.aspect * halfHeight;

        horizontalMin = -halfWidth;
        horizontalMax = halfWidth;
        AreCombining = false;
        Playing = false;
        CanUndo = true;

        this.RegisterListener(EventID.Spell, (param) => OnUseSpell((float)param));
        this.RegisterListener(EventID.PlayGame, (param) => OnPlayGame());
        this.RegisterListener(EventID.GoHome, (param) => GoHome());
        this.RegisterListener(EventID.Win, (param) => SetPlaying(false));
        this.RegisterListener(EventID.GameOver, (param) => SetPlaying(false));

        vibrationOn = PlayerPrefs.GetInt("Vibration", 1) == 1;
        tutorialComplete = PlayerPrefs.GetInt("Tutorial", 0) == 1;
        tutorial.SetActive(!tutorialComplete);
        
        //StartGame();
        ClickPlayGame();
    }
    
    private void SetPlaying(bool playing)
    {
        Playing = playing;
    }

    private async void StartGame()
    {
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        if (MapGenerator.Instance.CheckFileExist())
        {
            ClickPlayGame();
        }
    }

    public void Revive()
    {
        AcceptInput = true;
        CanRevive = false;
        Playing = true;
        this.PostEvent(EventID.Revive);
    }

    private void OnPlayGame()
    {
        CanRevive = true;
        AcceptInput = true;
        queueZone.SetActive(true);
        Playing = true;
        InHome = false;
    }

    private void OnUseSpell(float seconds)
    {
        CanUndo = false;
        DelayUndo(seconds);
    }
    
    public void ClickPlayGame()
    {
        PlayGame(LevelManager.MAX_LEVEL);
    }

    public void ClickResume()
    {
        PlayerPrefs.DeleteKey("LastTime");
        ObjectPooler.Instance.ReleaseAll();
        SetBlockInput(true);
        this.PostEvent(EventID.PlayGame, LevelManager.MAX_LEVEL);
    }


    [Button("Play Game")]
    public void PlayGame(int level)
    {
        PlayerPrefs.DeleteKey("LastTime");
        Debug.Log("Play " + level);
        ObjectPooler.Instance.ReleaseAll();
        UIManager.Instance.PlayGame(level);
        SetBlockInput(true);
    }

    public void Retry()
    {
        PlayGame(LevelManager.Instance.currentLevel);
        this.PostEvent(EventID.Retry);
    }

    public void NextLevel()
    {
        PlayGame(LevelManager.Instance.currentLevel + 1);
    }

    public void SkipLevel()
    {
        PlayGame(LevelManager.Instance.currentLevel + 1);
    }

    public void RestartLevel()
    {
        ObjectPooler.Instance.ReleaseAll();
        this.PostEvent(EventID.Restart);
        MapGenerator.Instance.Restart();
        AcceptInput = true;
    }

    [Button("Test load scene")]
    public void ReloadScene()
    {
        DOTween.KillAll();
        SceneManager.LoadScene("GameplayScene");
    }

    async void DelayUndo(float seconds)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(seconds));
        CanUndo = true;
    }

    public void SetBlockInput(bool active)
    {
        AcceptInput = !active;
        blockInput.SetActive(active);
    }

    public void GoHome()
    {
        queueZone.SetActive(false);
        InHome = true;
        Playing = false;
    }

    [Button("Test save")]
    public void Test(bool hasFocus)
    {
        MapGenerator.Instance.SaveCurrentState();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (Playing) MapGenerator.Instance.SaveCurrentState();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        Common.Log("Pause");
        if (Playing) MapGenerator.Instance.SaveCurrentState();
        if (pauseStatus && Playing)
        {
            var now = DateTime.Now;
            PlayerPrefs.SetString("LastTime", now.ToString("MM/dd/yyyy HH:mm:ss"));
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Quit app");
    }


    [Button("Click To Play")]
    public void ClickPlayInHome()
    {
        // MasterControl.Instance.ShowBanner();
        // if (!MapGenerator.Instance.CheckFileExist())
        // {
        //     ScoreManager.Instance.UseLife();
        // }
    }
    
    [Button("Open Save File")]
    public void OpenSaveFile()
    {
        Common.Log(Application.persistentDataPath);
        Process.Start(Application.persistentDataPath);
    }

    public void PlayCombineParticles(Vector3 pos)
    {
        combineParticle.transform.position = pos;
        combineParticle.Play();
    }
    

    #region Toggle Func

    public void ToggleVibration()
    {
        vibrationOn = !vibrationOn;
        PlayerPrefs.SetInt("Vibration", vibrationOn ? 1 : 0);
    }

    public void ToggleSound()
    {
        AudioManager.Instance.ToggleSound();
    }

    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
    }

    #endregion
}