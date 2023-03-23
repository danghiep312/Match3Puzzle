using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiftBox : MonoBehaviour
{
    public GameObject[] booster;
    public void Open()
    {
        Array.ForEach(booster, g => BoosterManager.Instance.CollectBooster(g.name, 2));
    }
}