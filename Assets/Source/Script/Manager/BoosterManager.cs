using System;
using TMPro;
using UnityEngine;

public class BoosterManager : Singleton<BoosterManager>
{
    public bool unlimitedBooster;
    public int Hint;
    public int Return;
    public int Shuffle;

    private void Start()
    {
        Hint = PlayerPrefs.GetInt("Hint", 5);
        Return = PlayerPrefs.GetInt("Return", 5);
        Shuffle = PlayerPrefs.GetInt("Shuffle", 5);
    }
    
    public void UseHint()
    {
        if (unlimitedBooster)
            return;
        Hint--;
        PlayerPrefs.SetInt("Hint", Hint);
    }
    
    public void UseReturn()
    {
        if (unlimitedBooster)
            return;
        Return--;
        PlayerPrefs.SetInt("Return", Return);
    }  
    
    public void UseShuffle()
    {
        if (unlimitedBooster)
            return;
        Shuffle--;
        PlayerPrefs.SetInt("Shuffle", Shuffle);
    }

    public void CollectBooster(string type, int quantity)
    {
        switch (type)
        {
            case "Hint":
                Hint += quantity;
                PlayerPrefs.SetInt("Hint", Hint);
                break;
            case "Return":
                Return += quantity;
                PlayerPrefs.SetInt("Return", Return);
                break;
            case "Shuffle":
                Shuffle += quantity;
                PlayerPrefs.SetInt("Shuffle", Shuffle);
                break;
        }
    }
    
    
    
}

