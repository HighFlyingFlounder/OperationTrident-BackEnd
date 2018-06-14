using System;

public partial class HandlePlayerMsg
{
	public void MsgRoomBroadCast(Player player, ProtocolBase protoBase){

		ProtocolBytes proto = (ProtocolBytes)protoBase;
		//string protoName
		int start = 0;
		string protoName = proto.GetString(start, ref start);
		string sync_content = proto.GetString(start, ref start);


		Room room = player.tempData.room;
		ProtocolBytes protoRet = new ProtocolBytes();
		protoRet.AddString("RoomBroadCast");
		protoRet.AddString(sync_content);

		//room.Broadcast()

	}

	//获取分数,可用
	//协议参数：
	//返回协议：int分数
	public void MsgGetScore(Player player, ProtocolBase protoBase)
	{
		ProtocolBytes protocolRet = new ProtocolBytes ();
		protocolRet.AddString ("GetScore");
		protocolRet.AddInt (player.data.score);
		player.Send (protocolRet);
		Console.WriteLine ("MsgGetScore " + player.id + player.data.score);
	}

	//增加分数,分数+1,可用
	//协议参数：
    //无返回协议
	public void MsgAddScore(Player player, ProtocolBase protoBase)
	{
		//获取数值
		int start = 0;
		ProtocolBytes protocol = (ProtocolBytes)protoBase; // 似乎没用
		string protoName = protocol.GetString (start, ref start); // 似乎没用
		//处理
		player.data.score += 1;
		Console.WriteLine ("MsgAddScore " + player.id + " " + player.data.score.ToString ());
	}

	//获取玩家列表
	public void MsgGetList(Player player, ProtocolBase protoBase)
	{
		Scene.instance.SendPlayerList (player);
	}
	
	//更新信息(需要修改, 场景广播, 全体广播)
	//协议参数: float x, float y, float z,
	//广播协议: UpdateInfo, string player.id, float x, float y, float z, int score
	public void MsgUpdateInfo(Player player, ProtocolBase protoBase)
	{
		//获取数值
		int start = 0;
		ProtocolBytes protocol = (ProtocolBytes)protoBase;
		string protoName = protocol.GetString (start, ref start);
		float x = protocol.GetFloat (start, ref start);
		float y = protocol.GetFloat (start, ref start);
		float z = protocol.GetFloat (start, ref start);
		int score = player.data.score;
		Scene.instance.UpdateInfo (player.id, x, y, z, score);
		//广播
		ProtocolBytes protocolRet = new ProtocolBytes();
		protocolRet.AddString ("UpdateInfo");
		protocolRet.AddString (player.id);
		protocolRet.AddFloat (x);
		protocolRet.AddFloat (y);
		protocolRet.AddFloat (z);
		protocolRet.AddInt (score);
		ServNet.instance.Broadcast (protocolRet);
	}

	//获取玩家信息(需要修改)
	//协议参数:
	//返回协议:GetAchieve, int win, int fail
	public void MsgGetAchieve(Player player, ProtocolBase protoBase)
	{
		ProtocolBytes protocolRet = new ProtocolBytes ();
		protocolRet.AddString ("GetAchieve");
		protocolRet.AddInt (player.data.win);
		protocolRet.AddInt (player.data.fail);
		player.Send (protocolRet);
		Console.WriteLine ("MsgGetAchieve " + player.id + player.data.win);
	}

	public void MsgHitRock(Player player, ProtocolBase protoBase)
    {
		int start = 0;
		ProtocolBytes protocol = (ProtocolBytes)protoBase;
        string protoName = protocol.GetString(start, ref start);
		string rock_name = protocol.GetString(start, ref start);
        ProtocolBytes protocolRet = new ProtocolBytes();
        protocolRet.AddString("HitRock");
		protocolRet.AddString(rock_name);
		Room room = player.tempData.room;
		room.Broadcast(protocolRet);
		//player.Send(protocolRet);
		Console.WriteLine("MsgHitRock: " + rock_name);
    }

	public void MsgDead(Player player, ProtocolBase protoBase)
    {
        
        ProtocolBytes protocolRet = new ProtocolBytes();
        protocolRet.AddString("Dead");
		protocolRet.AddString(player.id);
        Room room = player.tempData.room;
        room.Broadcast(protocolRet);
        //player.Send(protocolRet);
		Console.WriteLine("MsgDead: " + player.id);
		room.isArrived = -1;
		room.UpdateWin();
    }

}