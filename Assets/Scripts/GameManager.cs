using UnityEngine;

public class GameManager : MonoBehaviour
{
    DungeonManager dungeonManager;
    NetworkManager networkManager;
    InputManager inputManager;

    void Start()
    {
        InitializeManager();
    }

    void Update()
    {
        //inputManager.GetKeyInput();
        networkManager.DataHandler.DataHandle();
        networkManager.DataSender.DataSend();
    }

    void InitializeManager()
    {
        networkManager = (Instantiate(Resources.Load("Manager/NetworkManager")) as GameObject).GetComponent<NetworkManager>();
        networkManager.name = "NetworkManager";
        networkManager.tag = "NetworkManager";
        networkManager.InitializeManager();
        networkManager.ConnectServer();

        dungeonManager = (Instantiate(Resources.Load("Manager/DungeonManager")) as GameObject).GetComponent<DungeonManager>();
        dungeonManager.name = "DungeonManager";
        dungeonManager.InitializeManager();
        dungeonManager.CreatePlayer(0);

        inputManager = (Instantiate(Resources.Load("Manager/InputManager")) as GameObject).GetComponent<InputManager>();
        inputManager.name = "InputManager";
        inputManager.tag = "InputManager";
        inputManager.InitializeManager();
    }

    public void OnApplicationQuit()
    {
        Debug.Log("Game Exit");
        networkManager.SocketClose();
    }
}

