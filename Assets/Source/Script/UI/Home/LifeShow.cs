using System;
using TMPro;
using UnityEngine;


public class LifeShow : MonoBehaviour
{
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI countDownText;

    private int countdown;
    private void Update()
    {
        lifeText.text = ScoreManager.Instance.life.ToString();
        if (ScoreManager.Instance.life == 5) countDownText.text = "FULL";
        else
        {
            countdown = (int) ScoreManager.Instance.countDownTime;
            countDownText.text = $"{countdown / 60:00}:{countdown % 60:00}";
        }
    }
}
