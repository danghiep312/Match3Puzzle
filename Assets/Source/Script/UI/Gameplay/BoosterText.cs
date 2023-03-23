using System;
using TMPro;
using UnityEngine;

public class BoosterText : MonoBehaviour
{
    public TextMeshProUGUI txt;

    private void Start()
    {
        txt = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        txt.text = gameObject.name switch
        {
            "Hint" => BoosterManager.Instance.Hint.ToString(),
            "Return" => BoosterManager.Instance.Return.ToString(),
            "Shuffle" => BoosterManager.Instance.Shuffle.ToString(),
            _ => txt.text
        };

        if (txt.text.Equals("0"))
        {
            txt.text = "+";
        }
        
    }
    
    
}