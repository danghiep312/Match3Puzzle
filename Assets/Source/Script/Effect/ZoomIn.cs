using System;
using DG.Tweening;
using UnityEngine;

public class ZoomIn : MonoBehaviour
{
    public float duration;
    public float delay;
    public Ease ease;
    
    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, duration).SetDelay(delay).SetEase(ease);
    }
}
