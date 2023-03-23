using DG.Tweening;
using UnityEngine;


public class LosePanel : MonoBehaviour
{
    private RectTransform rt;

    public GameObject reviveButton;
    public GameObject retryButton;
    

    private void OnEnable()
    {
        rt = GetComponent<RectTransform>();

        reviveButton.SetActive(GameManager.Instance.CanRevive);
        retryButton.SetActive(!GameManager.Instance.CanRevive);
    }

    private void OnDisable()
    {
        rt.localScale = Vector3.zero;
    }
    
    public void Disappearance()
    {
        rt.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            transform.parent.gameObject.SetActive(false);
        });
    }

    public void Revive()
    {
        rt.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            GameManager.Instance.Revive();
            transform.parent.gameObject.SetActive(false);
        });
    }
}
