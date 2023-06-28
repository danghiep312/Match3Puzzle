using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;


public class MapGenerator : Singleton<MapGenerator>
{
    [Header("Matrix data")]
    public int row;
    public int col;
    public int high;
    public int currentLevel;
    public GameObject[,,] map;
    public Tile[,,] tileMap;
    public int[,,] matrixLevel; 
    public List<GameObject> listOfTile;
    public int[] levelsData;

    [Header("Board data")]
    public GameObject prefab;
    public GameObject board;
    public RectTransform boardZone;
    public Vector2 boardSize;
    public int numOfTileComplete;
    
    [Header("Game data")]
    public bool valid;
    public int totalTile;
    public bool testing;
    public bool converting;
    
    //For check lock
    private int[] dirX = { 0, -1, -1, 0 };
    private int[] dirY = { -1, 0, -1, 0 };

    #region Init map

    private void Start()
    {
        if (!testing)
        {
            this.RegisterListener(EventID.PlayGame, (param) => PlayGame((int)param));
            this.RegisterListener(EventID.Spell, (param) => CheckStatusEachTile());
            this.RegisterListener(EventID.ClickToTile, (param) => OnClickToTile((GameObject)param));
            this.RegisterListener(EventID.CombineComplete, (param) => OnCombineComplete());
            this.RegisterListener(EventID.Win, (param) => DeleteSaveFile());
            this.RegisterListener(EventID.GameOver, (param) => DeleteSaveFile());
        }
        
        
        levelsData = JsonUtility.FromJson<Difficulty>(Resources.Load<TextAsset>("difficulty").text).difficulty;
        listOfTile = new List<GameObject>();
        int sOrder = 10;
        GetSizeOfBoard();
        
        if (testing) // Init map blueprint
        {
            board.transform.parent.position = boardZone.transform.position;
            this.RegisterListener(EventID.Delete, (param) => DeleteTile((string)param));
            this.RegisterListener(EventID.Return, (param) => ReturnTile((GameObject)param));
            var dis = GetDistanceEachTile();
            map = new GameObject[row, col, high];
            matrixLevel = new int[10, 10, 8];
            Debug.Log("init");
            dis = 1;
            for (int k = 0; k < high; k++)
            {
                for (int i = 0; i < row - k; i++)
                {
                    for (int j = 0; j < col - k; j++)
                    {
                        map[i, j, k] = ObjectPooler.Instance.Spawn("Tile");
                        map[i, j, k].transform.parent = board.transform.GetChild(k);
                        
                        map[i, j, k].transform.localScale = Vector3.one * dis;
                        map[i, j, k].name = "Tile " + i + " " + j + " " + k;
                        map[i, j, k].GetComponent<Tile>().id = 1;
                        map[i, j, k].GetComponent<Tile>().posInBoard = new Pos(i, j, k);
                        map[i, j, k].GetComponent<SpriteRenderer>().sortingOrder = sOrder += 2;
                        map[i, j, k].transform.localPosition = new Vector3(
                            -((col * 1f - 1) / 2 - j) * dis + dis / 2 * k,
                            ((row * 1f - 1) / 2 - i) * dis - dis / 2 * k,
                            -1 * k);
                        // if (col % 2 == 0)
                        // {
                        //     map[i, j, k].transform.localPosition -= Vector3.right * dis / 2;
                        // }
                        
                    }
                }
            }
            
        }
    }

    private void OnCombineComplete()
    {
        numOfTileComplete++;
    }

