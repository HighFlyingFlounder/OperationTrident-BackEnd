using System;
using System.Collections.Generic;

public class PlayerTempData
{
	public PlayerTempData()
	{
		status = Status.None;
	}
	//状态
	public enum Status
	{
		None,
		Room,
        Loading,
		Fight,
	}
	public Status status;
	//room状态
	public Room room;
	public int team = 1;
	public bool isOwner = false;
	//战场相关
	public long lastUpdateTime;
	public float posX;
	public float posY;
	public float posZ;
    //为什么没有rotX
	public long lastShootTime;
	public float hp = 200;
}