# OperationTrident-BackEnd
Back-End of the game OperationTrident

## Release v0.1
- 能与客户端v0.3相适应,进行网络同步
- 完成Conn, DataMgr, Logger, Protocol, ServNet, Player, Room基本组件
- 完成基本的客户端连接, 注册登录, 创建房间, 开始游戏, 测试延时基本功能
- 完成与Mysql数据库连接,保存基本玩家信息
- 使用NLog完成日志功能
- 完成消息的处理(反射), 与房间内广播(Broadcast)
- 完成房间内RPC功能, 支持传输所有Serializable类型以及部分Unity类型(Vector2, Vector3)

