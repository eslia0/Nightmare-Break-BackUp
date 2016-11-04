using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;

public class DataHandler : MonoBehaviour
{
    public enum Source
    {
        ServerSource = 0,
        ClientSource,
    }

    NetworkManager networkManager;

    public Queue<DataPacket> receiveMsgs;

    object receiveLock;

    byte[] msg = new byte[1024];
    EndPoint ipEndPoint;

    public delegate P2PPacketId P2PRecvNotifier(byte[] data);
    public delegate ServerPacketId ServerRecvNotifier(byte[] data);
    P2PRecvNotifier p2pRecvNotifier;
    ServerRecvNotifier serverRecvNotifier;
    private Dictionary<int, P2PRecvNotifier> p2p_notifier = new Dictionary<int, P2PRecvNotifier>();
    private Dictionary<int, ServerRecvNotifier> server_notifier = new Dictionary<int, ServerRecvNotifier>();

    DungeonManager dungeonManager;

    public void Initialize(Queue<DataPacket> receiveQueue, object newReceiveLock, Queue<DataPacket> sendQueue, object newSendLock)
    {
        receiveMsgs = receiveQueue;
        receiveLock = newReceiveLock;

        networkManager = GetComponent<NetworkManager>();

        SetServerNotifier();
        SetUdpNotifier();
    }

    public void SetServerNotifier()
    {
        server_notifier.Add((int)ServerPacketId.Match, Match);
    }

    public void SetUdpNotifier()
    {
        p2p_notifier.Add((int)P2PPacketId.ConnectionCheck, ConnectionAnswer);
        p2p_notifier.Add((int)P2PPacketId.CreateUnit, CreateUnit);
        p2p_notifier.Add((int)P2PPacketId.CharacterState, CharacterState);
    }

    public void DataHandle()
    {
        if (receiveMsgs.Count != 0)
        {
            //패킷을 Dequeue 한다 
            //패킷 : 메시지 타입 + 메시지 내용
            DataPacket packet = receiveMsgs.Dequeue();
            msg = packet.msg;
            ipEndPoint = packet.endPoint;

            Debug.Log("Dequeue Message Length : " + msg.Length);

            if (packet.headerData.source == (byte)Source.ServerSource)
            {
                if (server_notifier.TryGetValue(packet.headerData.id, out serverRecvNotifier))
                {
                    ServerPacketId packetId = serverRecvNotifier(msg);
                }
                else
                {
                    Debug.Log("DataHandler::Server.TryGetValue 에러 " + packet.headerData.id);
                    packet.headerData.id = (byte)ServerPacketId.None;
                }
            }
            else if (packet.headerData.source == (byte)Source.ClientSource)
            {
                if (p2p_notifier.TryGetValue(packet.headerData.id, out p2pRecvNotifier))
                {
                    P2PPacketId packetId = p2pRecvNotifier(msg);
                }
                else
                {
                    Debug.Log("DataHandler::P2P.TryGetValue 에러 " + packet.headerData.id);
                    packet.headerData.id = (byte)P2PPacketId.None;
                }
            }
        }
    }

    public ServerPacketId Match(byte[] data)
    {
        networkManager.ConnectP2P();

        return ServerPacketId.None;
    }

    public P2PPacketId ConnectionAnswer(byte[] data)
    {
        Debug.Log("연결 확인 답장");
        dungeonManager = GameObject.Find("DungeonManager").GetComponent<DungeonManager>();
        dungeonManager.CreatePlayer(0);

        return P2PPacketId.ConnectionAnswer;
    }

    public P2PPacketId CreateUnit(byte[] data)
    {
        CreateUnitDataPacket createUnitDataPacket = new CreateUnitDataPacket(data);
        CreateUnitData createUnitData = createUnitDataPacket.GetData();

        dungeonManager.CreateUnit(createUnitData.ID, new Vector3(createUnitData.PosX, createUnitData.PosY, createUnitData.PosZ));

        return P2PPacketId.None;
    }

    //원래는 보낸 IP를 체크해서 몇번째 플레이어인지 확인 후 그 플레이어의 캐릭터를 조정해야한다.
    //현재는 고정적으로 1번 플레이어를 설정
    public P2PPacketId CharacterState(byte[] data)
    {
        Debug.Log("캐릭터 상태 수신");

        CharacterStateDataPacket characterStateDataPacket = new CharacterStateDataPacket(data);
        CharacterStateData characterStateData = characterStateDataPacket.GetData();

        CharacterManager characterManager = dungeonManager.Players[1].GetComponent<CharacterManager>();
        characterManager.SetState(characterStateData);

        return P2PPacketId.None;
    }

