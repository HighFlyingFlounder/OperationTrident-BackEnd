using System;

public class HandlePlayerEvent
{
	//上线,可用
	public void OnLogin(Player player)
	{

	}
	//下线
	public void OnLogout(Player player)
	{
		//房间中
		if (player.tempData.status == PlayerTempData.Status.Room) 
		{
			Logger.Default.Trace("Player logout with Status Room");
			Room room = player.tempData.room;
			RoomMgr.instance.LeaveRoom (player);
			if(room != null)
				room.Broadcast(room.GetRoomInfo());
			return;
		}
        //Loading
		if (player.tempData.status == PlayerTempData.Status.Loading)
        {
            Logger.Default.Trace("Player logout with Status Loading");
            Room room = player.tempData.room;
			RoomMgr.instance.LeaveRoom(player);
			if (room != null)
                room.Broadcast(room.GetRoomInfo());
            return;
        }
		//战斗中
		if (player.tempData.status == PlayerTempData.Status.Fight) 
		{
			Logger.Default.Trace("Player logout with Status Fight");
			Room room = player.tempData.room;
			room.ExitFight(player);
			RoomMgr.instance.LeaveRoom(player);
			return;
		}
		Logger.Default.Trace("Player logout with Status None");
	}
}