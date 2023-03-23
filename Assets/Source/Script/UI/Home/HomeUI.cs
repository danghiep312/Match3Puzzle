using System;
using System.IO;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : MonoBehaviour
{
    [ShowInInspector]
    public static float TIME_TO_TRANSITION = .7f;
    
    public GameObject icon;
    public GameObject playButton;
    public RectTransform settingButton;
    public RectTransform jigsawButton;
    public RectTransform noAdsButton;
    public RectTransform awardButton;
    public RectTransform heart;
    public RectTransform coin;
    public RectTransform screenButtonZone;

    public TextMeshProUGUI resumeText;

    private void OnEnable()
    {
        Appearance();
    }

    private void Start()
    {
        this.RegisterListener(EventID.GoHome, (param) => Appearance());
    }

    private void Appearance()
    {
        icon.transform.localScale = Vector3.one * 1.4f;
        icon.GetComponent<Image>().color = new Color(1, 1, 1, 0);

        icon.GetComponent<Image>().DOFade(1, TIME_TO_TRANSITION).SetEase(Ease.Linear);
        icon.transform.DOScale(Vector3.one, TIME_TO_TRANSITION).SetEase(Ease.OutBack);

        playButton.transform.localScale = Vector3.one * 1.4f;
        playButton.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        playButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 0);

        playButton.transform.DOScale(Vector3.one, TIME_TO_TRANSITION).SetEase(Ease.OutBack);
        playButton.GetComponent<Image>().DOFade(1f, TIME_TO_TRANSITION).SetEase(Ease.Linear);
        playButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().DOFade(1f, TIME_TO_TRANSITION).SetEase(Ease.Linear);

        screenButtonZone.anchoredPosition = new Vector2(screenButtonZone.anchoredPosition.x, -400f);
        screenButtonZone.DOAnchorPos(new Vector2(screenButtonZone.anchoredPosition.x, 150f), TIME_TO_TRANSITION)
            .SetDelay(0.1f).SetEase(Ease.OutBack);

        settingButton.anchoredPosition = new Vector2(settingButton.anchoredPosition.x, 400f);
        settingButton.DOAnchorPos(new Vector2(settingButton.anchoredPosition.x, -100f), TIME_TO_TRANSITION)
            .SetDelay(0.5f);
        
        coin.anchoredPosition = new Vector2(coin.anchoredPosition.x, 400f);
        coin.DOAnchorPos(new Vector2(coin.anchoredPosition.x, -100f), TIME_TO_TRANSITION).SetDelay(0.4f)
            .SetEase(Ease.OutBack);
        
        heart.anchoredPosition = new Vector2(heart.anchoredPosition.x, 400f);
        heart.DOAnchorPos(new Vector2(heart.anchoredPosition.x, -100), TIME_TO_TRANSITION).SetDelay(0.3f);
        
        jigsawButton.anchoredPosition = new Vector2(jigsawButton.anchoredPosition.x + 400f, jigsawButton.anchoredPosition.y);
        jigsawButton.DOAnchorPos(new Vector2(-100f, jigsawButton.anchoredPosition.y), TIME_TO_TRANSITION).SetDelay(0.5f)
            .SetEase(Ease.OutBack);
        
        noAdsButton.anchoredPosition = new Vector2(noAdsButton.anchoredPosition.x - 400f, noAdsButton.anchoredPosition.y);
        noAdsButton.DOAnchorPos(new Vector2(100f, noAdsButton.anchoredPosition.y), TIME_TO_TRANSITION).SetDelay(0.5f)
            .SetEase(Ease.OutBack);
        
        awardButton.anchoredPosition = new Vector2(awardButton.anchoredPosition.x - 400f, awardButton.anchoredPosition.y);
        awardButton.DOAnchorPos(new Vector2(100f, awardButton.anchoredPosition.y), TIME_TO_TRANSITION).SetDelay(0.4f)
            .SetEase(Ease.OutBack);

        // resumeText.gameObject.SetActive(File.Exists(Application.persistentDataPath + "/CurrentState.bytes"));
        // resumeText.color = new Color(0.33f, 0.1f, 0f, 0f);
        // resumeText.DOFade(1f, TIME_TO_TRANSITION).SetDelay(0.5f).SetEase(Ease.Linear);
    }

    public void PlayGame()
    {
        icon.transform.DOScale(Vector3.one * 1.4f, TIME_TO_TRANSITION).SetDelay(0f).SetEase(Ease.InBack);
        icon.GetComponent<Image>().DOFade(0, TIME_TO_TRANSITION).SetDelay(0f).SetEase(Ease.Linear);
        
        playButton.transform.DOScale(Vector3.one * 1.4f, TIME_TO_TRANSITION).SetDelay(0f).SetEase(Ease.InBack);
        playButton.GetComponent<Image>().DOFade(0, TIME_TO_TRANSITION).SetDelay(0f).SetEase(Ease.Linear);
        playButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().DOFade(0, TIME_TO_TRANSITION).SetDelay(0f)
            .SetEase(Ease.Linear);

        screenButtonZone.DOAnchorPos(new Vector2(screenButtonZone.anchoredPosition.x, -400f), TIME_TO_TRANSITION)
            .SetDelay(0.1f)
            .SetEase(Ease.InBack);

        settingButton.DOAnchorPos(new Vector2(settingButton.anchoredPosition.x, 400f), TIME_TO_TRANSITION).SetDelay(0.3f)
            .SetEase(Ease.InBack);
        
        coin.DOAnchorPos(new Vector2(coin.anchoredPosition.x, 400f), TIME_TO_TRANSITION).SetDelay(0.4f)
            .SetEase(Ease.InBack);
        
        heart.DOAnchorPos(new Vector2(heart.anchoredPosition.x, 400f), TIME_TO_TRANSITION).SetDelay(0.5f)
            .SetEase(Ease.InBack);
        
        jigsawButton
            .DOAnchorPos(new Vector2(jigsawButton.anchoredPosition.x + 400f, jigsawButton.anchoredPosition.y), TIME_TO_TRANSITION)
            .SetDelay(0.3f)
            .SetEase(Ease.InBack);
        
        noAdsButton
            .DOAnchorPos(new Vector2(noAdsButton.anchoredPosition.x - 400f, noAdsButton.anchoredPosition.y), TIME_TO_TRANSITION)
            .SetDelay(0.3f)
            .SetEase(Ease.InBack);
        
        awardButton
            .DOAnchorPos(new Vector2(awardButton.anchoredPosition.x - 400f, awardButton.anchoredPosition.y), TIME_TO_TRANSITION)
            .SetDelay(0.4f)
            .SetEase(Ease.InBack);

        //resumeText.DOFade(0, TIME_TO_TRANSITION).SetEase(Ease.Linear);
    }
}
