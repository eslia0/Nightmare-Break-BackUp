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
        //dungeonManager = GameObject.Find("DungeonManager").GetComponent<DungeonManager>();

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

            //출처 분리
            byte source = msg[0];

            //타입 분리
            byte[] Id = DataReceiver.ResizeByteArray(1, NetworkManager.packetId, ref msg);

            HeaderData headerData = new HeaderData();

            Debug.Log(source);
            Debug.Log(Id[0]);

            if (source == (byte)Source.ServerSource)
            {
                if (server_notifier.TryGetValue(Id[0], out serverRecvNotifier))
                {
                    ServerPacketId packetId = serverRecvNotifier(msg);
                }
                else
                {
                    Debug.Log("DataHandler::Server.TryGetValue 에러 " + Id);
                    headerData.Id = (byte)ServerPacketId.None;
                }
            }
            else if (source == (byte)Source.ClientSource)
            {
                if (p2p_notifier.TryGetValue(Id[0], out p2pRecvNotifier))
                {
                    P2PPacketId packetId = p2pRecvNotifier(msg);
                }
                else
                {
                    Debug.Log("DataHandler::P2P.TryGetValue 에러 " + Id);
                    headerData.Id = (byte)P2PPacketId.None;
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
        DataSender.CreateResultPacket(new byte[1], P2PPacketId.ConnectionAnswer);

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

    //    Debug.Log("아이디 : " + accountData.Id + "패스워드 : " + accountData.password);

    //    try
    //    {
    //        if (database.AddAccountData(accountData.Id, accountData.password))
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

    //    Debug.Log("아이디 : " + accountData.Id + "패스워드 : " + accountData.Id);

    //    try
    //    {
    //        if (database.DeleteAccountData(accountData.Id, accountData.password))
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

    //    Debug.Log("아이디 : " + accountData.Id + "비밀번호 : " + accountData.password);

    //    try
    //    {
    //        if (database.AccountData.Contains(accountData.Id))
    //        {
    //            if (((LoginData)database.AccountData[accountData.Id]).PW == accountData.password)
    //            {
    //                if (!LoginUser.ContainsValue(accountData.Id))
    //                {
    //                    msg[0] = (byte)UnityServer.Result.Success;
    //                    Debug.Log("로그인 성공");
    //                    LoginUser.Add(tcpPacket.client, accountData.Id);
    //                }
    //                else
    //                {
    //                    Debug.Log("현재 접속중인 아이디입니다.");

    //                    if (CompareIP(GetSocket(accountData.Id).RemoteEndPoint.ToString(), tcpPacket.client.RemoteEndPoint.ToString()))
    //                    {
    //                        LoginUser.Remove(GetSocket(accountData.Id));
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
    //            string Id = LoginUser[tcpPacket.client];
    //            database.FileSave(Id + ".data", database.GetAccountData(Id));
    //            database.UserData.Remove(Id);

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

    //byte[] CreateHeader<T>(IPacket<T> data, ServerPacketId Id)
    //{
    //    byte[] msg = data.GetPacketData();

    //    HeaderData headerData = new HeaderData();
    //    HeaderSerializer headerSerializer = new HeaderSerializer();

    //    headerData.Id = (byte)Id;
    //    headerData.length = (short)msg.Length;

    //    headerSerializer.Serialize(headerData);
    //    byte[] header = headerSerializer.GetSerializedData();

    //    return header;
    //}

    //byte[] CreatePacket<T>(IPacket<T> data, ServerPacketId Id)
    //{
    //    byte[] msg = data.GetPacketData();
    //    byte[] header = CreateHeader(data, Id);
    //    byte[] packet = CombineByte(header, msg);

    //    return packet;
    //}

    //byte[] CreateResultPacket(byte[] msg, ServerPacketId Id)
    //{
    //    HeaderData headerData = new HeaderData();
    //    HeaderSerializer HeaderSerializer = new HeaderSerializer();

    //    headerData.Id = (byte)Id;
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

    //public Socket GetSocket(string Id)
    //{
    //    foreach (KeyValuePair<Socket, string> client in LoginUser)
    //    {
    //        if (client.Value == Id)
    //        {
    //            return client.Key;
    //        }
    //    }

    //    return null;
    //}

    public static byte[] CombineByte(byte[] array1, byte[] array2)
    {
        byte[] array3 = new byte[array1.Length + array2.Length];
        Array.Copy(array1, 0, array3, 0, array1.Length);
        Array.Copy(array2, 0, array3, array1.Length, array2.Length);
        return array3;
    }

    public static byte[] CombineByte(byte[] array1, byte[] array2, byte[] array3)
    {
        byte[] array4 = CombineByte(CombineByte(array1, array2), array3); ;
        return array4;
    }
}

[Serializable]
public class TcpClient
{
    public Socket client;
    public string Id;

    public TcpClient(Socket newClient)
    {
        client = newClient;
        Id = "";
    }
}

public class HeaderData
{
    // 헤더 == [2바이트 - 패킷길이][1바이트 - ID]
    public short length; // 패킷의 길이
    public byte source;
    public byte Id; // 패킷 ID
}