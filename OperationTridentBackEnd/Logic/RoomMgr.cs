using System;
using System.Collections.Generic;
using System.Linq; 

public class RoomMgr
{
	//单例
	public static RoomMgr instance;
	public RoomMgr()
	{
		instance = this;
	}
	
	//房间列表
	public List<Room> list = new List<Room>();

	//创建房间
	public void CreateRoom(Player player)
	{   
		Room room = new Room ();
		lock (list) 
		{
			list.Add(room);
            //房间ID等于list中的index
            room.setRoomID(list.IndexOf(room));
			room.AddPlayer(player);
		}
	}

	//玩家离开,可用
	public void LeaveRoom(Player player)
	{
		PlayerTempData tempDate = player.tempData;
		if (tempDate.status == PlayerTempData.Status.None)
			return;
		
		Room room = tempDate.room;
		
		lock(list)
		{
			room.DelPlayer(player.id);
			if(room.list.Count == 0)
				list.Remove(room);
		}
	}

	//列表
	//返回协议: int count, [room.list.count, room.status]
	public ProtocolBytes GetRoomList()
	{
		ProtocolBytes protocol = new ProtocolBytes ();
		protocol.AddString ("GetRoomList");
		int count = list.Count;
		//房间数量
		protocol.AddInt (count);
		//每个房间信息
		for (int i=0; i<count; i++) 
		{
			Room room = list[i];
			protocol.AddInt(room.list.Count);
			protocol.AddInt((int)room.status);
		}
		return protocol;
	}

}