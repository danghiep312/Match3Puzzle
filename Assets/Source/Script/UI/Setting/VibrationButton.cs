using System;
using UnityEngine;
using UnityEngine.UI;


public class VibrationButton : MonoBehaviour
{
    [Tooltip("0 is off, 1 is on")]
    public Sprite[] vibrationSprites; // 0 is off, 1 is on
    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        _image.sprite = vibrationSprites[GameManager.Instance.vibrationOn ? 1 : 0];
    }
    
}
