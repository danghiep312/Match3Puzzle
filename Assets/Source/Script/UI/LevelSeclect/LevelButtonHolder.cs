using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;


public class LevelButtonHolder : MonoBehaviour
{
    private RectTransform anchor;
    public GameObject buttonPrefab;
    public List<LevelButton> levelButtons;
    public static int row = 5;
    public static int col = 4;
    [ShowInInspector]
    public static int CURRENT_CHAP;
    public Vector2 spacing = new Vector2(50f, 60f);
    public int totalLevel;
    
    public void Start()
    {
        WaitToEnd();
    }

    private async void WaitToEnd()
    {
        //await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
        Debug.Log("Call this func");
        CURRENT_CHAP = PlayerPrefs.GetInt("MaxLevel", 1) / (row * col) + 1;
        anchor = transform.parent.GetComponent<RectTransform>();
        print(anchor.rect.width + " " + anchor.rect.height);
        // 222 and 270 is original size of button
        var size = anchor.rect.size;
        var scaleX = (float) Math.Round((size.x - spacing.x * (col - 1)) / (col * 222f), 2);
        var scaleY = (float) Math.Round((size.y - spacing.y * (row - 1)) / (row * 270f), 2);
        var sizeScale = Math.Min(scaleX, scaleY);
        Debug.Log(sizeScale);
        for (int i = 0; i < row * col; i++)
        {
            var go = Instantiate(buttonPrefab, transform);
            var button = go.GetComponent<LevelButton>();
            levelButtons.Add(button);
            button.Init(sizeScale, i, spacing);
        }
    }

    public void NextChap()
    {
        var maxChap = totalLevel / (col * row) + 1;
        if (CURRENT_CHAP == maxChap) return;
        CURRENT_CHAP++;
        RecheckStatusButtonLevel();
    }

    public void PreviousChap()
    {
        if (CURRENT_CHAP == 1) return;
        CURRENT_CHAP--;
        RecheckStatusButtonLevel();
    }

    private void RecheckStatusButtonLevel()
    {
        foreach (var button in levelButtons)
        {
            button.SetStatus(CURRENT_CHAP);
            button.gameObject.SetActive(button.lv <= MapGenerator.Instance.levelsData.Length);
        }
    }
}
