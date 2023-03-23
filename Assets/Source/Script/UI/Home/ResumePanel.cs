using System;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


public class ResumePanel : MonoBehaviour
{
    private RectTransform rt;
    private void OnEnable()
    {
        if (rt == null)
        {
            rt = GetComponent<RectTransform>();
        }

        //rt.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutQuad);
        
        rt.localScale = Vector3.zero;
        rt.anchoredPosition = Vector2.zero;
        rt.DOScale(Vector3.one, .5f).SetEase(Ease.InOutSine);
        rt.DOAnchorPos(Vector2.right * 700f, 0.25f).SetEase(Ease.InSine).OnComplete(() =>
        {
            rt.DOAnchorPos(Vector2.zero, 0.25f).SetEase(Ease.OutSine);
        });
    }

    private void OnDisable()
    {
        rt.anchoredPosition = Vector2.zero;
    }

    public void Disappearance()
    {
        PlayerPrefs.DeleteKey("LastTime");
        rt.DOAnchorPos(Vector2.right * 2000f, 0.3f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            transform.parent.gameObject.SetActive(false);
        });
    }

    public void ClickHomeButton()
    {
        rt.DOAnchorPos(Vector2.right * 2000f, 0.3f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            transform.parent.gameObject.SetActive(false);
            this.PostEvent(EventID.GoHome);
        });
    }
}
