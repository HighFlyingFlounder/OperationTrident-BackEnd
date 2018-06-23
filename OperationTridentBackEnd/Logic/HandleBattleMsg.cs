using System;
using System.Collections.Generic;

public partial class HandlePlayerMsg
{
    //开始战斗,可用, room.StartGame()开始战斗
    //发送协议: StartGame, int 0/1
    public void MsgStartGame(Player player, ProtocolBase protoBase)
	{
		ProtocolBytes protocol = new ProtocolBytes ();
		protocol.AddString ("StartGame");
		//条件判断
		if (player.tempData.status != PlayerTempData.Status.Room) 
		{
			Console.WriteLine ("MsgStartGame status err " + player.id);
			protocol.AddInt (-1);
			player.Send (protocol);
			return;
		}
		
		if (!player.tempData.isOwner) 
		{
			Console.WriteLine ("MsgStartGame owner err " + player.id);
			protocol.AddInt (-1);
			player.Send (protocol);
			return;
		}
		
		Room room = player.tempData.room;
		if(!room.CanStart())
		{
			Console.WriteLine ("MsgStartGame CanStart err " + player.id);
			protocol.AddInt (-1);
			player.Send (protocol);
			return;
		}
		
		//开始战斗
		protocol.AddInt (0);
		player.Send (protocol);
		room.EnterGame ();
	}

    public void MsgFinishLoading(Player player, ProtocolBase protoBase)
    {
        Room room = player.tempData.room;
		player.tempData.status = PlayerTempData.Status.Loading;
		ProtocolBytes protoRet = new ProtocolBytes();
		protoRet.AddString("FinishLoading");
		protoRet.AddString(player.id);
		room.Broadcast(protoRet);      
	}

	public void MsgStartFight(Player player, ProtocolBase protoBase){
		Room room = player.tempData.room;
		room.getReadyToFight += 1;
        if (room.getReadyToFight == room.list.Count)
            room.StartFight();
	}
    //同步单元信息, 
    //协议参数: float posX, posY, posZ, rotX, rotY, rotZ, gunRot, gunRoll
    //广播协议: UpdateUnitInfo, string id, float posX, posY, posZ, 
    // rotX, rotY, rotZ, gunRot, gunRoll
    public void MsgUpdateUnitInfo(Player player, ProtocolBase protoBase)
	{
		//获取数值
		int start = 0;
		ProtocolBytes protocol = (ProtocolBytes)protoBase;
		string protoName = protocol.GetString (start, ref start);
		float posX = protocol.GetFloat (start, ref start);
		float posY = protocol.GetFloat (start, ref start);
		float posZ = protocol.GetFloat (start, ref start);
		float rotX = protocol.GetFloat (start, ref start);
		float rotY = protocol.GetFloat (start, ref start);
		float rotZ = protocol.GetFloat (start, ref start);
		int isRun = protocol.GetInt(start, ref start);
		int isPush = protocol.GetInt(start, ref start);
		//获取房间
		if (player.tempData.status != PlayerTempData.Status.Fight)
			return;
		Room room = player.tempData.room;
		//作弊校验 略
		player.tempData.posX = posX;
		player.tempData.posY = posY;
		player.tempData.posZ = posZ;
		player.tempData.lastUpdateTime = Sys.GetTimeStamp ();
		//广播
		ProtocolBytes protocolRet = new ProtocolBytes();
		protocolRet.AddString ("UpdateUnitInfo");
		protocolRet.AddString (player.id);
		protocolRet.AddFloat (posX);
		protocolRet.AddFloat (posY);
		protocolRet.AddFloat (posZ);
		protocolRet.AddFloat (rotX);
		protocolRet.AddFloat (rotY);
		protocolRet.AddFloat (rotZ);
		protocolRet.AddInt (isRun);
		protocolRet.AddInt (isPush);
		room.Broadcast (protocolRet);
	}

