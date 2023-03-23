using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Pos
{
    public int x, y, z;
    
    public Pos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

public class Board : MonoBehaviour
{
    public static float TIME_TO_SHUFFLE = 0.3f;
    
    public int row, col, high;
    public GameObject[,,] map;
    public Tile[,,] tileMap;
    public List<GameObject> listOfTile;
    public QueueZone queue;
    public GameObject[] q; // queue array
    
    public List<Pos> currentAvailablePos = new List<Pos>();
    public List<GameObject> listTileIsPendingToCombine = new List<GameObject>();
    public List<GameObject> listTileById = new List<GameObject>();
    public List<GameObject> listTileOfEachFloor = new List<GameObject>();

    
    private void Start()
    {
        this.RegisterListener(EventID.CombineComplete, (param) => OnCombineComplete());
        this.RegisterListener(EventID.PrepareGoHome, (param) => Disappearance());
        this.RegisterListener(EventID.PlayGame, (param) => OnPlayGame());
        this.RegisterListener(EventID.Revive, (param) => OnRevive());
        //this.RegisterListener(EventID.Restart, (param) => MoveFloorToOutsideCamera());
    }

    private void MoveFloorToOutsideCamera()
    {
        foreach (Transform t in transform)
        {
            t.localPosition = Vector3.up * -20f;
        }
    }

    private void OnRevive()
    {
        for (int i = q.Length - 1; i >= 0; i--)
        {
            if (q[i] != null)
            {
                ReturnWithoutSpell(q[i]);
            }
        }

        queue.Restart();
        RearrangeAndSortingOrder();
        MapGenerator.Instance.CheckStatusEachTile();
        
    }
    
    private void OnPlayGame()
    {
        transform.localPosition = Vector3.zero;
        //ArrangeFloor();
        AppearanceEffect();
        // TODO: Appearance effect
    }

    public void Disappearance()
    {
        transform.DOLocalMove(Vector3.left * 30f, 0.4f).SetEase(Ease.InBack);
    }
    

    private void OnCombineComplete()
    {
        StartCoroutine(AfterCombine());
    }

    private void Update()
    {
        q = queue.queue;
        row = MapGenerator.Instance.row;
        col = MapGenerator.Instance.col;
        high = MapGenerator.Instance.high;
        map = MapGenerator.Instance.map;
        tileMap = MapGenerator.Instance.tileMap;
    }

    private void CheckWinGame()
    {
        if (!GameManager.Instance.Playing) return;
        var totalChild = transform.Cast<Transform>().Sum(child => child.childCount) +
                         queue.transform.Cast<Transform>().Sum(child => child.childCount);
        
        if (totalChild == 0)
        {
            AudioManager.Instance.Play("Win");
            this.PostEvent(EventID.Win);
        }
    }

    private void OnWin()
    {
        MoveFloorToOutsideCamera();   
    }

    
    IEnumerator AfterCombine()
    {
        yield return new WaitForEndOfFrame();
        CheckWinGame();
    }

    IEnumerator EnableInputProcess(float time)
    {
        yield return new WaitForSeconds(time);
        GameManager.Instance.SetBlockInput(false);
    }

    [Button("Appearance")]
    public void AppearanceEffect()
    {
        
        foreach (Transform floor in transform)
        {
            foreach (Transform tile in floor)
            {
                tile.GetComponent<Tile>().Appearance(row, col);
            }
        }

        StartCoroutine(EnableInputProcess(1f));
    }
    
    #region Spell Function

    // public void Shuffle()
    // {
    //     Common.Log("Shuffle");
    //     this.PostEvent(EventID.Spell, 0.5f);
    //     GameManager.Instance.SetBlockInput(true);
    //     int[] countType = new int[10];
    //     foreach (Transform floor in transform)
    //     {
    //         foreach (Transform child in floor)
    //         {
    //             if (child.name.Contains("Tile"))
    //             {
    //                 listOfTile.Add(child.gameObject);
    //                 countType[child.GetComponent<Tile>().id]++;
    //             }
    //         }
    //     }
    //
    //     for (int i = 0; i < countType.Length; i++)
    //     {
    //         for (int j = 0; j < countType[i]; j++)
    //         {
    //             GetAndSetNewTypeForTile(listOfTile, i);
    //         }
    //     }
    //     
    //     ArrangeFloor();
    // }

    public void Shuffle()
    {
        if (BoosterManager.Instance.Shuffle <= 0) return;
        Common.Log("Shuffle");
        GameManager.Instance.SetBlockInput(true);
        listOfTile.Clear();
        currentAvailablePos.Clear();

        for (int k = 0; k < high; k++)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    var tile = map[i, j, k];
                    if (tile != null && tile.activeSelf && !tile.GetComponent<Tile>().inQueue &&
                        !tile.GetComponent<Tile>().isCombining && !tile.GetComponent<Tile>().isSelected)
                    {
                        currentAvailablePos.Add(new Pos(i, j, k));
                        listOfTile.Add(map[i, j, k]);
                    }
                }
            }
        }
        
        if (listOfTile.Count == 0) return;
        var dis = listOfTile[0].transform.localScale.x;
        Debug.Log(dis);
        while (listOfTile.Count > 0) {
            var randomPos = GetRandomPos();
            
            var pivotPos = new Vector3(
                -((col * 1f - 1) / 2 - randomPos.y) * dis + dis / 2 * randomPos.z,
                ((row * 1f - 1) / 2 - randomPos.x) * dis - dis / 2 * randomPos.z, -1 * randomPos.z);
            
            
            var tile = GetRandomTile();
            map[randomPos.x, randomPos.y, randomPos.z] = tile;
            MapGenerator.Instance.tileMap[randomPos.x, randomPos.y, randomPos.z] = tile.GetComponent<Tile>();
            //Debug.Log(tile.name + " pos " + randomPos.x + " "  + randomPos.y + " " + randomPos.z);
            tile.transform.parent = transform.GetChild(randomPos.z);
            tile.GetComponent<Tile>().posInBoard = randomPos;
            tile.GetComponent<Tile>().pivotPosInBoard = pivotPos;
            
            tile.transform.DOLocalMove(pivotPos, TIME_TO_SHUFFLE).SetEase(Ease.Linear);
        }
        
        RearrangeAndSortingOrder();
        queue.RearrangeSortingOrderQueue();
        
        BoosterManager.Instance.UseShuffle();
        this.PostEvent(EventID.Spell, 0.5f);
        StartCoroutine(EnableInputProcess(TIME_TO_SHUFFLE + 0.1f));
    }

    private void RearrangeAndSortingOrder()
    {
        int sOrder = 10;
        for (int k = 0; k < high; k++)
        {
            for (int i = 0; i < row - k; i++)
            {
                for (int j = 0; j < col - k; j++)
                {
                    if (map[i, j, k] != null && map[i, j, k].activeSelf)
                    {
                        map[i, j, k].GetComponent<SpriteRenderer>().sortingOrder = sOrder;
                        map[i, j, k].GetComponent<Tile>().originalSortingOrder = sOrder;
                        sOrder += 2;
                    }
                }
            }
        }
    }
    
    private Pos GetRandomPos()
    {
        int index = Random.Range(0, currentAvailablePos.Count);
        Pos pos = currentAvailablePos[index];
        currentAvailablePos.RemoveAt(index);
        return pos;
    }

    private GameObject GetRandomTile()
    {
        int index = Random.Range(0, listOfTile.Count);
        var tile = listOfTile[index];
        listOfTile.RemoveAt(index);
        return tile;
    }
    
    public void ArrangeFloor()
    {
        List<Vector3> listPos = new List<Vector3> { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
        List<int> listDir = new List<int>();
        List<int> preDir = new List<int>();
        int[] dir = new int[8];
        dir[0] = Random.Range(0, 4);
        for (int i = 1; i < 8; i++)
        {
            listDir.Clear();
            preDir.Clear();
            for (int j = i - 3; j < i; j++)
            {
                if (j < 0) continue;
                preDir.Add(dir[j]);
            }

            for (int j = 0; j < 4; j++)
            {
                if (!preDir.Contains(j)) listDir.Add(j);
            }
            
            dir[i] = listDir[Random.Range(0, listDir.Count)];
        }

        float appearDuration = .5f;
        for (int i = 0; i < 8; i++)
        {
            if (i == 7 || transform.GetChild(i + 1).childCount == 0)
            {
                StartCoroutine(EnableInputProcess(appearDuration + appearDuration/2 * i));
            }

            transform.GetChild(i).localPosition = listPos[dir[i]] * 20f;
            transform.GetChild(i).DOLocalMove(Vector3.zero, appearDuration).SetDelay(appearDuration / 2 * i)
                .SetEase(Ease.OutExpo);
            
        }
    }
    

    public void Hint()
    {
        if (GameManager.Instance.AreCombining || BoosterManager.Instance.Hint <= 0) return;
        Common.Log("Hint");
        
        int targetId = -1, num = 0;
       
        if (GetNumOfAvailableSeatInQueue() >= 2)
        {
            targetId = GetId1Streak();
            num = 1;
            if (targetId == -1)
            {
                targetId = GetId2Streak();
                num = targetId != -1 ? 2 : 0;
            }
        }
        else
        {
            targetId = GetId2Streak();
            num = targetId != -1 ? 2 : 0;
            if (targetId == -1) return;
        }
        

        // var listTileOfEachFloor = new List<GameObject>();
        // var listTileById = new List<GameObject>();
        // var listTileIsPendingToCombine = new List<GameObject>();
        listTileOfEachFloor.Clear();
        listTileById.Clear();
        listTileIsPendingToCombine.Clear();
        for (int k = high - 1; k >= 0; k--)
        {
            for (int i = 0; i < row - k; i++)
            {
                for (int j = 0; j < col - k; j++)
                {
                    if (map[i, j, k] != null && map[i, j, k].activeSelf && map[i, j, k].transform.IsChildOf(transform))
                    {
                        listTileOfEachFloor.Add(map[i, j, k]);
                    }
                }
            }

            if (listTileOfEachFloor.Count > 0 && targetId == -1)
            {
                targetId = listTileOfEachFloor[Random.Range(0, listTileOfEachFloor.Count)].GetComponent<Tile>().id;
                
            }
            
            
            listTileById.AddRange(GetListTileByIdFromList(listTileOfEachFloor, targetId));

            while (listTileById.Count > 0 && listTileIsPendingToCombine.Count != 3 - num)
            {
                listTileIsPendingToCombine.Add(GetRandomTileFromList(listTileById));
            }
            if (listTileIsPendingToCombine.Count == 3 - num) break;
            
        }

        string name = "";
        foreach (var tile in listTileIsPendingToCombine)
        {
            name += tile.name + " ";
            tile.GetComponent<Tile>().DirectionClick();
        }

        //Debug.Log(name);
        BoosterManager.Instance.UseHint();
        this.PostEvent(EventID.Spell, 0.5f);
    }

    [Button("Test 1")]
    private int GetId1Streak()
    {
        if (q[0] == null) return -1;
        for (int i = 0; i < 5; i++)
        {
            if (q[i] == null) return -1;
            if (i > 0 && q[i].GetComponent<Tile>().id == q[i - 1].GetComponent<Tile>().id) continue;
            if (q[i + 1] == null) return q[i].GetComponent<Tile>().id;
            if (q[i + 1].GetComponent<Tile>().id != q[i].GetComponent<Tile>().id) return q[i].GetComponent<Tile>().id;
        }

        return -1;
    }

    [Button("Test 2")]
    private int GetId2Streak()
    {
        for (int i = 0; i < q.Length + 1; i++)
        {
            if (q[i] == null) return -1;
            if (q[i + 1] != null)
            {
                if (q[i + 1].GetComponent<Tile>().id == q[i].GetComponent<Tile>().id)
                {
                    if (q[i + 2] == null)
                    {
                        return q[i].GetComponent<Tile>().id;
                    }

                    if (q[i + 2].GetComponent<Tile>().id == q[i + 1].GetComponent<Tile>().id)
                    {
                        i += 2;
                    }
                }
                    
            }
        }

        return -1;
    }

    private GameObject GetRandomTileFromList(List<GameObject> list)
    {
        if (list.Count == 0) return null;
        var index = Random.Range(0, list.Count);
        var result = list[index];
        list.RemoveAt(index);
        return result;
    }

    private int GetNumOfAvailableSeatInQueue()
    {
        int result = 0;
        for (int i = 0; i < 7; i++)
        {
            if (queue.queue[i] == null) result++;
        }

        return result;
    }

    private List<GameObject> GetListTileByIdFromList(List<GameObject> list, int id)
    {
        var result = new List<GameObject>();
        foreach (var tile in list)
        {
            if (tile.GetComponent<Tile>().id == id)
            {
                result.Add(tile);
            }
        }

        foreach (var tile in result)
        {
            list.Remove(tile);
        }

        return result;
    }

    private GameObject GetLastItemInQueueToReturn()
    {
        for (int i = q.Length - 1; i >= 0; i--)
        {
            if (q[i] != null && q[i].GetComponent<Tile>().CheckConditionToReturn())
            {
                var ob = q[i];
                q[i] = null;
                return ob;
            }
        }

        return null;
    }

    public void Return()
    {
        if (!GameManager.Instance.CanUndo || BoosterManager.Instance.Return <= 0) return;
        Common.Log("Return");
        var lastItem = GetLastItemInQueueToReturn();
        if (lastItem == null) return;
        var pos = lastItem.GetComponent<Tile>().posInBoard;
        map[pos.x, pos.y, pos.z] = lastItem;
        tileMap[pos.x, pos.y, pos.z] = lastItem.GetComponent<Tile>();
        lastItem.transform.parent = transform.GetChild(pos.z);
        lastItem.GetComponent<Tile>().ReturnToBoard();
        RearrangeAndSortingOrder();
        BoosterManager.Instance.UseReturn();
        this.PostEvent(EventID.Spell, 0f);
    }

    public void ReturnWithoutSpell(GameObject tile)
    {
        var pos = tile.GetComponent<Tile>().posInBoard;
        map[pos.x, pos.y, pos.z] = tile;
        tileMap[pos.x, pos.y, pos.z] = tile.GetComponent<Tile>();
        tile.transform.parent = transform.GetChild(pos.z);
        tile.GetComponent<Tile>().ReturnToBoard();
        RearrangeAndSortingOrder();
    }

    #endregion

    [Button("Test")]
    public void Test(int i, int j, int k)
    {
        if (map[i, j, k] == null)
        {
            Debug.Log("null");
        }
        else
        {
            Debug.Log(map[i, j, k].name);
        }
    }
    
}