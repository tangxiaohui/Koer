using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

public class Conn
{
    public const int BUFFER_SIZE = 1024;
    public Socket socket;
    public bool isUse = false;
    public byte[] readBuff = new byte[BUFFER_SIZE];
    public int buffCount = 0;
    public byte[] lenBytes = new byte[sizeof(UInt32)];
    public Int32 msgLength = 0;

    public Conn()
    {
        readBuff = new byte[BUFFER_SIZE];
    }

    public void Init(Socket socket)
    {
        this.socket = socket;
        isUse = true;
        buffCount = 0;
    }

    public int BuffRemain()
    {
        return BUFFER_SIZE - buffCount;
    }

    public string GetAdress()
    {
        if (!isUse)
            return "无法获取地址";
        return socket.RemoteEndPoint.ToString();
    }

    public void Close()
    {
        if (!isUse)
            return;
        Console.WriteLine("断开连接" + GetAdress());
        socket.Close();
        isUse = false;
    }
}
