using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;

public class DataReceiver : MonoBehaviour
{
    Socket tcpSock;
    Socket udpSock;

    object receiveLock;

    Queue<DataPacket> msgs;

    //클래스 초기화
    public void Initialize(Queue<DataPacket> receiveMsgs, object newReceiveLock, Socket newSock)
    {
        msgs = receiveMsgs;
        receiveLock = newReceiveLock;
        tcpSock = newSock;
        StartTcpReceive();
    }

    //Tcp (서버) 수신 시작
    public void StartTcpReceive()
    {
        AsyncData asyncData = new AsyncData(tcpSock);

        //패킷 헤더 중 패킷의 길이 (2) 만큼 데이터를 받는다
        tcpSock.BeginReceive(asyncData.msg, 0, NetworkManager.packetLength, SocketFlags.None, new AsyncCallback(TcpReceiveLengthCallback), asyncData);
    }

    //Tcp 길이 수신
    public void TcpReceiveLengthCallback(IAsyncResult asyncResult)
    {
        AsyncData asyncData = (AsyncData)asyncResult.AsyncState;
        Socket tcpSock = asyncData.tcpSock;

        try
        {
            asyncData.msgSize = (short)tcpSock.EndReceive(asyncResult);
            Debug.Log("메시지 받음");
            Debug.Log(asyncData.EP);
        }
        catch (Exception e)
        {
            Debug.Log("연결 끊김 : " + e.Message);
            tcpSock.Close();
            return;
        }

        if (asyncData.msgSize >= NetworkManager.packetLength)
        {
            try
            {   //데이터 길이 변환에 성공하면 데이터를 받는다
                //남은 데이터는 데이터 출처 + 데이터 아이디 + 데이터
                short msgSize = BitConverter.ToInt16(asyncData.msg, 0);
                asyncData = new AsyncData(tcpSock);
                tcpSock.BeginReceive(asyncData.msg, 0, msgSize + NetworkManager.packetSource + NetworkManager.packetId, SocketFlags.None, new AsyncCallback(TcpReceiveDataCallback), asyncData);
            }
            catch
            {   //데이터 길이 변환 실패시 다시 데이터 길이를 받는다
                Console.WriteLine("DataReceiver::HandleAsyncLengthReceive.BitConverter 에러");
                asyncData = new AsyncData(tcpSock);
                tcpSock.BeginReceive(asyncData.msg, 0, NetworkManager.packetLength, SocketFlags.None, new AsyncCallback(TcpReceiveLengthCallback), asyncData);
            }
        }
        else
        {   //데이터 길이를 받지 못했을 시 다시 데이터 길이를 받는다
            asyncData = new AsyncData(tcpSock);
            tcpSock.BeginReceive(asyncData.msg, 0, NetworkManager.packetLength, SocketFlags.None, new AsyncCallback(TcpReceiveLengthCallback), asyncData);
        }
    }

    //Tcp 데이터 수신
    void TcpReceiveDataCallback(IAsyncResult asyncResult)
    {
        AsyncData asyncData = (AsyncData)asyncResult.AsyncState;
        Socket tcpSock = asyncData.tcpSock;

        try
        {
            asyncData.msgSize = (short)tcpSock.EndReceive(asyncResult);
        }
        catch
        {
            Debug.Log("NetworkManager::HandleAsyncDataReceive.EndReceive 에러");
            tcpSock.Close();
            return;
        }

        if (asyncData.msgSize >= NetworkManager.packetId)
        {
            Array.Resize(ref asyncData.msg, asyncData.msgSize + NetworkManager.packetId + NetworkManager.packetSource);
            Debug.Log(asyncData.msg.Length);
            DataPacket packet = new DataPacket(asyncData.msg);

            lock (receiveLock)
            {
                try
                {   //큐에 삽입
                    Debug.Log("Enqueue Message Length : " + packet.msg.Length);
                    msgs.Enqueue(packet);
                }
                catch
                {
                    Console.WriteLine("NetworkManager::HandleAsyncDataReceive.Enqueue 에러");
                }
            }
        }

        //재 수신
        asyncData = new AsyncData(tcpSock);
        tcpSock.BeginReceive(asyncData.msg, 0, NetworkManager.packetLength, SocketFlags.None, new AsyncCallback(TcpReceiveLengthCallback), asyncData);
    }

    //Udp (클라이언트) 수신 시작
    public void StartUdpReceive(Socket newSock, List<EndPoint> clients)
    {
        udpSock = newSock;

        //매개변수로 받은 리스트의 IPEndPoint에서 비동기 수신을 대기한다
        foreach (EndPoint newEndPoint in clients)
        {
            AsyncData asyncData = new AsyncData(newEndPoint);
            udpSock.BeginReceiveFrom(asyncData.msg, 0, NetworkManager.packetLength, SocketFlags.None, ref asyncData.EP, new AsyncCallback(UdpReceiveLengthCallback), asyncData);
            Debug.Log("수신시작 : " + newEndPoint);
        }
    }

