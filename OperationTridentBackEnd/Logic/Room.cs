using System;
using System.Collections.Generic;
using System.Linq; 

//房间
public class Room
{
	public int roomId;
	//状态
	public enum Status
	{
		Prepare = 1,
		Loading = 2,
		Fight = 3 ,
	}
	public Status status = Status.Prepare;
	//玩家
	public int maxPlayers = 6;
	public Dictionary<string,Player> list = new Dictionary<string,Player>();
	public int isArrived = 0;
    public int getReadyToFight = 0;

    //添加玩家
    public bool AddPlayer(Player player)
	{
		lock (list) 
		{
			if (list.Count >= maxPlayers)
				return false;
			PlayerTempData tempData = player.tempData;
			tempData.room = this; 
			tempData.team = SwichTeam ();
			tempData.status = PlayerTempData.Status.Room;

			if (list.Count == 0)
				tempData.isOwner = true;
			else
				tempData.isOwner = false;
			string id = player.id;
			list.Add(id, player);
		}
		return true;
	}
	
	//分配队伍
	public int SwichTeam()
	{
		int count1 = 0;
		int count2 = 0;
		foreach(Player player in list.Values)
		{
			if(player.tempData.team == 1) count1++;
			if(player.tempData.team == 2) count2++;
		}
		if (count1 <= count2)
			return 1;
		else
			return 2;
	}

	//删除玩家
	public void DelPlayer(string id)
	{
		Logger.Default.Trace("DelPlayer " + id);
		lock (list) 
		{
			if (!list.ContainsKey(id))
				return;
			bool isOwner = list[id].tempData.isOwner;
			list[id].tempData.status = PlayerTempData.Status.None;
			list.Remove(id);
			if(isOwner)
				UpdateOwner();
		}
	}

	//更换房主
	public void UpdateOwner()
	{
		lock (list) 
		{
			if(list.Count <= 0)
				return;
			
			foreach(Player player in list.Values)
			{
				player.tempData.isOwner = false;
			}
			
			Player p = list.Values.First();
			p.tempData.isOwner = true;
		}
	}

	//广播
	public void Broadcast(ProtocolBase protocol)
	{
		foreach(Player player in list.Values)
		{
			player.Send(protocol);
		}
	}

	//房间信息
	public ProtocolBytes GetRoomInfo()
	{
		ProtocolBytes protocol = new ProtocolBytes ();
		protocol.AddString ("GetRoomInfo");
		//房间信息
		protocol.AddInt (list.Count);
		//每个玩家信息
		foreach(Player p in list.Values)
		{
			protocol.AddString(p.id);
			//protocol.AddInt(p.tempData.team);
			protocol.AddInt(p.data.win);
			protocol.AddInt(p.data.fail);
			int isOwner = p.tempData.isOwner? 1: 0;
			protocol.AddInt(isOwner);
        }
		return protocol;
	}

	//房间能否开战
	public bool CanStart()
	{
		if (status != Status.Prepare)
			return false;

		int cnt = list.Count;
		foreach (Player player in list.Values){
			if (player.tempData.status != PlayerTempData.Status.Room)
				return false;
		}
		/*
		int count1 = 0;
		int count2 = 0;
		
		foreach(Player player in list.Values)
		{
			if(player.tempData.team == 1) count1++;
			if(player.tempData.team == 2) count2++;
		}

		if (count1 < 1 || count2 < 1)
			return false;
        */
		if (cnt < 1) return false;
		return true;
	}


	public void EnterGame()
	{
		ProtocolBytes protocol = new ProtocolBytes ();
		protocol.AddString ("EnterGame");
		status = Status.Loading;
		lock (list) 
		{
			protocol.AddInt(list.Count);
			foreach(Player p in list.Values)
			{
				protocol.AddString(p.id);
				p.tempData.status = PlayerTempData.Status.Loading;
			}
			Broadcast(protocol);
		}
	}

    public void StartFight()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("StartFight");
		status = Status.Fight;
        isArrived = 0;
        int swopID = 1;
        lock (list)
        {
            protocol.AddInt(list.Count);
            foreach (Player p in list.Values)
            {
                p.tempData.hp = 200;
                protocol.AddString(p.id);
                protocol.AddInt(swopID++);
                p.tempData.status = PlayerTempData.Status.Fight;
            }
            Broadcast(protocol);
        }
    }

	//胜负判断, 失败返回0, 胜利返回1, 未出现胜负返回2
    private int IsWin()
	{
		/*
		if (status != Status.Fight)
			return 0;
		
		int count1 = 0;
		int count2 = 0;
		foreach(Player player in list.Values)
		{
			PlayerTempData pt = player.tempData;
			if(pt.team == 1 && pt.hp > 0) count1++;
			if(pt.team == 2 && pt.hp > 0) count2++;
		}
		
		if(count1 <= 0) return 2;
		if(count2 <= 0) return 1;
		return 0;
		*/
		if (isArrived == list.Count) return 1;
		else if (isArrived == -1) return 0;
		else return 2;
	}


	public void UpdateWin()
	{
		int isWin = IsWin();
		if (isWin == 2)
			return;
		//改变状态 数值处理
		lock (list) 
		{
			status = Status.Prepare;
			foreach (Player player in list.Values) 
			{
				player.tempData.status = PlayerTempData.Status.Room;
				if (player.tempData.team == isWin)
					player.data.win++;
				else
					player.data.fail++;
			}
		}
        
		//广播
		ProtocolBytes protocol = new ProtocolBytes();
		protocol.AddString ("Result");
		protocol.AddInt (isWin);
		Logger.Default.Info("Broadcast Result : isWin = " + isWin);
		Broadcast (protocol);
		isArrived = 0;
		getReadyToFight = 0;
	}


	//中途退出战斗
	public void ExitFight(Player player)
	{
		Logger.Default.Trace("Player " + player.id + " ExitFight");
		if (list [player.id] != null)
			list [player.id].tempData.hp = -1;	
		//增加失败次数
		if (IsWin () == 0)
			player.data.fail++;
		//胜负判断
		//UpdateWin();
	}
}