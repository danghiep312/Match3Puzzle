using System;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.UI;


public class WinPanel : MonoBehaviour
{
    private RectTransform rt;
    public GameObject[] stars;
    private Image[] images;
    [SerializeField] private Ease ease;
    [SerializeField] private float timeToMove;


    private void Start()
    {
        images = new Image[stars.Length];
        for (int i = 0; i < stars.Length; i++)
        {
            images[i] = stars[i].GetComponent<Image>();
        }
    }

    private void OnEnable()
    {
        var mapInstance = MapGenerator.Instance;
        var star = (int) ((mapInstance.numOfTileComplete * 1f/ mapInstance.totalTile) / 0.3);

        GetComponentInParent<Fade>().Executive();
        rt = GetComponent<RectTransform>();
        
        rt.anchoredPosition = Vector2.right * 2000f;
        rt.DOAnchorPos(Vector2.zero, 0.5f).SetDelay(0.3f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            for (int i = 0; i < star; i++)
            {
                stars[i].transform.DOScale(Vector3.one, timeToMove).SetEase(ease).SetDelay(timeToMove * i);
                images[i].color = new Color(1, 1, 1, 0);
                images[i].DOFade(1f, timeToMove).SetEase(Ease.Linear).SetDelay(timeToMove * i);
                stars[i].SetActive(true);
            }
        });
        
    }

    private void OnDisable()
    {

        rt.anchoredPosition = Vector2.right * 2000f;
       
        for (int i = 0; i < 3; i++)
        {
            stars[i].transform.DOKill();
            stars[i].transform.localScale = Vector3.one * 2f;
            images[i].DOFade(0, 0);
            stars[i].SetActive(false);
        }
    }
    
    public void Disappearance()
    {
        rt.DOAnchorPos(Vector2.right * 2000f, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            
            transform.parent.gameObject.SetActive(false);
        });
    }
}