    //Udp 길이 수신
    public void UdpReceiveLengthCallback(IAsyncResult asyncResult)
    {
        Debug.Log("Udp 길이 수신");
        AsyncData asyncData = (AsyncData)asyncResult.AsyncState;

        try
        {
            asyncData.msgSize = (short)udpSock.EndReceive(asyncResult);
            Debug.Log("메시지 길이 받음");
            Debug.Log(asyncData.EP);
        }
        catch
        {
            Console.WriteLine("DataReceiver::HandleAsyncReceiveLength.EndReceive 에러");
            return;
        }

        if (asyncData.msgSize >= NetworkManager.packetLength)
        {
            short msgSize = 0;

            try
            {   //데이터 길이 변환에 성공하면 데이터를 받는다.
                //남은 데이터는 데이터 출처 + 데이터 아이디 + 데이터
                msgSize = BitConverter.ToInt16(asyncData.msg, 0);
                asyncData = new AsyncData((IPEndPoint)asyncData.EP);
                udpSock.BeginReceive(asyncData.msg, 0, msgSize + NetworkManager.packetSource + NetworkManager.packetId, SocketFlags.None, UdpReceiveDataCallback, asyncData);
            }
            catch
            {   //데이터 길이 변환 실패시 다시 데이터 길이를 받는다.
                Console.WriteLine("DataReceiver::HandleAsyncReceiveLength.BitConverter 에러");
                asyncData = new AsyncData((IPEndPoint)asyncData.EP);
                udpSock.BeginReceiveFrom(asyncData.msg, 0, NetworkManager.packetLength, SocketFlags.None, ref asyncData.EP, new AsyncCallback(UdpReceiveLengthCallback), asyncData);
            }
        }
        else
        {   //데이터 길이를 받지 못했을 시 다시 데이터 길이를 받는다
            asyncData = new AsyncData((IPEndPoint)asyncData.EP);
            udpSock.BeginReceiveFrom(asyncData.msg, 0, NetworkManager.packetLength, SocketFlags.None, ref asyncData.EP, new AsyncCallback(UdpReceiveLengthCallback), asyncData);
        }
    }

    //Udp 데이터 수신
    public void UdpReceiveDataCallback(IAsyncResult asyncResult)
    {
        AsyncData asyncData = (AsyncData)asyncResult.AsyncState;

        try
        {
            asyncData.msgSize = (short)udpSock.EndReceive(asyncResult);
            Debug.Log("메시지 받음");
            Debug.Log(asyncData.EP);
        }
        catch (Exception e)
        {
            Debug.Log("연결 끊김 :" + e.Message);
            udpSock.Close();
        }

        if (asyncData.msgSize > 0)
        {
            //삭제 가능한지-
            //Array.Resize(ref msg, asyncData.msgSize + NetworkManager.packetId + NetworkManager.packetSource);
            DataPacket packet = new DataPacket(asyncData.msg, (IPEndPoint)asyncData.EP);

            lock (receiveLock)
            {   //큐에 삽입
                Debug.Log("Enqueue Message Length : " + asyncData.msg.Length);
                msgs.Enqueue(packet);
            }

            //다시 수신 준비
            asyncData = new AsyncData((IPEndPoint)asyncData.EP);
            udpSock.BeginReceiveFrom(asyncData.msg, 0, AsyncData.msgMaxSize, SocketFlags.None, ref asyncData.EP, new AsyncCallback(UdpReceiveLengthCallback), asyncData);
        }
    }

    //index 부터 length만큼을 잘라 반환하고 매개변수 배열을 남은 만큼 잘라서 반환한다
    public static byte[] ResizeByteArray(int index, int length, ref byte[] array)
    {
        //desArray = 자르고 싶은 배열
        //sourArray = 자르고 남은 배열 => 원래 배열
        //ref 연산자로 원래 배열을 변경하게 된다.

        byte[] desArray = new byte[length];
        byte[] sourArray = new byte[array.Length - length];

        Array.Copy(array, index, desArray, 0, length);
        Array.Copy(array, length, sourArray, 0, array.Length - length);
        array = sourArray;

        return desArray;
    }
}

//비동기 콜백을 위한 클래스
public class AsyncData
{
    public Socket tcpSock;
    public EndPoint EP;
    public byte[] msg;
    public short msgSize;
    public const int msgMaxSize = 1024;

    public AsyncData(EndPoint newEndPoint)
    {
        tcpSock = null;
        EP = newEndPoint;
        msgSize = 0;
        msg = new byte[msgMaxSize];
    }

    public AsyncData(Socket newSock)
    {
        tcpSock = newSock;
        EP = null;
        msgSize = 0;
        msg = new byte[msgMaxSize];
    }
}

public class DataPacket
{
    public byte[] msg;
    public EndPoint endPoint;

    public DataPacket(byte[] newMsg)
    {
        msg = newMsg;
        endPoint = null;
    }

    public DataPacket(byte[] newMsg, EndPoint newEndPoint)
    {
        msg = newMsg;
        endPoint = newEndPoint;
    }
}