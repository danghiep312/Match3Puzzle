using System;
using UnityEngine;


public class CameraFitBackground : MonoBehaviour
{
    public SpriteRenderer risk;

    private void Start()
    {
        float orthoSize = risk.bounds.size.x * Screen.height / Screen.width * 0.5f;
        Camera.main.orthographicSize = orthoSize;
    }
}
