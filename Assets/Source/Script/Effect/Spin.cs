using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float duration;
    public float delay;
    
    public void Executive()
    {
        StartCoroutine(Wait(delay));
    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        transform.DORotate(Vector3.forward * 360f, duration, RotateMode.WorldAxisAdd).SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }
}
