using System;
using UnityEngine;
using UnityEngine.UI;


public class SoundButton : MonoBehaviour
{
    [Tooltip("0 is off, 1 is on")]
    public Sprite[] soundSprites;
    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        _image.sprite = soundSprites[AudioManager.Instance.soundOn ? 1 : 0];
    }
}
