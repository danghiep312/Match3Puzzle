using System;
using System.Collections;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject settingPanel;
    public GameObject giftPanel;

    private void Start()
    {
        this.RegisterListener(EventID.GameOver, (param) => OnGameOver());
        this.RegisterListener(EventID.Win, (param) => OnWin());

    }

    private void OnGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    private void OnWin()
    {
        winPanel.SetActive(true);
    }
    
    public void ClickSetting()
    {
        settingPanel.SetActive(true);
        GameManager.Instance.AcceptInput = false;
    }
    
    public void ClickGift()
    {
        giftPanel.SetActive(true);
        GameManager.Instance.AcceptInput = false;
    }
    
    public void ClickBackSetting()
    {
        GameManager.Instance.AcceptInput = true;
    } 
    
    public void ClickBackGift()
    {
        GameManager.Instance.AcceptInput = true;
    }
    
    public void PlayGame(int level)    
    {
        AfterPlayGame(level);
    }

    private async void AfterPlayGame(int level)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0f));
        this.PostEvent(EventID.PlayGame, level);
    }
    
    public void ShowResume()
    {
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
    }
}