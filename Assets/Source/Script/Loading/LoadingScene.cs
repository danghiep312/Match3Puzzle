using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class LoadingScene : MonoBehaviour
{
    public Image fill;
    public TextMeshProUGUI text;

    private void Start()
    {
        LoadScene("Source/Scenes/GameplayScene");
    }

    private void Update()
    {
        text.text = $"{Mathf.RoundToInt(fill.fillAmount * 100)}%";
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Source/Scenes/GameplayScene");

        float timeToLoad = 5f;
        while (!operation.isDone && timeToLoad > 0)
        {
            timeToLoad -= Time.deltaTime;
            fill.fillAmount = Mathf.Lerp(0, 1, 1 - timeToLoad);
            yield return null;
        }

    }
    
    private async void LoadScene(string sceneName)
    {
        var _realProgress = 0.0f;
        var _fakeProgress = 0.0f;
        fill.fillAmount = 0.0f;
 
        // Start loading scene but not activate it
        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;
 
        do
        {
            await UniTask.Delay(50); // Timeout between the 'ticks' of progress bar
            _fakeProgress += Random.Range(0.01f, 0.1f); // Value of one 'tick'
            fill.DOKill();
            fill.DOFillAmount(_fakeProgress, 0.05f).SetEase(Ease.OutSine);
        } while (_fakeProgress < 0.9f);
        // Using Random we can have the progress between 0.91 and 1.0 in the end of loop...
 
        //...so, set the progress bar value to 100%
        fill.fillAmount = 1f;
 
        // Meanwhile on the background we checking the real progress
        do
        {
            _realProgress = scene.progress;
        } while (_realProgress < 0.9f); // In Unity scene progress always between 0 and 0.9 ¯\_(ツ)_/¯
 
        scene.allowSceneActivation = true; // Now, activate the scene
    }
}
