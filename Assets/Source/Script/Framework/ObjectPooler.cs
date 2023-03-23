using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class PoolObject
{
    public GameObject gameObject;
    public int count; // restrict number of this game object
    public bool expandable;
}

public class ObjectPooler : Singleton<ObjectPooler>
{
    public List<PoolObject> poolObjects;

    public List<GameObject> pooledGameObjects;

    public override void Awake()
    {
        base.Awake();
        pooledGameObjects = new List<GameObject>();

        foreach (var item in poolObjects)
            for (int i = 0; i < item.count; i++) 
                pooledGameObjects.Add(CreateGobject(item.gameObject));
    }

    public GameObject Spawn(string tagOfObject, string nameObject = "")
    {
        foreach (var t in pooledGameObjects)
        {
            if (nameObject != "")
            {
                if (t.gameObject.name != nameObject)
                {
                    continue;
                }
            }
            if (!t.activeSelf && t.CompareTag(tagOfObject))
            {
                t.SetActive(true);
                return t;
            }
        }

        foreach (var item in poolObjects)
        {
            if (nameObject != "")
            {
                if (item.gameObject.name != nameObject)
                {
                    continue;
                }
            }
            if (item.gameObject.CompareTag(tagOfObject))
                if (item.expandable)
                {
                    GameObject obj = CreateGobject(item.gameObject);
                    pooledGameObjects.Add(obj);
                    obj.SetActive(true);
                    return obj;
                }
        }

        return null;
    }
    
    private GameObject CreateGobject(GameObject item)
    {
        GameObject gObject = Instantiate(item, transform);
        gObject.SetActive(false);
        return gObject;
    }

    public void ReleaseObject(GameObject item)
    {
        //Debug.Log("Release " + item.name);
        item.transform.SetParent(transform);
        item.SetActive(false);
    }
    
    
    public void ReleaseAll()
    {
        foreach (var item in pooledGameObjects)
        {
            ReleaseObject(item);
        }
    }
    
}

