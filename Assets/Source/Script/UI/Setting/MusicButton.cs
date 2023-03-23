using System;
using UnityEngine;
using UnityEngine.UI;


public class MusicButton : MonoBehaviour
{
    [Tooltip("0 is off, 1 is on")]
    public Sprite[] musicSprites;
    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        _image.sprite = musicSprites[AudioManager.Instance.musicOn ? 1 : 0];
    }
}