    //public ServerPacketId CreateAccount(byte[] data)
    //{
    //    Debug.Log(tcpPacket.client.RemoteEndPoint.ToString() + " 가입요청");

    //    AccountPacket accountPacket = new AccountPacket(data);
    //    AccountData accountData = accountPacket.GetData();

    //    Debug.Log("아이디 : " + accountData.id + "패스워드 : " + accountData.password);

    //    try
    //    {
    //        if (database.AddAccountData(accountData.id, accountData.password))
    //        {
    //            msg[0] = (byte)UnityServer.Result.Success;
    //            Debug.Log("가입 성공");
    //        }
    //        else
    //        {
    //            msg[0] = (byte)UnityServer.Result.Fail;
    //            Debug.Log("가입 실패");
    //        }
    //    }
    //    catch
    //    {
    //        Debug.Log("DataHandler::AddPlayerData 에러");
    //        Debug.Log("가입 실패");
    //        msg[0] = (byte)UnityServer.Result.Fail;
    //    }

    //    Array.Resize(ref msg, 1);
    //    msg = CreateResultPacket(msg, ServerPacketId.CreateResult);

    //    return ServerPacketId.CreateResult;
    //}

    //public ServerPacketId DeleteAccount(byte[] data)
    //{
    //    Debug.Log(tcpPacket.client.RemoteEndPoint.ToString() + " 탈퇴요청");

    //    AccountPacket accountPacket = new AccountPacket(data);
    //    AccountData accountData = accountPacket.GetData();

    //    Debug.Log("아이디 : " + accountData.id + "패스워드 : " + accountData.id);

    //    try
    //    {
    //        if (database.DeleteAccountData(accountData.id, accountData.password))
    //        {
    //            msg[0] = (byte)UnityServer.Result.Success;
    //            Debug.Log("탈퇴 성공");
    //        }
    //        else
    //        {
    //            msg[0] = (byte)UnityServer.Result.Fail;
    //            Debug.Log("탈퇴 실패");
    //        }
    //    }
    //    catch
    //    {
    //        Debug.Log("DataHandler::RemovePlayerData 에러");
    //        Debug.Log("탈퇴 실패");
    //        msg[0] = (byte)UnityServer.Result.Fail;
    //    }

    //    Array.Resize(ref msg, 1);
    //    msg = CreateResultPacket(msg, ServerPacketId.DeleteResult);

    //    return ServerPacketId.DeleteResult;
    //}

    //public ServerPacketId Login(byte[] data)
    //{
    //    Debug.Log(tcpPacket.client.RemoteEndPoint.ToString() + " 로그인요청");

    //    AccountPacket accountPacket = new AccountPacket(data);
    //    AccountData accountData = accountPacket.GetData();

    //    Debug.Log("아이디 : " + accountData.id + "비밀번호 : " + accountData.password);

    //    try
    //    {
    //        if (database.AccountData.Contains(accountData.id))
    //        {
    //            if (((LoginData)database.AccountData[accountData.id]).PW == accountData.password)
    //            {
    //                if (!LoginUser.ContainsValue(accountData.id))
    //                {
    //                    msg[0] = (byte)UnityServer.Result.Success;
    //                    Debug.Log("로그인 성공");
    //                    LoginUser.Add(tcpPacket.client, accountData.id);
    //                }
    //                else
    //                {
    //                    Debug.Log("현재 접속중인 아이디입니다.");

    //                    if (CompareIP(GetSocket(accountData.id).RemoteEndPoint.ToString(), tcpPacket.client.RemoteEndPoint.ToString()))
    //                    {
    //                        LoginUser.Remove(GetSocket(accountData.id));
    //                        Debug.Log("현재 접속중 해제");
    //                    }
    //                    msg[0] = (byte)UnityServer.Result.Fail;
    //                }
    //            }
    //            else
    //            {
    //                Debug.Log("패스워드가 맞지 않습니다.");
    //                msg[0] = (byte)UnityServer.Result.Fail;
    //            }
    //        }
    //        else
    //        {
    //            Debug.Log("존재하지 않는 아이디입니다.");
    //            msg[0] = (byte)UnityServer.Result.Fail;
    //        }
    //    }
    //    catch
    //    {
    //        Debug.Log("DataHandler::PlayerData.Contains 에러");
    //        msg[0] = (byte)UnityServer.Result.Fail;
    //    }