    [Button("Call Start")]
    public void CallStart()
    {
        ObjectPooler.Instance.ReleaseAll();
        var dis = GetDistanceEachTile();
        map = new GameObject[row, col, high];
        matrixLevel = new int[10, 8, 8];
        int sOrder = 10;
        dis = 1;
        for (int k = 0; k < high; k++)
        {
            for (int i = 0; i < row - k; i++)
            {
                for (int j = 0; j < col - k; j++)
                {
                    map[i, j, k] = ObjectPooler.Instance.Spawn("Tile");
                    map[i, j, k].transform.parent = board.transform.GetChild(k);
                    map[i, j, k].transform.localScale = Vector3.one * dis;
                    map[i, j, k].name = "Tile " + i + " " + j + " " + k;
                    map[i, j, k].GetComponent<Tile>().id = 1;
                    map[i, j, k].GetComponent<Tile>().posInBoard = new Pos(i, j, k);
                    map[i, j, k].GetComponent<SpriteRenderer>().sortingOrder = sOrder += 2;
                    map[i, j, k].transform.localPosition = new Vector3(
                        -((col * 1f - 1) / 2 - j) * dis + dis / 2 * k,
                        ((row * 1f - 1) / 2 - i) * dis - dis / 2 * k,
                        -1 * k);
                    // if (col % 2 == 0)
                    // {
                    //     map[i, j, k].transform.localPosition -= Vector3.right * dis / 2;
                    // }
                        
                }
            }
        }
        board.GetComponent<Board>().ArrangeFloor();
    }
    
    
    [Button("Get Size")]
    public async void GetSizeOfBoard()
    {
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        Vector3[] corners = new Vector3[4];
        boardZone.GetWorldCorners(corners);
        var height = (int)Math.Round((corners[0] - corners[1]).magnitude);
        var width = (int)Math.Round((corners[0] - corners[3]).magnitude);
        boardSize = new Vector2(width, height);
    }

    [Button("Get distance")]
    public float GetDistanceEachTile()
    {
        var dis = boardSize.x / col;
        dis = dis > 1.2f ? 1.2f : (float)Math.Round(dis, 1);
        return dis;
    }

    #endregion
    
