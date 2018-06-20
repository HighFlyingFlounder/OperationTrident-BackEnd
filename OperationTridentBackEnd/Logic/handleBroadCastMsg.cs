using System;

public partial class HandlePlayerMsg
{
	public void MsgBroadCast(Player player, ProtocolBase protoBase)
	{
		ProtocolBytes proto = (ProtocolBytes)protoBase;
		//string protoName
		int start = 0;
		string protoName = proto.GetString(start, ref start);
		string sync_content = proto.GetString(start, ref start);

		Room room = player.tempData.room;
		ProtocolBytes protoRet = new ProtocolBytes();
		protoRet.AddString(sync_content);
		protoRet.AddString(player.id);
		protoRet.AppendBytes(proto.bytes, start, proto.bytes.Length - start);
		room.Broadcast(protoRet);
	}

	public void MsgRPC(Player player, ProtocolBase protoBase){
		ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        string rpcId = proto.GetString(start, ref start);
        Room room = player.tempData.room;
        ProtocolBytes protoRet = new ProtocolBytes();
		protoRet.AddString(rpcId);
        protoRet.AddString(player.id);
        protoRet.AppendBytes(proto.bytes, start, proto.bytes.Length - start);
        room.Broadcast(protoRet);
	}
}