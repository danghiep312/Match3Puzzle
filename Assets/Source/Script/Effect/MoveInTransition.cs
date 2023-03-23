using System;
using DG.Tweening;
using UnityEngine;

public class MoveInTransition : MonoBehaviour
{
    public float duration;
    public float delay;

    public Vector3 targetPos;
    /// <summary>
    /// Target anchor is used for UI element.
    /// </summary>
    public Vector2 targetAnchor;
    public Ease ease;
    
    public bool offOnComplete;
    
    public RectTransform rt;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    public void Executive()
    {
        if (rt != null)
        {
            rt.DOAnchorPos(targetAnchor, duration).SetDelay(delay).SetEase(ease).OnComplete(() =>
            {
                if (offOnComplete)
                {
                    gameObject.SetActive(false);
                }
            });
        }
    }
}
