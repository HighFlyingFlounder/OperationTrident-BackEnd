using System;
using System.Threading;

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
			string ip = System.Configuration.ConfigurationManager.AppSettings["Listen_ip"];
			int port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["Listen_port"]);
			servNet.Start(ip,port);
            
			while(true)
			{
				string str = Console.ReadLine();
				//Thread.Sleep(10000);
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