    public void SaveCurrentState()
    {
        try
        {
            Common.Log("Save");
            var bf = new BinaryFormatter();
            var file = File.Create(Application.persistentDataPath +  "/CurrentState" + ".bytes");
            var queueData = board.GetComponent<Board>().q;
            var data = new CurrentLevelMatrix(GetCurrentMatrixId(queueData), board.GetComponent<Board>().queue.GetCurrentPosTileInQueue(), row, col, high);
            PlayerPrefs.SetInt("TileCombined", numOfTileComplete);
            bf.Serialize(file, data);
            Debug.Log("Save " + DateTime.Now);
            file.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    
    private int[,,] GetCurrentMatrixId(GameObject[] queueData)
    {
        var matrixId = new int[row, col, high];
        for (int k = 0; k < high; k++)
        {
            for (int i = 0; i < row - k; i++)
            {
                for (int j = 0; j < col - k; j++)
                {
                    if (map[i, j, k] != null)
                    {
                        matrixId[i, j, k] = tileMap[i, j, k].id;
                    }
                }
            }
        }

        foreach (var tile in queueData)
        {
            if (tile != null)
            {
                var pos = tile.GetComponent<Tile>().posInBoard;
                matrixId[pos.x, pos.y, pos.z] = tile.GetComponent<Tile>().id;
            }
        }
        
        return matrixId;
    }
    
    public bool TryLoadCurrentState(out Pos[] idInQueue)
    {
        idInQueue = new Pos[14];
        try
        {
            var bf = new BinaryFormatter();
            var file = File.Open(Application.persistentDataPath + "/CurrentState.bytes", FileMode.Open);
            var data = (CurrentLevelMatrix)bf.Deserialize(file);
            matrixLevel = data.boardMatrix;
            for (int k = 0; k < matrixLevel.GetLength(2); k++)
            {
                string floor = "";
                for (int i = 0; i < matrixLevel.GetLength(0); i++)
                {
                    for (int j = 0; j < matrixLevel.GetLength(1); j++)
                    {
                        floor += matrixLevel[i, j, k] + " ";
                    }

                    floor += "\n";
                }
//                Debug.Log(floor);
            }
            row = data.row;
            col = data.col;
            high = data.high;
            idInQueue = data.queue;
            foreach (var pos in idInQueue)
            {
                if (pos == null) break;
                Debug.Log(pos.x + " " + pos.y + " " + pos.z);
            }
            
            file.Close();
            DeleteSaveFile();
            return true;
        }
        catch (FileNotFoundException)
        {
            Common.LogWarning(this, "Can't resume game");
            return false;
        }
        catch (Exception e)
        {
            Common.LogWarning(this, e.Message);
            return false;
        }
    }

    public bool CheckFileExist()
    {
        return File.Exists(Application.persistentDataPath + "/CurrentState.bytes");
    }

    public void DeleteSaveFile()
    {
        try
        {
            if (File.Exists(Application.persistentDataPath + "/CurrentState.bytes"))
            {
                File.Delete(Application.persistentDataPath + "/CurrentState.bytes");
            }
        }
        catch (Exception e)
        {
            Common.Log(e.Message);
        }
    }

    public void Load(int level)
    {
        //string path = Application.persistentDataPath + "/level" + level + ".dat";
        TextAsset levelFile = Resources.Load<TextAsset>("LevelMatrix/level" + level);
        LevelMatrix data;
        using (var stream = new MemoryStream(levelFile.bytes))
        {
            var formatter = new BinaryFormatter ();
            data = (LevelMatrix)formatter.Deserialize (stream);
        }
        row = data.row;
        col = data.col;
        high = data.high;
        matrixLevel = data.matrix;
        Common.Log(row + " " + col + " " + high);
    }

    #region Gameplay function
    
    public void PlayGame(int level)
    {
        board.transform.parent.position = boardZone.transform.position;
        Debug.Log("Play Game " + level);
        int sOrder = 10;
        
        Pos[] idInQueue;
        bool isResume = TryLoadCurrentState(out idInQueue);
        Debug.Log(isResume);
        if (isResume)
        {
    
        }
        else
        {
            numOfTileComplete = 0;
            Load(level);
        }
        
        map = new GameObject[row, col, high];
        tileMap = new Tile[row, col, high];

        var dis = GetDistanceEachTile();
        Tile.ORIGINAL_SCALE = Vector3.one * dis;
        
        Debug.Log(isResume);
        for (int k = 0; k < high; k++)
        {
            for (int i = 0; i < row - k; i++)
            {
                for (int j = 0; j < col - k; j++)
                {
                    if (matrixLevel[i, j, k] > 0)
                    {
                        var pos = new Vector3(
                            -((col * 1f - 1) / 2 - j) * dis + dis / 2 * k,
                            ((row * 1f - 1) / 2 - i) * dis - dis / 2 * k,
                            -1 * k);
                        map[i, j, k] = ObjectPooler.Instance.Spawn("Tile");
                        tileMap[i, j, k] = map[i, j, k].GetComponent<Tile>();
                        tileMap[i, j, k].Init(new Pos(i, j, k), pos, sOrder,  matrixLevel[i, j, k], board);
                        sOrder += 2;
                    }
                }
            }
        }
        SetStatusCollider(true);
        
        totalTile = GetTotalTile();
        if (!isResume)
        {
            Debug.Log("init");
            Debug.Log(levelsData[level]);
            InitTileValue(levelsData[level]);
            CheckStatusEachTile();
        }
        else
        {
            numOfTileComplete = PlayerPrefs.GetInt("TileCombined");;
            totalTile += numOfTileComplete;
            Progress.Instance.SetFillAmount(numOfTileComplete);
            PlayerPrefs.DeleteKey("TileCombined");
            WaitToArrangeFloor(0.6f, idInQueue);
        }
    }

    private async void WaitToArrangeFloor(float seconds, Pos[] queues)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(seconds));
        if (queues != null)
        {
            foreach (var pos in queues)
            {
                if (pos != null)
                {
                    tileMap[pos.x, pos.y, pos.z].DirectionClick();
                }
            }
        }

        CheckStatusEachTile();
    }
    
    private void InitTileValue(int difficulty)
    {
        SetTileToList(listOfTile);
        int numOfSet = totalTile / 3;
        int numOfSetEachType = numOfSet / difficulty;
        for (int i = 1; i <= difficulty; i++)
        {
            for (int j = 0; j < numOfSetEachType; j++)
            {
                GenerateOneTileSet(i);
            }
        }
        
        for (int i = 0; i < numOfSet % difficulty; i++)
        {
            GenerateOneTileSet(Random.Range(1, difficulty + 1));
        }
    }
    
