using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{

    [Header("File Storage Config")]
    [SerializeField] private string fileName;



    private FileDataHandler dataHandler;
    public static DataPersistenceManager instance { get; private set; }

    private GameData gameData;

    private string selectedProfileID = "";

    private List<IDataPersistence> dataPersistencesObjects;
    public void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("More Than one data Persistence manager in this scene");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.selectedProfileID = dataHandler.GetMostRecentlyUpdateedProfileID();
    }

    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }


    public void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.dataPersistencesObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }


    public void OnSceneUnloaded(Scene scene)
    {
        SaveGame();
    }

    public void ChangeSelectedProfileId(string newProfileId)
    {
        this.selectedProfileID = newProfileId;
        LoadGame();
    }
    public void NewGame()
    {
        this.gameData = new GameData();
    }
    public void SaveGame()
    {

        if(this.gameData == null)
        {
            Debug.LogWarning("No Data was found. A new game will need to be started");
        }
        // To Do pass the fata to other scripts so they can update it
        foreach (IDataPersistence dataPersistence in dataPersistencesObjects)
        {
            dataPersistence.SaveData(ref gameData);
        }

        //gameData.lastUpdate = System.DateTime.Now.ToBinary();
        // save the data to a file using the data handler
        dataHandler.Save(gameData, selectedProfileID);
    }
    public void LoadGame()
    {
        //  Load any saved data from a file handeler
        this.gameData = dataHandler.Load(selectedProfileID);
        if (this.gameData == null)
        {
            Debug.Log("No data was found. A new game will need to be started");
            return;
        }

        foreach (IDataPersistence dataPersistence in dataPersistencesObjects)
        {
            dataPersistence.LoadData(gameData);
        }
    }
    // Game will save on application quit
    //public void OnApplicationQuit()
    //{
    //    SaveGame();
    //}

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistencesObjects = 
            FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistencesObjects);
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.loadAllProfiles();
    }

    public bool HasGameData()
    {
        return gameData != null;
    }
}
