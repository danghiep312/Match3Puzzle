 using System;
using DG.Tweening;
using UnityEngine;


public class GameplayUI : MonoBehaviour
{
    public RectTransform levelNotification;
    public RectTransform coinZone;
    public RectTransform setting;
    public RectTransform buttonHolder;

    private void Start()
    {
        this.RegisterListener(EventID.PrepareGoHome, (param) => Disappearance());
        this.RegisterListener(EventID.Win, (param) => Disappearance());
        this.RegisterListener(EventID.PlayGame, (param) => Appearance());
        this.RegisterListener(EventID.GameOver, (param) => Disappearance());

    }

    public void Appearance()
    {
        if (!gameObject.activeSelf) return;
        levelNotification.DOAnchorPos(Vector2.up * -100f, 0.5f).SetEase(Ease.OutBack);
        setting.DOAnchorPos(new Vector2(-100f, setting.anchoredPosition.y), 0.5f).SetEase(Ease.OutBack);
        coinZone.DOAnchorPos(new Vector2(200f, coinZone.anchoredPosition.y), 0.5f).SetEase(Ease.OutBack);
        buttonHolder.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBack);
    }

    public void Disappearance()
    {
        levelNotification.DOAnchorPos(Vector2.up * 400f, 0.5f).SetEase(Ease.InBack);
        setting.DOAnchorPos(new Vector2(400f, setting.anchoredPosition.y), 0.5f).SetDelay(.1f).SetEase(Ease.InBack);
        coinZone.DOAnchorPos(new Vector2(-400f, coinZone.anchoredPosition.y), 0.5f).SetDelay(.1f).SetEase(Ease.InBack);
        buttonHolder.DOAnchorPos(Vector2.down * 600f, 0.5f).SetEase(Ease.InBack);
    }
    

    private void OnEnable()
    {
        Appearance();
    }

    private void OnDisable()
    {
        levelNotification.anchoredPosition = Vector2.up * 400f;
        setting.anchoredPosition = new Vector2(400f, setting.anchoredPosition.y);
        coinZone.anchoredPosition = new Vector2(-400f, coinZone.anchoredPosition.y);
        buttonHolder.anchoredPosition = Vector2.down * 600f;
    }
    
    
}
