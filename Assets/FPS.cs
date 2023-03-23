using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    public Text fps;
    void Start()
    {
#if PLATFORM_ANDROID
        gameObject.SetActive(false);
#endif
#if UNITY_EDITOR
        gameObject.SetActive(true);
#endif
        fps = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        fps.text = ((int)(1 / Time.deltaTime)).ToString();
    }
}