    //开枪射击同步
	//协议参数: float posX, posY, posZ, rotX, rotY, rotZ
	//广播协议: string id, float posX, posY, posZ, rotX, rotY, rotZ
	public void MsgShooting(Player player, ProtocolBase protoBase)
	{
		//获取数值
		int start = 0;
		ProtocolBytes protocol = (ProtocolBytes)protoBase;
		string protoName = protocol.GetString (start, ref start);
		float posX = protocol.GetFloat (start, ref start);
		float posY = protocol.GetFloat (start, ref start);
		float posZ = protocol.GetFloat (start, ref start);
		float rotX = protocol.GetFloat (start, ref start);
		float rotY = protocol.GetFloat (start, ref start);
		float rotZ = protocol.GetFloat (start, ref start);
		//获取房间
		if (player.tempData.status != PlayerTempData.Status.Fight)
			return;
		Room room = player.tempData.room;
		//广播
		ProtocolBytes protocolRet = new ProtocolBytes();
		protocolRet.AddString ("Shooting");
		protocolRet.AddString (player.id);
		protocolRet.AddFloat (posX);
		protocolRet.AddFloat (posY);
		protocolRet.AddFloat (posZ);
		protocolRet.AddFloat (rotX);
		protocolRet.AddFloat (rotY);
		protocolRet.AddFloat (rotZ);
		room.Broadcast (protocolRet);
	}

	//伤害处理, 同时也做胜负判断
	//协议参数: string enemyName, float damage
	//f协议: string player.id, string enemy.id, float damage
	public void MsgHit(Player player, ProtocolBase protoBase)
	{
		//解析协议
		int start = 0;
		ProtocolBytes protocol = (ProtocolBytes)protoBase;
		// string protoName = protocol.GetString (start, ref start);
		// string enemyName = protocol.GetString (start, ref start);
		float damage = protocol.GetFloat (start, ref start);
		//作弊校验
        /*
		long lastShootTime = player.tempData.lastShootTime;
		if (Sys.GetTimeStamp () - lastShootTime < 1) 
		{
			Console.WriteLine ("MsgHit开炮作弊 " + player.id);
			return;
		}
		player.tempData.lastShootTime = Sys.GetTimeStamp();
		*/
		//更多作弊校验 略
		//获取房间
		if (player.tempData.status != PlayerTempData.Status.Fight)
			return;
		Room room = player.tempData.room;
		//扣除生命值
        /*
		if (!room.list.ContainsKey (enemyName))
		{
			Console.WriteLine ("MsgHit not Contains enemy " + enemyName);
			return;
		}
		Player enemy = room.list[enemyName];
		if (enemy == null)
			return;
		if (enemy.tempData.hp <= 0)
			return;
		enemy.tempData.hp -= damage;
        */
        //Console.WriteLine("MsgHit " + enemyName + "  hp:" + enemy.tempData.hp + " damage:" + damage);
		Console.WriteLine("MsgHit " + " damage:" + damage);
		//广播
		ProtocolBytes protocolRet = new ProtocolBytes();
		protocolRet.AddString ("Hit");
		protocolRet.AddString (player.id);
		//protocolRet.AddString (enemy.id);
		protocolRet.AddFloat (damage);
		room.Broadcast (protocolRet);
		//胜负判断
		//room.UpdateWin ();   //下一节实现
	}

	public void MsgSpaceArriveEnd(Player player, ProtocolBase protoBase){
		ProtocolBytes proto = (ProtocolBytes)protoBase;
		int start = 0;
		string name = proto.GetString(start, ref start);
		int num = proto.GetInt(start, ref start);
		Console.WriteLine("MsgSpaceArriveEnd: num = " + num);

		ProtocolBytes protocolRet = new ProtocolBytes();
		protocolRet.AddString("SpaceArriveEnd");
		protocolRet.AddString(player.id);
		protocolRet.AddInt(num);
		Room room = player.tempData.room;
		room.Broadcast(protocolRet);
		room.isArrived += 1;
		room.UpdateWin();
	}









}