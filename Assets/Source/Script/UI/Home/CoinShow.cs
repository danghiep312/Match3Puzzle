using System;
using TMPro;
using UnityEngine;

public class CoinShow : MonoBehaviour
{
    public TextMeshProUGUI coinText;

    private void Start()
    {
        coinText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        coinText.text = ScoreManager.Instance.coin.ToString();
    }
}
