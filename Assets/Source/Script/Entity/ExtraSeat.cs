using System;
using DG.Tweening;
using UnityEngine;


public class ExtraSeat : MonoBehaviour
{
    private SpriteRenderer sr;
    private Collider2D col;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        this.RegisterListener(EventID.PlayGame, (param) => OnPlayGame());
    }

    private void OnPlayGame()
    {
        sr.color = Color.white;
        col.enabled = true;
    }

    private void OnMouseDown()
    {
        OpenExtraSlot();
        // MasterControl.Instance.ShowRewardedAd((success) =>
        // {
        //     if (success)
        //     {
        //         OpenExtraSlot();
        //     }
        // });
    }

    private void OpenExtraSlot()
    {
        col.enabled = false;
        sr.DOFade(0, 0.5f).SetEase(Ease.Linear);
        this.PostEvent(EventID.AddSlot);
    }
    
}
