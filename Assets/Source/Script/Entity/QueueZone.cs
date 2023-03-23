 using System;
using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
 using Sirenix.OdinInspector;
using UnityEngine;


public class QueueZone : MonoBehaviour
{
    [ShowInInspector]
    public static bool full;

    public int limit;
    public RectTransform queuePivot;
    public GameObject[] queue;
    public GameObject[] seats;
    public GameObject combineQueue;
    public SpriteRenderer sr;

    private void OnEnable()
    {
        // sr.DOFade(0, 0);
        // transform.position = Vector3.up * -12f;
        // transform.DOMove(queuePivot.transform.position, 0.6f).SetDelay(0.2f).SetEase(Ease.OutBack);
        // sr.DOFade(1, 0.6f).SetDelay(0.2f).SetEase(Ease.Linear).OnComplete(() =>
        // {
        //     UIManager.Instance.SetStatusGamePlayPanel(true);
        // });
}

    private void Start()
    {
        seats = new GameObject[transform.childCount - 2];
        queue = new GameObject[seats.Length];
        for (int i = 0; i < transform.childCount - 2; i++)
        {
            seats[i] = transform.GetChild(i).gameObject;
            seats[i].transform.localPosition = Vector3.right * (i - 3.5f) + Vector3.up * 0.1f;
        }
        this.RegisterListener(EventID.ClickToTile, (param) => OnClickToTile(param as GameObject));
        this.RegisterListener(EventID.Restart, (param) => Restart());
        this.RegisterListener(EventID.Retry, (param) => Restart());
        this.RegisterListener(EventID.CombineComplete, (param) => OnCombineComplete(param as GameObject));
        //this.RegisterListener(EventID.Win, (param) => OnWin());
        //this.RegisterListener(EventID.GameOver, (param) => OnLose());
        this.RegisterListener(EventID.PlayGame, (param) => OnPlayGame());
        this.RegisterListener(EventID.GoHome, (param) => Restart());
        this.RegisterListener(EventID.PrepareGoHome, (param) => OnWin());
        this.RegisterListener(EventID.AddSlot, (param) => OpenExtraSeat());

        full = false;
        limit = 7;
    }
    

    public Pos[] GetCurrentPosTileInQueue()
    {
        var ids = new Pos[queue.Length];
        for (int i = 0; i < ids.Length; i++)
        {
            if (queue[i] != null) ids[i] = queue[i].GetComponent<Tile>().posInBoard;
        }

        return ids;
    }

    private void OnLose()
    {
        transform.DOMove(Vector3.up * -12f, 0.6f).SetEase(Ease.InBack);
    }

    private void OnPlayGame()
    {
        limit = 7;
        full = false;
        //transform.DOMove(queuePivot.transform.position, 0.6f).SetDelay(0.3f).SetEase(Ease.OutBack);
        Restart();
    }

    private void OnWin()
    {
        //transform.DOMove(Vector3.up * -12f, 0.6f).SetEase(Ease.InBack);
        Restart();
    }

    private void OnClickToTile(GameObject tile)
    {
        if (!full)
        {
            tile.GetComponent<SpriteRenderer>().sortingOrder = 400;
            AddToQueue(tile);
        }
    }

    public void Restart()
    {
        full = false;
        for (int i = 0; i < queue.Length; i++)
        {
            queue[i] = null;
        }
    }
    
    private void AddToQueue(GameObject tile)
    {
        var index = FindPositionAvailableInQueue(tile.GetComponent<Tile>().id);
        if (!CheckPositionInQueueAvailable(index))
        {
            for (int i = queue.Length - 1; i > index; i--)
            {
                if (queue[i - 1] != null && queue[i - 1].activeSelf)
                {
                    queue[i] = queue[i - 1];
                }
            }

            queue[index] = null;
            MoveSeatToAssignedPos();
        }

        queue[index] = tile;
        tile.transform.parent = GetSeatAvailable(index).transform;
        RearrangeSortingOrderQueue();
        this.PostEvent(EventID.AddToQueue, index);
        
        if (CountNumOfTileInQueue(tile.GetComponent<Tile>().id) == 3)
        {
            var tileSet = new TileSet(ref queue[index - 2], ref queue[index - 1], ref queue[index]);
            combineQueue.GetComponent<CombineQueue>().AddToCombineQueue(tileSet);
        }

        CheckGameOver();
    }

    private IEnumerator AfterGameOver(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        this.PostEvent(EventID.GameOver);
        AudioManager.Instance.Play("Lose");
    }

