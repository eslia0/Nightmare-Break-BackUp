using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class DataSender : MonoBehaviour
{
    CharacterManager characterManager;

    Socket tcpSock;
    Socket udpSock;

    Queue<DataPacket> sendMsgs;
    object sendLock;

    public void Initialize(Queue<DataPacket> newSendMsgs, object newSendLock, Socket newTcpSock, Socket newUdpSock)
    {
        //characterManager = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterManager>();

        sendMsgs = newSendMsgs;
        sendLock = newSendLock;
        tcpSock = newTcpSock;
        udpSock = newUdpSock;
    }

    //데이타를 전송하는 메소드. byte[] msg 를 newIPEndPoint로 전송한다.
    public void DataSend()
    {
        if (sendMsgs.Count > 0)
        {
            DataPacket packet;

            lock (sendLock)
            {
                packet = sendMsgs.Dequeue();
            }

            byte[] msg = packet.msg;
            EndPoint endPoint = packet.endPoint;

            Debug.Log("메시지 보냄 : " + endPoint);
            Debug.Log("메시지 보냄 (출처) : " + msg[2]);
            Debug.Log("메시지 보냄 (타입) : " + msg[3]);
            Debug.Log("메시지 보냄 (길이) : " + msg.Length);

            if (endPoint != null)
            {
                udpSock.BeginSendTo(msg, 0, msg.Length, SocketFlags.None, endPoint, new AsyncCallback(SendData), null);
            }
            else
            {
                tcpSock.Send(msg, 0, msg.Length, SocketFlags.None);
            }

        }
    }

    //비동기 콜백 메소드
    private void SendData(IAsyncResult ar)
    {
        udpSock.EndSend(ar);
    }

    //연결 확인
    public void ConnectionCheck(List<EndPoint> newEndPoint)
    {
        Debug.Log("연결 체크");

        foreach (EndPoint client in newEndPoint)
        {
            Debug.Log(client.ToString());
            byte[] msg = CreateResultPacket(new byte[1], P2PPacketId.ConnectionCheck);

            DataPacket packet = new DataPacket(msg, client);
            sendMsgs.Enqueue(packet);
        }
    }

    //캐릭터의 생성을 보내주는 메소드
    public void CreateUnitSend(short newId, Vector3 position)
    {
        short Id = newId;
        float xPos = position.x;
        float yPos = position.y;
        float zPos = position.z;

        CreateUnitData createUnitData = new CreateUnitData(Id, xPos, yPos, zPos);
        CreateUnitDataPacket createUnitDataPacket = new CreateUnitDataPacket(createUnitData);

        byte[] msg = CreatePacket(createUnitDataPacket, P2PPacketId.CreateUnit);

        DataPacket packet = new DataPacket(msg, NetworkManager.client1);
        sendMsgs.Enqueue(packet);
    }

    //캐릭터의 애니메이션, 방향, 위치를 보내주는 메소드
    public IEnumerator CharacterDataSend()
    {
        yield return new WaitForSeconds(1.0f);

        while (true)
        {
            yield return new WaitForEndOfFrame();

            byte state = (byte)characterManager.State;
            float vertical = characterManager.Animator.GetFloat("Ver");
            float horizontal = characterManager.Animator.GetFloat("Hor");
            bool direction = characterManager.Animator.GetBool("Direction");
            float xPos = characterManager.transform.position.x;
            float yPos = characterManager.transform.position.y;
            float zPos = characterManager.transform.position.z;

            CharacterStateData characterStateData = new CharacterStateData(state, direction, horizontal, vertical, xPos, yPos, zPos);
            CharacterStateDataPacket characterStateDataPacket = new CharacterStateDataPacket(characterStateData);

            byte[] msg = CreatePacket(characterStateDataPacket, P2PPacketId.CharacterState);

            //현재는 client로 고정되있지만
            //차후 수정으로 매개변수 newIPEndPoint를 설정하여 여러명의 클라이언트에 동시에 보내도록 수정할 예정
            DataPacket packet = new DataPacket(msg, NetworkManager.client1);
            sendMsgs.Enqueue(packet);
        }
    }

    //패킷의 헤더 부분을 생성하는 메소드
    byte[] CreateHeader<T>(IPacket<T> data, P2PPacketId Id)
    {
        byte[] msg = data.GetPacketData();

        HeaderData headerData = new HeaderData();
        HeaderSerializer headerSerializer = new HeaderSerializer();

        headerData.Id = (byte)Id;
        headerData.source = (byte)DataHandler.Source.ClientSource;
        headerData.length = (short)msg.Length;

        headerSerializer.Serialize(headerData);
        byte[] header = headerSerializer.GetSerializedData();

        return header;
    }

    public static byte[] CreateResultPacket(byte[] msg, P2PPacketId Id)
    {
        HeaderData headerData = new HeaderData();
        HeaderSerializer HeaderSerializer = new HeaderSerializer();

        headerData.Id = (byte)Id;
        headerData.source = (byte)DataHandler.Source.ClientSource;
        headerData.length = (short)msg.Length;

        HeaderSerializer.Serialize(headerData);
        msg = DataHandler.CombineByte(HeaderSerializer.GetSerializedData(), msg);
        return msg;
    }

    //패킷을 생성하는 메소드. 데이터 패킷과 패킷아이디를 적어주면 알아서 합쳐준다.
    //Send를 하기 전 반드시 해야한다.
    byte[] CreatePacket<T>(IPacket<T> data, P2PPacketId Id)
    {
        byte[] msg = data.GetPacketData();
        byte[] header = CreateHeader(data, Id);
        byte[] packet = DataHandler.CombineByte(header, msg);

        return packet;
    }
}
