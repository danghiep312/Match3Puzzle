using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    private DOTweenPath p;
    
    private void Start()
    {
        
    }

    [Button("Go")]
    public void Move()
    {
        List<Vector3> way = new List<Vector3>();
        way.Add(transform.position);
       
        way.Add(Vector3.up * 2f);
        way.Add(Vector3.zero);
        transform.DOPath(way.ToArray(), 5f, PathType.CatmullRom, PathMode.Ignore).SetEase(Ease.OutBounce);
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
    }
}
