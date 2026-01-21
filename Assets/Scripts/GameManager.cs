using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    [SerializeField]
    private GameData gameData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameData GetGameData
    {
        get {  return gameData; }
        set { gameData = value; }
    }

    public void SaveGame()
    {
        string data = JsonUtility.ToJson(gameData);
        PlayerPrefs.SetString("SavedData", data);
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("SavedData"))
        {
            string data = PlayerPrefs.GetString("SavedData");
            gameData = JsonUtility.FromJson<GameData>(data);
        }
        
    }
}
