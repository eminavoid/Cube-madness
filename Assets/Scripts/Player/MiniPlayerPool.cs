using UnityEngine;
using System.Collections.Generic;

public class MiniPlayerPool
{
    private GameObject miniPlayerPrefab;
    private int poolSize;
    private List<MiniPlayer> availableObjects;
    private CustomUpdateManager updateManager;
    private Transform miniPlayersFolder;

    public MiniPlayerPool(GameObject prefab, int size, CustomUpdateManager manager)
    {
        miniPlayerPrefab = prefab;
        poolSize = size;
        availableObjects = new List<MiniPlayer>(poolSize);
        updateManager = manager;
        CreateMiniPlayersFolder();
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }

    private void CreateMiniPlayersFolder()
    {
        GameObject folder = GameObject.Find("MiniPlayersFolder");
        if (folder == null)
        {
            folder = new GameObject("MiniPlayersFolder");
        }
        miniPlayersFolder = folder.transform;
    }

    private void CreateNewObject()
    {
        GameObject newObject = GameObject.Instantiate(miniPlayerPrefab, miniPlayersFolder);
        MiniPlayer miniPlayer = newObject.GetComponent<MiniPlayer>();
        if (miniPlayer == null)
        {
            Debug.LogError("MiniPlayer prefab does not have a MiniPlayer script attached.");
        }
        miniPlayer.Initialize(this);
        // REMOVE THIS LINE: newObject.SetActive(false);
        availableObjects.Add(miniPlayer);
        if (updateManager != null)
        {
            updateManager.Register(miniPlayer);
        }
    }

    public MiniPlayer GetObject()
    {
        if (availableObjects.Count == 0)
        {
            // Optionally increase the pool size here if needed
            CreateNewObject();
        }
        MiniPlayer instance = availableObjects[availableObjects.Count - 1];
        availableObjects.RemoveAt(availableObjects.Count - 1);
        return instance;
    }

    public void ReturnObject(MiniPlayer miniPlayer)
    {
        if (updateManager != null)
        {
            updateManager.Unregister(miniPlayer);
        }
        availableObjects.Add(miniPlayer);
    }
}