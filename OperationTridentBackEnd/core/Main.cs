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
			servNet.Start("0.0.0.0",8000);

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
