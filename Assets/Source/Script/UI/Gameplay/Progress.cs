using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Progress : Singleton<Progress>
{
    public Image fill;
    public GameObject[] stars;

    private void OnEnable()
    {
        fill.fillAmount = 0;
        Array.ForEach(stars, star => star.SetActive(false));
    }

    private void Start()
    {
        this.RegisterListener(EventID.PlayGame, (param) => OnPlayGame());
        this.RegisterListener(EventID.CombineComplete, (param) => OnCombineComplete());
    }

    private void OnCombineComplete()
    {
        var mapInstance = MapGenerator.Instance;
        fill.DOKill();
        var value = mapInstance.numOfTileComplete;
        if (value >= mapInstance.totalTile - 2) value = mapInstance.totalTile;
        fill.DOFillAmount( value * 1f / mapInstance.totalTile, 0.2f).SetEase(Ease.Linear);
    }

    public void SetFillAmount(float value)
    {
        fill.DOFillAmount( value * 1f / MapGenerator.Instance.totalTile, 0.2f).SetEase(Ease.Linear);
    }
    
    private void OnPlayGame()
    {
        fill.DOFillAmount(0f, 0.2f).SetEase(Ease.Linear);
        Array.ForEach(stars, star =>
        {
            star.transform.localScale = Vector3.zero;
            star.SetActive(false);
        });
    }

    private void Update()
    {
        var mapInstance = MapGenerator.Instance;
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(mapInstance.numOfTileComplete * 1f / mapInstance.totalTile >= 0.3 * (i + 1));
        }
    }
}
