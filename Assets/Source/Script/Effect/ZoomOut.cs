using DG.Tweening;
using UnityEngine;


public class ZoomOut : MonoBehaviour
{
    public float duration;
    public float delay;
    public Ease ease;

    public bool wantToDisable;
    public GameObject goToDisable;
    
    public void Executive()
    {
        transform.DOScale(Vector3.zero, duration).SetDelay(delay).SetEase(ease).SetUpdate(true).OnComplete(() =>
        {
            if (wantToDisable) goToDisable.SetActive(false);
        });
    }
}
