using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class Tutorial : MonoBehaviour
{
    public static List<Tile> tilesForTutorial;
    public Transform finger;
    public float speed = 50f;

    private void Start()
    {
        this.RegisterListener(EventID.PlayGame, (param) => OnPlayGame((int)param));
        this.RegisterListener(EventID.ClickToTile, (param) => OnClickToTile((GameObject)param));
        
    }

    private void Update()
    {
        if (tilesForTutorial != null && tilesForTutorial.Count > 0)
            finger.position = Vector3.MoveTowards(finger.position, tilesForTutorial[0].transform.position,
                speed * Time.deltaTime);
    }

    private void OnClickToTile(GameObject tile)
    {
        tilesForTutorial.Remove(tile.GetComponent<Tile>());
        if (tilesForTutorial.Count == 0)
        {
            CompleteTutorial();
        }
        //finger.DOMove(tilesForTutorial[0].transform.position, 0.3f).SetEase(Ease.OutQuad);
    }

    private void OnPlayGame(int level)
    {
        if (level > 1 || GameManager.Instance.tutorialComplete) return;
       
        tilesForTutorial = MapGenerator.Instance.PrepareForTutorial();
        
        finger.position = tilesForTutorial[0].transform.position;
    }
    
    private void CompleteTutorial()
    {
        GameManager.Instance.tutorialComplete = true;
        PlayerPrefs.SetInt("Tutorial", 1);
        MapGenerator.Instance.RecheckStatusTile();
        gameObject.SetActive(false);
    }
}