    private int FindPositionAvailableInQueue(int id)
    {
        var tmpIndex = 0;
        for (var i = queue.Length - 1; i >= 0; i--)
        {
            if (queue[i] == null) continue;
            tmpIndex = i + 1;
            break;
        }
        
        for (var i = queue.Length - 1; i >= 0; i--)
        {
            if (queue[i] != null && queue[i].GetComponent<Tile>().id == id)
            {
                if (!queue[i].GetComponent<Tile>().isCombining) return i + 1;
            }
        }
        return tmpIndex;
    }
    
    private bool CheckPositionInQueueAvailable(int index)
    {
        return queue[index] == null || !queue[index].activeSelf;
    }
    
    public void RearrangeSortingOrderQueue()
    {
        for (int i = 0; i < queue.Length; i++)
        {
            if (queue[i] != null && queue[i].activeSelf)
            {
                queue[i].GetComponent<SpriteRenderer>().sortingOrder = 200 + i;
            }
        }
    }

    private int CountNumOfTileInQueue(int id)
    {
        int cnt = 0;
        Array.ForEach(queue, tile =>
        {
            if (tile != null && tile.GetComponent<Tile>().id == id && !tile.GetComponent<Tile>().isCombining) cnt++;
        });
        return cnt;
    }


    private int GetIndexOfTileInQueue(GameObject tile)
    {
        for (int i = 0; i < queue.Length; i++)
        {
            if (queue[i] != null && tile.name.Equals(queue[i].name)) return i;
        }

        
        return -1;
    }
    public void MoveSeatToAssignedPos()
    {

        foreach (var seat in seats)
        {
            if (seat.transform.childCount > 0)
            {
                var tile = seat.transform.GetChild(0).gameObject;
                var index = GetIndexOfTileInQueue(tile);
                // TODO: reformat
                var pos = Vector3.right * (index * 1f - 3.5f) + Vector3.up * 0.1f;
                seat.transform.DOLocalMove(pos, Tile.TIME_TO_REARRANGE).SetEase(Ease.OutQuad);
            }
            
        }
    }

    public GameObject GetSeatAvailable(int index)
    {
        foreach (var seat in seats)
        {
            if (seat.transform.childCount == 0)
            {
                // TODO: reformat
                seat.transform.localPosition = Vector3.right * (index * 1f - 3.5f) + Vector3.up * 0.1f;
                return seat;
            }
        }
        return null;
    }

    private void OnCombineComplete(GameObject tile)
    {
        PopOutTileOfTheQueue(tile);
        WaitToRearrange();
    }

    public void RearrangeQueue()
    {
//        Debug.Log("Rearrange");
        int index = 0;
        foreach (var tile in queue)
        {
            if (tile != null && tile.activeSelf)
            {
                queue[index] = tile;
                index++;
            }
        }
        for (var i = index; i < queue.Length; i++)
        {
            queue[i] = null;
        }
        
        MoveSeatToAssignedPos();
        RearrangeTiming(Tile.TIME_TO_REARRANGE * 2f);
    }
 
    private async void RearrangeTiming(float seconds)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(seconds));
        GameManager.Instance.AreCombining = false;
    }

    public void PopOutTileOfTheQueue(GameObject tile)
    {
        for (int i = 0; i < queue.Length; i++)
        {
            if (queue[i] != null && queue[i].name.Equals(tile.name))
            {
                queue[i] = null;
                break;
            }
        }
    }

    public void CheckGameOver()
    {
        if (!GameManager.Instance.Playing) return;
        var numOfType = new int[20];
        foreach (var tile in queue)
        {
            if (tile != null)
            {
                numOfType[tile.GetComponent<Tile>().id]++;
            }
        }

        var isPendingTile = numOfType.Sum(num => num % 3);
        if (isPendingTile >= limit)
        {
            GameManager.Instance.Playing = false;
            GameManager.Instance.AcceptInput = false;
            full = true;
            //GameOverProcess(Tile.TIME_TO_MOVE_QUEUE);
            StartCoroutine(AfterGameOver(1f));
        }
    }

    async void WaitToRearrange()
    {
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        RearrangeQueue();
    }

    async void GameOverProcess(float second)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(second));
        MapGenerator.Instance.GameOverProcess();
        Array.ForEach(queue, tile =>
        {
            if (tile == null) return;
            tile.GetComponent<Tile>().GameOverProcess();
        });
    }

    public void OpenExtraSeat()
    {
        limit = 8;
    }
}
