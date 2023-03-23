using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LevelButton : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private Button btn;
    private int index;
    public int lv;
    public GameObject starZone;
    public TextMeshProUGUI levelText;
    public Image levelPanel;
    public GameObject lockedIcon;
    public GameObject playingIcon;
    private void Start()
    {
        rt = GetComponent<RectTransform>();
        btn = GetComponent<Button>();
        btn.onClick.AddListener(Click);
    }

    public void Init(float sizeScale, int index, Vector2 spacing)
    {
        if (rt == null) rt = GetComponent<RectTransform>();
        rt.localScale = Vector3.one * sizeScale;
        var x = index % 4;
        var y = index / 4;
        rt.anchoredPosition =  Vector2.right * (spacing.x + rt.sizeDelta.x * rt.localScale.x) * x  
                               + Vector2.up * -(spacing.y + rt.sizeDelta.y * rt.localScale.y) * y;
        this.index = index;
        SetStatus(LevelButtonHolder.CURRENT_CHAP);
    }

    private void OnEnable()
    {
        SetStatus(LevelButtonHolder.CURRENT_CHAP);
    }

    public void SetStatus(int chap)
    {
        starZone.SetActive(false);
        playingIcon.SetActive(false);
        lockedIcon.SetActive(false);
        lv = LevelButtonHolder.row * LevelButtonHolder.col * (chap - 1) + (index + 1);
        
        levelText.text = lv + "";
        if (lv == LevelManager.MAX_LEVEL)
        {
            levelPanel.sprite = SpriteMachine.Instance.buttonLevelPlaying;
            playingIcon.SetActive(true);
        }
        else if (lv < LevelManager.MAX_LEVEL)
        {
            levelPanel.sprite = SpriteMachine.Instance.buttonLevelPassed;
            starZone.SetActive(true);
        }
        else
        {
            levelPanel.sprite = SpriteMachine.Instance.buttonLevelLocked;
            lockedIcon.SetActive(true);
        }
    }

    private void Click()
    {
        if (lockedIcon.activeSelf) return;
        MapGenerator.Instance.DeleteSaveFile();
        transform.parent.parent.parent.gameObject.SetActive(false);
        GameManager.Instance.PlayGame(lv);
    }
}
