using System;
using Sirenix.OdinInspector;
using UnityEngine;


public class ScoreManager : Singleton<ScoreManager>
{
    public static float TIME_TO_HEAL = 60; // seconds
    [ShowInInspector]
    public DateTime timeClickToPlay;
    public float countDownTime;
    
    public int coin;
    public int life;

    public override void Awake()
    {
        base.Awake();


        try
        {
            timeClickToPlay = DateTime.ParseExact(PlayerPrefs.GetString("Time", "01/01/2000 00:00:00"), Pattern.TIME_PATTERN, null);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("Score "+ PlayerPrefs.GetString("Time", "01/01/2000 00:00:00"));
            timeClickToPlay = DateTime.Now;
        }
        life = PlayerPrefs.GetInt("Life", 5);
        coin = PlayerPrefs.GetInt("Coin", 0);

        var elapsed = DateTime.Now.Subtract(timeClickToPlay);
        life += (int) (elapsed.TotalSeconds / TIME_TO_HEAL);
        if (life >= 5) life = 5;
        countDownTime = (int) elapsed.TotalSeconds % TIME_TO_HEAL;
        if (life <= 0) life = 0;
    }


    [Button("Test")]
    public void UseLife()
    {
        if (life == 5)
        {
            timeClickToPlay = DateTime.Now;
            PlayerPrefs.SetString("Time", timeClickToPlay.ToString("MM/dd/yyyy HH:mm:ss"));
            countDownTime = TIME_TO_HEAL;
        }
        life--;
        if (life <= 0) life = 0;
        PlayerPrefs.SetInt("Life", life);
    }

    public void AddCoin(int value)
    {
        coin += value;
        PlayerPrefs.SetInt("Coin", coin);
    }

    public void RefillLife()
    {
        life = 5;
    }

    private void Update()
    {
        countDownTime -= Time.deltaTime;
        if (countDownTime <= 0)
        {
            if (life < 5)
            {
                life++;
                PlayerPrefs.SetInt("Life", life);
            }
            countDownTime = TIME_TO_HEAL;
        }
    }
    
}
