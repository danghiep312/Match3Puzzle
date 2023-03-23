using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        AudioManager.Instance.Play("Click");
    }
}