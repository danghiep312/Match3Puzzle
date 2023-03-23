using System;
using UnityEngine;


public class LevelSelectPanel : MonoBehaviour
{
    private void Update()
    {
        GameManager.Instance.AcceptInput = !transform.gameObject.activeSelf;
    }
}
