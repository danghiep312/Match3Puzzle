using DG.Tweening;
using UnityEngine;


public class Scale : MonoBehaviour
{
    public float duration;
    public float delay;
    
    public Vector3 targetScale;
    public Vector3 originalScale;

    public Ease ease;
    public bool offOnComplete;
    
    public void Executive()
    {
        originalScale = transform.localScale;
        transform.DOScale(targetScale, duration).SetDelay(delay).SetEase(ease).SetUpdate(true).OnComplete(() =>
        {
            if (offOnComplete)
            {
                gameObject.SetActive(false);
                transform.localScale = originalScale;
            }
        });
    }
    

}
