using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Tile : MonoBehaviour
{
    public int id;
    public bool isSelected;
    public bool inQueue;

    public bool isMoving;
    public bool isCombining;
    public bool isLocking;
    public SpriteRenderer offsetSr;
    public SpriteRenderer sr;
    public Collider2D col;
    
    
    public int originalSortingOrder;
    public Vector3 pivotPosInBoard;
    [ShowInInspector]
    public Pos posInBoard;

    [SerializeField] private AnimationCurve curve;
    public static Vector3 ORIGINAL_SCALE;
    public static float TIME_TO_REARRANGE = 0.3f;
    [ShowInInspector]
    public static float TIME_TO_MOVE_QUEUE = 0.6f;
    
    private void Start()
    {
        this.RegisterListener(EventID.AddToQueue, (param) => OnTileAddToQueue((int)param));
       // this.RegisterListener(EventID.Revive, (param) => OnRevive());
    }

    public void Init(Pos posInMatrix, Vector3 pos, int sortingOrder, int idTile, GameObject board)
    {
        transform.localScale = ORIGINAL_SCALE;
        originalSortingOrder = sr.sortingOrder = sortingOrder;
        transform.parent = board.transform.GetChild(posInMatrix.z);
        pivotPosInBoard = transform.localPosition = pos;
        posInBoard = posInMatrix;
        id = idTile;
        gameObject.name = "Tile " + posInMatrix.x + " " + posInMatrix.y + " " + posInMatrix.z;
    }

    private void Update()
    {
        sr.sprite = SpriteMachine.Instance.itemSprites[id - 1];
        offsetSr.sprite = SpriteMachine.Instance.itemSprites[id - 1];
        if (isLocking)
        {
            //if (Physics2D.OverlapCollider(col, filter, collidedObjects) == 0)
            // if (Physics2D.OverlapCollider(col, filter, collidedObjects) == 0)
            // {
            //     isLocking = false;
            // }
            offsetSr.enabled = true;
            offsetSr.sortingOrder = sr.sortingOrder + 1;
        }
        else
        {
            offsetSr.enabled = false;
        }
        

        if (!GameManager.Instance.AcceptInput) return;
        int i = 0;
        while (i < Input.touchCount)
        {
            Touch t = Input.GetTouch(i);
            if (t.phase == TouchPhase.Began)
            {
                var pos = Camera.main.ScreenToWorldPoint(t.position);
                pos.z = 0;
                var colliderHit = Physics2D.OverlapPointAll(pos);
                if (colliderHit.Contains(col))
                {
                    Pressed();
                }
            }

            i++;
        }
    }

    #region Build map blueprint

    [Button("Delete")]
    public void Delete()
    {
        this.PostEvent(EventID.Delete, gameObject.name);
        gameObject.SetActive(false);
    }

    [Button("Return")]
    public void Return()
    {
        this.PostEvent(EventID.Return, gameObject);
        gameObject.SetActive(true);
    }

    #endregion

    public void Combine(Vector3 posTarget)
    {
        var timeToDisappear = 0.2f;
        // transform.DOScale(Vector3.zero, timeToDisappear).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
        // {
        //     this.PostEvent(EventID.CombineComplete, gameObject);
        //     ObjectPooler.Instance.ReleaseObject(gameObject);
        // });
        
        transform.DOMove(posTarget, timeToDisappear).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
        {
            this.PostEvent(EventID.CombineComplete, gameObject);
            ObjectPooler.Instance.ReleaseObject(gameObject);
        });
    }

    public void CombineProcess(int index) // -1 or 1
    {
        var targetPos = transform.localPosition + Vector3.right * -index;
        transform.DOLocalMove(Vector3.right * (index * 0.35f), 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            transform.DOLocalMove(targetPos, 0.15f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                if (index == 0) GameManager.Instance.PlayCombineParticles(transform.position);
                this.PostEvent(EventID.CombineComplete, gameObject);
                ObjectPooler.Instance.ReleaseObject(gameObject);
            });
        });
    }
    
    public void Pressed()
    {
        if (inQueue || !GameManager.Instance.AcceptInput || isMoving) return;
        if (isLocking)
        {
            //Common.Log("Locking");
            return;
        }

        if (!GameManager.Instance.tutorialComplete)
        {
            if (!gameObject.name.Equals(Tutorial.tilesForTutorial[0].gameObject.name)) return;
        }

        AudioManager.Instance.Play("Pickup");
        isSelected = true;

        if (GameManager.Instance.vibrationOn)
        {
            MMVibrationManager.Haptic(HapticTypes.Selection);
            //Handheld.Vibrate();
        }
        this.PostEvent(EventID.ClickToTile, gameObject);
    
    }


    // private void OnMouseDown()
    // {
    //     return;
    //     if (inQueue || !GameManager.Instance.AcceptInput) return;
    //     if (isLocking)
    //     {
    //         Common.Log("Locking");
    //         return;
    //     }
    //     
    //     isSelected = true;
    //     gameObject.layer = LayerMask.NameToLayer("Freedom");
    //     this.PostEvent(EventID.ClickToTile, gameObject);
    // }

    public void DirectionClick()
    {
        isLocking = false;
        isSelected = true;
        this.PostEvent(EventID.ClickToTile, gameObject);
    }
    
    private void OnTileAddToQueue(int index)
    {
        if (isSelected)
        {
            Common.Log(gameObject.name);
            isSelected = false;
            isMoving = true;
            transform.DOScale(Vector3.one, TIME_TO_MOVE_QUEUE).SetUpdate(true);
            // // transform.DOLocalMove(Vector3.zero, TIME_TO_MOVE_QUEUE).SetEase(Ease.OutExpo).SetUpdate(true).OnComplete(() =>
            // // {
            // //     inQueue = true;
            // // });
            // transform.DOLocalJump(Vector3.zero, 1, 1, TIME_TO_MOVE_QUEUE).OnComplete(() =>
            // {
            //     inQueue = true;
            // });

            transform.DOLocalMoveX(0, TIME_TO_MOVE_QUEUE).SetEase(Ease.OutQuad);
            transform.DOLocalMoveY(0, TIME_TO_MOVE_QUEUE).SetEase(curve).OnComplete(() =>
            {
                inQueue = true;
            });
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer < gameObject.layer) return;
        isLocking = true;
    }
    

    private void OnDisable()
    {
        isSelected = false;
        inQueue = false;
        isCombining = false;
        isMoving = false;
        transform.localScale = Vector3.one;
        sr.color = Color.white;
    }

    // private Vector3 GetPosInQueue(int index)
    // {
    //     return Vector3.up * 0.25f + Vector3.right * (index * 1.1f - 3.35f);
    // }
    //
    // public void MoveToAssignedPosInQueue()
    // {
    //     var pos = GetPosInQueue(indexInQueue);
    //     transform.DOLocalMove(pos, TIME_TO_REARRANGE).SetEase(Ease.Linear).SetUpdate(true);
    // }

    public void MoveToPos(Vector3 pos)
    {
        transform.DOLocalMove(pos, .1f).SetEase(Ease.OutExpo).SetUpdate(true);
    }

    public bool CheckConditionToReturn()
    {
        return !isCombining;
    }
    
    public void ReturnToBoard()
    {
        if (!inQueue)
        {
            transform.DOKill();
        }
        MoveToPos(pivotPosInBoard);
        isMoving = true;
        transform.DOScale(ORIGINAL_SCALE, .1f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            isMoving = false;
        });
        inQueue = false;
        sr.sortingOrder = originalSortingOrder;
    }

    public void GameOverProcess()
    {
        float delayTime = (MapGenerator.Instance.high - posInBoard.z) * Random.Range(0.08f, 0.2f) +
                          (MapGenerator.Instance.row - posInBoard.x) * Random.Range(0.01f, 0.02f);
        var pos = transform.localPosition;
        transform.DOLocalMove(new Vector3(pos.x, pos.y + 1f, 0), 0.2f)
            .SetDelay(delayTime).SetEase(Ease.OutQuart).OnComplete(() =>
            {
               transform.DOLocalMove(new Vector3(pos.x, pos.y - 21f, 0), 0.5f)
                    .SetEase(Ease.InQuart);
            });
    }

    private void OnRevive()
    {
        transform.DOKill();
        transform.localPosition = pivotPosInBoard;
        transform.localScale = ORIGINAL_SCALE;
        if (inQueue)
        {
            transform.localPosition = Vector3.zero;
        }
        
    }

    public void Appearance(int row, int column)
    {
        float timeToMove = .3f;
        var delayTime = Random.Range(0.03f, 0.07f) * (Mathf.Abs(row / 2 - posInBoard.x) + Mathf.Abs(column / 2 - posInBoard.y));
        transform.localPosition = Vector3.zero;
        //transform.transform.rotation = Quaternion.Euler(Vector3.forward * Random.Range(0, 45));
        transform.DOLocalMove(pivotPosInBoard, timeToMove).SetEase(Ease.OutSine).SetDelay(delayTime);
        transform.DORotate(
            Vector3.forward * 360f * (Random.Range(0, 2) == 0 ? 1 : -1) - transform.transform.rotation.eulerAngles,
            timeToMove,
            RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetDelay(delayTime);
    }
}