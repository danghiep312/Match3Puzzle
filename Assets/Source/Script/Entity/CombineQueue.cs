using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class TileSet
{
    public bool areCombining;
    public Tile[] tiles = new Tile[3];
    
    public TileSet(ref GameObject tile1, ref GameObject tile2, ref GameObject tile3)
    {
        tiles[0] = tile1.GetComponent<Tile>();
        tiles[1] = tile2.GetComponent<Tile>();
        tiles[2] = tile3.GetComponent<Tile>();
        Array.ForEach(tiles, tile => tile.isCombining = true);
        areCombining = false;
    }

    public void ReleaseTileSet()
    {
        Array.ForEach(tiles, tile => ObjectPooler.Instance.ReleaseObject(tile.gameObject));
    }
    
    public bool ReadyToCombine()
    {
        return tiles[0].inQueue && tiles[1].inQueue && tiles[2].inQueue;
    }

    public void Combine()
    {
        areCombining = true;
        var target = tiles[1].transform.position + Vector3.up * 0f;
        //Array.ForEach(tiles, tile => tile.Combine(target));
        for (int i = 0; i < 3; i++)
        {
            tiles[i].CombineProcess(i - 1);
        }
    }

   
}

public class CombineQueue : MonoBehaviour
{
    [ShowInInspector]
    private List<TileSet> tileList;

    private void OnEnable()
    {
        tileList?.Clear();
    }

    private void OnDisable()
    {
        tileList?.Clear();
    }

    private void Start()
    {
        tileList = new List<TileSet>();
        this.RegisterListener(EventID.CombineComplete, (param) => OnCombineComplete((GameObject) param));
        this.RegisterListener(EventID.PlayGame, (param) => OnPlayGame());
    }

    private void OnPlayGame()
    {
        tileList?.Clear();
    }

    private void Update()
    {
        if (GameManager.Instance.AreCombining) return;
        if (CheckCombineQueueHaveTile())
        {
            CombineProcess();
        }
    }

    private void CombineProcess()
    {
        var haveCombineProcess = false;
        foreach (var tileSet in tileList.Where(tileSet => tileSet.ReadyToCombine()))
        {
            AudioManager.Instance.Play("Combine");
            haveCombineProcess = true;
            tileSet.Combine();
            // foreach (var tile in tileSet.tiles)
            // {
            //     GetComponentInParent<QueueZone>().PopOutTileOfTheQueue(tile.gameObject);
            // }
        }
        
        if (haveCombineProcess) GameManager.Instance.AreCombining = true;
    }
    
    public bool CheckCombineQueueHaveTile()
    {
        return tileList.Count > 0;
    }

    private void OnCombineComplete(GameObject tile)
    {
        GetComponentInParent<QueueZone>().PopOutTileOfTheQueue(tile);
    }
    
    public void AddToCombineQueue(TileSet tileSet)
    {
        tileList.Add(tileSet);
    }
}
