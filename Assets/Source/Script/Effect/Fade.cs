using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Fade : MonoBehaviour
{
     public float fadeValue;
     public float duration;
     public float delay;
     public Ease ease;

     public SpriteRenderer sr;
     public Image img;
     public TextMeshProUGUI txt;
     
     private void OnEnable()
     {
         sr = GetComponent<SpriteRenderer>();
         img = GetComponent<Image>();
         txt = GetComponent<TextMeshProUGUI>();
     }

     private void OnDisable()
     {
         if (sr != null)
         {
             sr.DOFade(0, 0);
         }
         else if (img != null)
         {
             img.DOFade(0, 0);
         }
         else if (txt != null)
         {
             txt.DOFade(0, 0);
         }
     }

     public void Executive()
    {
        if (sr != null)
        {
            sr.DOFade(fadeValue, duration).SetDelay(delay).SetEase(ease);
        }
        else if (img != null)
        {
            img.DOFade(fadeValue, duration).SetDelay(delay).SetEase(ease);
        }
        else if (txt != null)
        {
            txt.DOFade(fadeValue, duration).SetDelay(delay).SetEase(ease);
        }
    }
}
