using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    private RectTransform rt;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    public void GoHome()
    {
        MapGenerator.Instance.SaveCurrentState();
        rt.DOAnchorPos(Vector2.right * 2000f, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            this.PostEvent(EventID.PrepareGoHome);
            GetComponentInParent<Image>().DOFade(0, 0.6f).SetEase(Ease.Linear);
            rt.DOAnchorPos(rt.anchoredPosition, 0.6f).OnComplete(() =>
            {
                //GameManager.Instance.ReloadScene();
                transform.parent.gameObject.SetActive(false);
                this.PostEvent(EventID.GoHome);
                
            });
        });
    }

    public void OnDisable()
    {
        rt.anchoredPosition = Vector2.zero;
        GetComponent<Image>().color = new Color(1, 1, 1, 1);
        GameManager.Instance.AcceptInput = true;
    }

    private void Update()
    {
        GameManager.Instance.AcceptInput = !transform.gameObject.activeSelf;
    }
    
}