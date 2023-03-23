 using System;
using TMPro;
using UnityEngine;

public class LevelText : MonoBehaviour
{
     public TextMeshProUGUI levelText;

     private void Start()
     {
          levelText = GetComponent<TextMeshProUGUI>();
     }
     

     private void Update()
     {
          
          if (transform.parent != null && transform.parent.name.Equals("Play"))
          {
               levelText.text = $"Level {LevelManager.MAX_LEVEL}";
          }
          else
          {
               levelText.text = $"Level {LevelManager.Instance.currentLevel}";
          }
     }
}
