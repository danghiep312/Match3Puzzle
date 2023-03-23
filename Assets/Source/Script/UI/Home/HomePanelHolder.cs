using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class HomePanelHolder : MonoBehaviour
{
    [Tooltip("0 - Shop, 1 - Event, 2 - Home, 3 - Task, 4 - Theme")]
    public RectTransform[] panels;

    public RectTransform[] images;
    public GameObject[] texts;

    private RectTransform rt;
    private RectTransform roof;
    

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        GetSizeDelta();
        roof = panels[0].GetChild(0).GetChild(0).GetComponent<RectTransform>();
    }

   
    public void SwitchPanel(int panelType)
    {
        rt.DOKill();
        // rt.DOAnchorPosX(-panels[panelType].anchoredPosition.x, dis / sizeEachPanel * MoveHighLight.timeToMoveEachButton)
        //     .SetEase(Ease.OutExpo);
        
        rt.DOAnchorPosX(-panels[panelType].anchoredPosition.x, MoveHighLight.timeToMove)
            .SetEase(Ease.OutExpo);
        
        PrepareClick();
        images[panelType].DOAnchorPos(Vector2.up * 40, MoveHighLight.timeToMove).SetEase(Ease.OutExpo);
        texts[panelType].transform.DOScale(Vector3.one, MoveHighLight.timeToMove).SetEase(Ease.OutBack);
        for (int i = 0; i < 5; i++)
        {
            if (i != panelType)
            {
                texts[i].transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
                images[i].DOAnchorPos(Vector2.zero, 0.3f).SetEase(Ease.InBack);
            }
        }
        
        if (panelType == 0) ShowRoof();
        else HideRoof();
    }

    private void PrepareClick()
    {
        for (int i = 0; i < images.Length; i++)
        {
            texts[i].transform.DOKill();
            images[i].transform.DOKill();
        }
    }
    
    private async void GetSizeDelta()
    {
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        var size = panels[0].rect.size.x;
        for (int i = 0; i < 5; i++)
        {
            panels[i].anchoredPosition = Vector2.right * (i - 2) * size;
        }
    }
    
    private void HideRoof()
    {
        roof.anchoredPosition = Vector2.up * 3000f;
    }

    private void ShowRoof()
    {
        roof.DOKill();
        HideRoof();
        roof.DOAnchorPos(Vector2.up * 850f, .5f).SetDelay(1f).SetEase(Ease.OutQuad);
    }
}
