using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    private GameData gameData;
    public static DataPersistenceManager instance {get; private set;}
    private List<IDataPersistence> dataPersistenceObjects;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found more than one Data Persistence Manager in scene");
        }
    }

    public void Start()
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }
    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // todo - Load saved data from a file using data handler
        // if no data can be loaded, initialise new game

        if (this.gameData == null)
        {
            Debug.Log("No data found. Initialising data to default");
            NewGame();
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log("Loaded Death Count = " + gameData.deathCount);
        // todo - push loaded data to all other scripts that need it
    }

    public void SaveGame()
    {
        // todo - pass data to other scripts so they can update it
        // save that data to a file using data handler
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }
        Debug.Log("Saved Death Count = " + gameData.deathCount);
    }

    public void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() 
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
