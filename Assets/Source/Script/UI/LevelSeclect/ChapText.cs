using System;
using TMPro;
using UnityEngine;

public class ChapText : MonoBehaviour
{
    private TextMeshProUGUI chap;

    private void Start()
    {
        chap = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        chap.text = "Chap " + LevelButtonHolder.CURRENT_CHAP;
    }
}
