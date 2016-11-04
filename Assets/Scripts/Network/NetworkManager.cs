using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{
    //패킷의 길이
    //패킷이 어디서 오는지
    //패킷의 종류
    public const int packetLength = 2;
    public const int packetSource = 1;
    public const int packetId = 1;

    //테스트 중에서는 하나의 컴퓨터에서 진행하므로 다른 ip 대신에 다른 port를 이용한다
    public const int mainServerPortNumber = 8800;
    public const int serverPortNumber = 9002;
    public const int clientPortNumber = 9003;
    public const int client1PortNumber = 9001;
    public static IPEndPoint mainServer = new IPEndPoint(IPAddress.Parse("192.168.94.88"), mainServerPortNumber);
    public static IPEndPoint server = new IPEndPoint(IPAddress.Parse("192.168.94.88"), serverPortNumber);
    public static IPEndPoint client = new IPEndPoint(IPAddress.Parse("192.168.94.88"), clientPortNumber);
    public static IPEndPoint client1 = new IPEndPoint(IPAddress.Parse("192.168.94.88"), client1PortNumber);

    //udp Socket이 연결할 SocketList
    List<EndPoint> clients;

    Queue<DataPacket> receiveMsgs;
    Queue<DataPacket> sendMsgs;
    object receiveLock;
    object sendLock;

    Socket clientSock;
    Socket serverSock;

    DataReceiver dataReceiver;
    DataHandler dataHandler;
    DataSender dataSender;

    public DataReceiver DataReceiver { get { return dataReceiver; } }
    public DataHandler DataHandler { get { return dataHandler; } }
    public DataSender DataSender { get { return dataSender; } }

    public void InitializeManager()
    {
        receiveMsgs = new Queue<DataPacket>();
        sendMsgs = new Queue<DataPacket>();
        receiveLock = new object();
        sendLock = new object();

        serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSock.Bind(server);

        clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        clientSock.Bind(client);

        dataReceiver = GetComponent<DataReceiver>();
        dataHandler = GetComponent<DataHandler>();
        dataSender = GetComponent<DataSender>();

        dataReceiver.Initialize(receiveMsgs, receiveLock, serverSock);
        dataHandler.Initialize(receiveMsgs, receiveLock, sendMsgs, sendLock);
        dataSender.Initialize(sendMsgs, sendLock, serverSock, clientSock);
    }

    public void ConnectServer()
    {
        try
        {
            serverSock.Connect(mainServer);
            Debug.Log("서버 연결 성공");
        }
        catch
        {
            Debug.Log("서버 연결 실패");
        }
    }

    public void ConnectP2P()
    {
        //아이피 목록 생성
        clients = new List<EndPoint>();
        clients.Add(client1);

        dataReceiver.StartUdpReceive(clientSock, clients);
        dataSender.ConnectionCheck(clients);
    }

    public void SocketClose()
    {
        serverSock.Close();
        clientSock.Close();
    }
}
