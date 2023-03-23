using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;


public class MoveHighLight : MonoBehaviour
{
    public static float timeToMove = 0.7f;
    [ShowInInspector]
    private float size; // size of button
    private RectTransform rt;
    
    private void Start()
    {
        rt = GetComponent<RectTransform>();
        size = rt.sizeDelta.x;
    }

    public void Move(RectTransform target)
    {
        transform.DOKill();
        rt.DOAnchorPosX(target.anchoredPosition.x, timeToMove).SetEase(Ease.OutSine);
    }
    
}