    private void GenerateOneTileSet(int i)
    {
        GetTileFromList(listOfTile).GetComponent<Tile>().id = i;
        GetTileFromList(listOfTile).GetComponent<Tile>().id = i;
        GetTileFromList(listOfTile).GetComponent<Tile>().id = i;
    }
    
    private void SetTileToList(List<GameObject> list)
    {
        list.Clear();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                for (int k = 0; k < high; k++)
                {
                    if (map[i, j, k] != null)
                    {
                        list.Add(map[i, j, k]);
                    }
                }
            }
        }
    }
    
    private GameObject GetTileFromList(List<GameObject> list)
    {
        var index = Random.Range(0, list.Count);
        var tile = list[index];
        list.RemoveAt(index);
        return tile;
    }

    public void Restart()
    {
        DeleteSaveFile();
        PlayGame(LevelManager.Instance.currentLevel);
        for (int k = 0; k < high; k++)
        {
            for (int i = 0; i < row - k; i++)
            {
                for (int j = 0; j < col - k; j++)
                {
                    if (map[i, j, k] != null)
                    {
                        map[i, j, k].transform.localScale = Vector3.zero;
                        map[i, j, k].transform.DOScale(Tile.ORIGINAL_SCALE, 0.5f).SetEase(Ease.OutQuad);
                    }
                }
            }
        }
    }
    #endregion
    
    
    private void OnClickToTile(GameObject tile)
    {
        if (!QueueZone.full)
        {
            var pos = tile.GetComponent<Tile>().posInBoard;
            map[pos.x, pos.y, pos.z] = null;
            tileMap[pos.x, pos.y, pos.z] = null;
        }

        RecheckStatusTile();
    }
    
    
    #region Status Tile Check Func
    public void CheckStatusEachTile()
    {
        if (PlayerPrefs.GetInt("Tutorial", 0) != 1) return;
        for (int k = 0; k < high; k++)
        {
            for (int i = 0; i < row - k; i++)
            {
                for (int j = 0; j < col - k; j++)
                {
                    if (tileMap[i, j, k] == null) continue;
                    tileMap[i, j, k].isLocking = GetNumberTileIsColliding(i, j, k) != 0;
                }
            }
        }
    }

    [Button("Get num tile")]
    private int GetNumberTileIsColliding(int x, int y, int z)
    {
        int newX, newY, newZ, res = 0;
        for (int i = 0; i < 4; i++)
        {
            newX = x + dirX[i];
            newY = y + dirY[i];
            newZ = z + 1;
            if (newX >= 0 && newX < row && newY >= 0 && newY < col && newZ < high)
            {
                if (map[newX, newY, newZ] != null && map[newX, newY, newZ].activeSelf)
                {
                    res++;
                }
            }
        }

        return res;
    }
    
    public async void RecheckStatusTile()
    {
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        CheckStatusEachTile();
    }
    #endregion
    
    
    #region For Gen Map
    private void DeleteTile(string name)
    {
        string[] s = name.Split(' ');
        int i = int.Parse(s[1]);
        int j = int.Parse(s[2]);
        int k = int.Parse(s[3]);
        map[i, j, k] = null;
    }
    
    private void ReturnTile(GameObject tile)
    {
        var pos = tile.GetComponent<Tile>().posInBoard;
        var i = pos.x;
        var j = pos.y;
        var k = pos.z;
        map[i, j, k] = tile;
        Debug.Log("Return Tile " + i + " " + j + " " + k + " " + tile.name);
        matrixLevel[i, j, k] = 1;
    }
    
    [Button("Check Tile In Map")]
    public void GetTileInMap(int i, int j, int k)
    {
        Debug.Log(map[i, j, k].name);
    }

    [Button("Get Total Tile")]
    public int GetTotalTile()
    {
        int total = 0;
        int tRow = testing ? 10 : row;
        int tCol = testing ? 8 : col;
        int tHigh = testing ? 8 : high;
        try
        {
            for (int k = 0; k < tHigh; k++)
            {
                for (int i = 0; i < tRow; i++)
                {
                    for (int j = 0; j < tCol; j++)
                    {
                        //Debug.Log(i + " " + j + " " + k);
                        if (i < row && j < col && k < high && map[i, j, k] != null)
                        {
                            if (matrixLevel[i, j, k] == 0)
                            {
                                matrixLevel[i, j, k] = 1;
                            }

                            total++;
                        }
                        else
                        {
                            matrixLevel[i, j, k] = 0;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return total;
    }
    

    private void Update()
    {
        if (testing && !converting)
        {
            totalTile = GetTotalTile();
            valid = totalTile % 3 == 0;
            LevelManager.Instance.currentLevel = currentLevel;
            //CheckStatusEachTile();
        }
    }

    [Button("Save")]
    public void SaveMatrixToFile()
    {
        if (File.Exists("Assets/TmpLevel/level" + currentLevel + ".bytes"))
        {
            Common.LogWarning(this, "File already exist");
            return;
        }
        if (!valid)
        {
            Common.LogWarning(this, "Invalid matrix");
            return;
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create("Assets/TmpLevel/level" + currentLevel + ".bytes");
        LevelMatrix data = new LevelMatrix(matrixLevel, row, col, high);
        bf.Serialize(file, data);
        file.Close();
    } 
    #endregion
    

    public void GameOverProcess()
    {
        for (int k = 0; k < high; k++)
        {
            for (int i = 0; i < row - k; i++)
            {
                for (int j = 0; j < col - k; j++)
                {
                    if (map[i, j, k] != null && map[i, j, k].activeSelf)
                    {
                        map[i, j, k].GetComponent<Tile>().GameOverProcess();
                    }
                }
            }
        }
    }

    public void SetStatusCollider(bool status)
    {
        for (int k = 0; k < high; k++)
        {
            for (int i = 0; i < row - k; i++)
            {
                for (int j = 0; j < col - k; j++)
                {
                    if (tileMap[i, j, k] != null)
                    {
                        tileMap[i, j, k].col.enabled = status;
                    }
                }
            }
        }
    }

    public List<Tile> PrepareForTutorial()
    {
        var tiles = new List<Tile>();
        for (int k = high - 1; k >= 0; k--) 
        {
            for (int i = 0; i < row - k; i++)
            {
                for (int j = 0; j < col - k; j++)
                {
                    if (tileMap[i, j, k] != null)
                    {
                        if (tiles.Count == 0)
                        {
                            tiles.Add(tileMap[i, j, k]);
                        }
                        else
                        {
                            if (tiles.Count < 3 && tileMap[i, j, k].id == tiles[0].id)
                            {
                                tiles.Add(tileMap[i, j, k]);
                            }
                            else
                            {
                                tileMap[i, j, k].isLocking = true;
                            }
                        }
                    }
                }
            }
        }

        return tiles;
    }

    public void SetStatusConvert(int[,,] matrix, int row, int col, int high, int total)
    {
        this.row = row;
        this.col = col;
        this.high = high;
        matrixLevel = matrix;
        totalTile = total;
        SaveMatrixToFile();
    }
}

[Serializable]
public class LevelMatrix
{
    public int[,,] matrix;
    public int row, col, high;

    public LevelMatrix(int[,,] matrix, int row, int col, int high)
    {
        this.matrix = matrix;
        this.row = row;
        this.col = col;
        this.high = high;
    }
}

[Serializable]
public class CurrentLevelMatrix
{
    public int[,,] boardMatrix;
    public Pos[] queue;
    public int row, col, high;

    public CurrentLevelMatrix(int[,,] boardMatrix, Pos[] queue, int row, int col, int high)
    {
        this.boardMatrix = boardMatrix;
        this.queue = queue;
        this.row = row;
        this.col = col;
        this.high = high;
    }
}