    //    Array.Resize(ref msg, 1);

    //    msg = CreateResultPacket(msg, ServerPacketId.LoginResult);

    //    return ServerPacketId.LoginResult;
    //}

    //public ServerPacketId Logout(byte[] data)
    //{
    //    Debug.Log(tcpPacket.client.RemoteEndPoint.ToString() + " 로그아웃요청");

    //    string id = LoginUser[tcpPacket.client];

    //    msg = new byte[1];

    //    try
    //    {
    //        if (LoginUser.ContainsValue(id))
    //        {
    //            LoginUser.Remove(tcpPacket.client);
    //            Debug.Log(id + "로그아웃");
    //            msg[0] = (byte)UnityServer.Result.Success;
    //        }
    //        else
    //        {
    //            Debug.Log("로그인되어있지 않은 아이디입니다. : " + id);
    //            msg[0] = (byte)UnityServer.Result.Fail;
    //        }
    //    }
    //    catch
    //    {
    //        Debug.Log("DataHandler::PlayerData.Contains 에러");
    //        msg[0] = (byte)UnityServer.Result.Fail;
    //    }

    //    Array.Resize(ref msg, 1);

    //    msg = CreateResultPacket(msg, ServerPacketId.LoginResult);

    //    return ServerPacketId.None;
    //}

    //public ServerPacketId GameClose(byte[] data)
    //{
    //    Debug.Log("게임종료");

    //    try
    //    {
    //        Debug.Log(tcpPacket.client.RemoteEndPoint.ToString() + "가 접속을 종료했습니다.");

    //        if (LoginUser.ContainsKey(tcpPacket.client))
    //        {
    //            string id = LoginUser[tcpPacket.client];
    //            database.FileSave(id + ".data", database.GetAccountData(id));
    //            database.UserData.Remove(id);

    //            LoginUser.Remove(tcpPacket.client);
    //        }

    //        tcpPacket.client.Close();
    //    }
    //    catch
    //    {
    //        Debug.Log("DataHandler::LoginUser.Close 에러");
    //    }

    //    return ServerPacketId.None;
    //}

    //byte[] CreateHeader<T>(IPacket<T> data, ServerPacketId id)
    //{
    //    byte[] msg = data.GetPacketData();

    //    HeaderData headerData = new HeaderData();
    //    HeaderSerializer headerSerializer = new HeaderSerializer();

    //    headerData.id = (byte)id;
    //    headerData.length = (short)msg.Length;

    //    headerSerializer.Serialize(headerData);
    //    byte[] header = headerSerializer.GetSerializedData();

    //    return header;
    //}

    //byte[] CreatePacket<T>(IPacket<T> data, ServerPacketId id)
    //{
    //    byte[] msg = data.GetPacketData();
    //    byte[] header = CreateHeader(data, id);
    //    byte[] packet = CombineByte(header, msg);

    //    return packet;
    //}

    //byte[] CreateResultPacket(byte[] msg, ServerPacketId id)
    //{
    //    HeaderData headerData = new HeaderData();
    //    HeaderSerializer HeaderSerializer = new HeaderSerializer();

    //    headerData.id = (byte)id;
    //    headerData.length = (short)msg.Length;
    //    HeaderSerializer.Serialize(headerData);
    //    msg = CombineByte(HeaderSerializer.GetSerializedData(), msg);
    //    return msg;
    //}

    //bool CompareIP(string ip1, string ip2)
    //{
    //    if (ip1.Substring(0, ip1.IndexOf(":")) == ip2.Substring(0, ip2.IndexOf(":")))
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    //public Socket GetSocket(string id)
    //{
    //    foreach (KeyValuePair<Socket, string> client in LoginUser)
    //    {
    //        if (client.Value == id)
    //        {
    //            return client.Key;
    //        }
    //    }

    //    return null;
    //}

}

[Serializable]
public class TcpClient
{
    public Socket client;
    public string id;

    public TcpClient(Socket newClient)
    {
        client = newClient;
        id = "";
    }
}

public class HeaderData
{
    // 헤더 == [2바이트 - 패킷길이][1바이트 - ID]
    public short length; // 패킷의 길이
    public byte source;
    public byte id; // 패킷 ID
}