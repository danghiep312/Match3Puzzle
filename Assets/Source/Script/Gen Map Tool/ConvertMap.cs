using System;
using System.Collections.Generic;

using Sirenix.OdinInspector;
using UnityEngine;


public class ConvertMap : MonoBehaviour
{
    public int[] difficulty = new int[200];
    
    [Button("Convert map")]
    public void Convert(int level)
    {
        /*
        int totalTile = 0;
        TextAsset levelFile = Resources.Load<TextAsset>("LevelTest/Level " + level);
        //MapData mapData1 = JsonConvert.DeserializeObject<MapData>(levelFile.text);
        MapData mapData2 = null;
        try
        {
            levelFile = Resources.Load<TextAsset>("LevelTest/Level #" + level);
            mapData2 = JsonConvert.DeserializeObject<MapData>(levelFile.text);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        var mapData = mapData1;
        if (mapData2 != null)
        {
            if (mapData2.uniqueTiles < mapData1.uniqueTiles || !CheckValid(mapData1.layers))
                mapData = mapData2;
        }

        

        difficulty[level] = mapData.uniqueTiles;
        string tmp = "";
        int high = mapData.layers.Count;
        int row = mapData.layers[0].Count;
        int col = mapData.layers[0][0].Count;

        

        int[,,] map = new int[10, 10, 8];
        for (int k = 0; k < high; k++)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    try
                    {
                        map[i, j, k] = mapData.layers[k][i][j];
                        if (map[i, j, k] != 0)
                            totalTile++;
                    }
                    catch (Exception e)
                    {
                        // ignored
                    }
                }
            }
        }
        foreach (var floor in mapData.layers)
        {
            foreach (var r in floor)
            {
                foreach (var tile in r)
                {
                    tmp += tile + " ";
                }
                tmp += "\n";
            }
            tmp += "\n";
        }
        
        
        if (!CheckValid(mapData.layers))
        {
            Debug.Log(tmp);
            Common.LogWarning(this, "INVALID MAP: " + level);
            return;
        }

        Debug.Log(level + " " + totalTile);
        MapGenerator.Instance.valid = totalTile % 3 == 0;
        MapGenerator.Instance.totalTile = totalTile;
        MapGenerator.Instance.currentLevel = level;
        MapGenerator.Instance.SetStatusConvert(map, row, col, high, totalTile);
        

        */
        
    }

    private bool CheckValid(List<List<List<int>>> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            if (list[i].Count < list[i + 1].Count)
            {
                return false;
            }
        }

        return true;
    }

    
    [Button("Convert range")]
    public void ConvertRange(int start, int end)
    {
        for (int i = start; i <= end; i++)
        {
            Convert(i);
        }
    }
    
    [Button("Get Difficulty")]
    public void GetDifficulty()
    {
        difficulty = MapGenerator.Instance.levelsData;

        string json = JsonUtility.ToJson(new Difficulty(difficulty));
        Debug.Log(json);
    }
}

[System.Serializable]
public class MapData
{
    public List<List<List<int>>> layers;
    public String nextStage;
    public int limitedTime;
    public bool isAllUnknowTiles;
    public int mysteryTilesMaxCount;
    public int mysteryTilesMinCount;
    public int jokersTriplesMaxCountKey;
    public int jokersTriplesMinCountKey;
    public int uniqueTiles;
}

[Serializable]
public class Difficulty
{
    public int[] difficulty;
    
    public Difficulty(int[] diff) => difficulty = diff;
}