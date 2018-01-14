using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //简单的服务器
            //Socket listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            //IPEndPoint ipEp = new IPEndPoint(ipAdr, 1234);
            //listenfd.Bind(ipEp);
            //listenfd.Listen(0);
            //Console.WriteLine("服务器启动成功");
            //while (true)
            //{
            //    Socket connfd = listenfd.Accept();
            //    Console.WriteLine("服务器 Accpet");
            //    byte[] readBuff = new byte[1024];
            //    int count = connfd.Receive(readBuff);
            //    string str = System.Text.ASCIIEncoding.UTF8.GetString(readBuff, 0, count);
            //    Console.WriteLine("服务器接收" + str);
            //    byte[] bytes = System.Text.ASCIIEncoding.Default.GetBytes("ser echo" + str);
            //    connfd.Send(bytes);
            //}
           
            Serv serv = new Serv();
            serv.Start("127.0.0.1", 1234);

            DataMgr dataMgr = new DataMgr();
            bool ret = dataMgr.Register("LPY", "123");
            if (ret)
                Console.WriteLine("注册成功");
            else
                Console.WriteLine("注册失败");
            ret = dataMgr.CreatePlayer("Lpy");
            if (ret)
                Console.WriteLine("创建玩家成功");
            else
                Console.WriteLine("创建玩家失败");
            PlayerData pd = dataMgr.GetPlayerData("Lpy");
            if (pd != null)
                Console.WriteLine("获取玩家成功 分数是 " + pd.score);
            else
                Console.WriteLine("获取玩家失败 ");
            pd.score += 10;
            Player p = new Player();
            p.id = "Lpy";
            p.data = pd;
            dataMgr.SavePlayer(p);
            pd = dataMgr.GetPlayerData("Lpy");
            if (pd != null)
                Console.WriteLine("获取玩家成功 分数 是  " + pd.score);
            else
                Console.WriteLine("重新获取玩家数据失败");

            while (true)
            {
                string str = Console.ReadLine();
                switch (str)
                {
                    case "quit":
                        return;
                }
            }
        }
    }
}
