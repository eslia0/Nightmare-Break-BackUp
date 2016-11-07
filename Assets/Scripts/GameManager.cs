using UnityEngine;

public class GameManager : MonoBehaviour
{
    DungeonManager dungeonManager;
    NetworkManager networkManager;
    InputManager inputManager;

    [SerializeField]
    string ip;
    [SerializeField]
    string port;

    void Start()
    {
        InitializeManager();
    }

    void Update()
    {
        networkManager.DataHandler.DataHandle();
        networkManager.DataSender.DataSend();
    }

    void InitializeManager()
    {
        networkManager = (Instantiate(Resources.Load("Manager/NetworkManager")) as GameObject).GetComponent<NetworkManager>();
        networkManager.name = "NetworkManager";
        networkManager.tag = "NetworkManager";
        networkManager.InitializeManager(ip);

        dungeonManager = (Instantiate(Resources.Load("Manager/DungeonManager")) as GameObject).GetComponent<DungeonManager>();
        dungeonManager.name = "DungeonManager";
        dungeonManager.InitializeManager();

        inputManager = (Instantiate(Resources.Load("Manager/InputManager")) as GameObject).GetComponent<InputManager>();
        inputManager.name = "InputManager";
        inputManager.tag = "InputManager";
    }

    public void OnApplicationQuit()
    {
        Debug.Log("Game Exit");
        networkManager.DataSender.GameClose();
    }
}

