using UnityEngine;
using System.Collections.Generic;

public class MiniPlayerPool
{
    private GameObject miniPlayerPrefab;
    private int poolSize;
    private List<MiniPlayer> availableObjects;

    public MiniPlayerPool(GameObject prefab, int size)
    {
        miniPlayerPrefab = prefab;
        poolSize = size;
        availableObjects = new List<MiniPlayer>(poolSize);

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }

    private MiniPlayer CreateNewObject()
    {
        GameObject newObject = GameObject.Instantiate(miniPlayerPrefab);
        MiniPlayer miniPlayer = newObject.GetComponent<MiniPlayer>();
        if (miniPlayer == null)
        {
            Debug.LogError("MiniPlayer prefab does not have a MiniPlayer script attached.");
        }
        newObject.SetActive(false);
        availableObjects.Add(miniPlayer);
        return miniPlayer;
    }

    public MiniPlayer GetObject()
    {
        if (availableObjects.Count == 0)
        {
            CreateNewObject();
        }
        MiniPlayer instance = availableObjects[availableObjects.Count - 1];
        availableObjects.RemoveAt(availableObjects.Count - 1);
        return instance;
    }

    public void ReturnObject(MiniPlayer miniPlayer)
    {
        availableObjects.Add(miniPlayer);
    }
}