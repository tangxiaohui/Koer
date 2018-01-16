using System;

namespace Serv
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			RoomMgr roomMgr = new RoomMgr ();
			DataMgr dataMgr = new DataMgr ();
			ServNet servNet = new ServNet();
			servNet.proto = new ProtocolBytes ();
			servNet.Start("127.0.0.1",1234);

            Cards card = Cards.Get(1);

            Console.WriteLine("AAAAAAA:" + card.Power);

			while(true)
			{
				string str = Console.ReadLine();
				switch(str)
				{
				case "quit":
					servNet.Close();
					return;
				case "print":
					servNet.Print();
					break;
				}
			}

		}
	}
}
