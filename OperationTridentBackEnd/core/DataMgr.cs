using System;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DataMgr
{
    MySqlConnection sqlConn;

    //单例模式
    public static DataMgr instance;
    public DataMgr()
    {
        instance = this;
        Connect();
    }

    //连接
    public void Connect()
    {
        //数据库
		string database = System.Configuration.ConfigurationManager.AppSettings["Mysql_database"];
		string source = System.Configuration.ConfigurationManager.AppSettings["Mysql_source"];
		string userid = System.Configuration.ConfigurationManager.AppSettings["Mysql_userId"];
		string password = System.Configuration.ConfigurationManager.AppSettings["Mysql_password"];
		int port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["Mysql_port"]);
		string connStr = String.Format("Database={0};Data Source={1};", database, source);
		connStr += String.Format("User Id={0};Password={1};port={2}",  userid, password, port);
        sqlConn = new MySqlConnection(connStr);
        try
        {
            sqlConn.Open();
        }
        catch (Exception e)
        {
            Logger.Default.Error("[DataMgr]Connect " + e.Message);
            return;
        }
    }

    //判定安全字符串
    public bool IsSafeStr(string str)
    {
        return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
    }

    //是否存在该用户
    private bool CanRegister(string id)
    {
        //防sql注入
        if (!IsSafeStr(id))
            return false;
        //查询id是否存在
        string cmdStr = string.Format("select * from user where id='{0}';", id);
        MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
        try
        {
            MySqlDataReader dataReader = cmd.ExecuteReader();
            bool hasRows = dataReader.HasRows;
            dataReader.Close();
            return !hasRows;
        }
        catch (Exception e)
        {
            Logger.Default.Error("[DataMgr]CanRegister fail " + e.Message);
			Logger.Default.Error("[DataMgr]Reconnect Database");
            Connect();
            return false;
        }
    }

    //注册
    public bool Register(string id, string pw)
    {
        //防sql注入
        if (!IsSafeStr(id) || !IsSafeStr(pw))
        {
            Logger.Default.Info("[DataMgr]Register 使用非法字符");
            return false;
        }
        //能否注册
        if (!CanRegister(id))
        {
            Logger.Default.Info("[DataMgr]Register !CanRegister");
            return false;
        }
        //写入数据库User表
        string cmdStr = string.Format("insert into user set id ='{0}' ,pw ='{1}';", id, pw);
        MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
        try
        {
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Logger.Default.Error("[DataMgr]Register " + e.Message);
			Logger.Default.Error("[DataMgr]Reconnect Database");
            Connect();
            return false;
        }
    }

    //创建角色
    public bool CreatePlayer(string id)
    {
        //防sql注入
        if (!IsSafeStr(id))
            return false;
        //序列化
        IFormatter formatter = new BinaryFormatter();
        
		MemoryStream stream = new MemoryStream();
        PlayerData playerData = new PlayerData();
        try
        {
            formatter.Serialize(stream, playerData);
        }
        catch (Exception e)
        {
            Logger.Default.Info("[DataMgr]CreatePlayer 序列化 " + e.Message);
            return false;
        }
        byte[] byteArr = stream.ToArray();
        //写入数据库
        string cmdStr = string.Format("insert into player set id ='{0}' ,data =@data;", id);
        MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
        cmd.Parameters.Add("@data", MySqlDbType.Blob);
        cmd.Parameters[0].Value = byteArr;
        try
        {
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Logger.Default.Error("[DataMgr]CreatePlayer 写入 " + e.Message);
			Logger.Default.Error("[DataMgr]Reconnect Database");
            Connect();
            return false;
        }
    }

    //检测用户名密码
    public bool CheckPassWord(string id, string pw)
    {
        //防sql注入
        if (!IsSafeStr(id) || !IsSafeStr(pw))
            return false;
        //查询
        string cmdStr = string.Format("select * from user where id='{0}' and pw='{1}';", id, pw);
        MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
        try
        {
            MySqlDataReader dataReader = cmd.ExecuteReader();
            bool hasRows = dataReader.HasRows;
            dataReader.Close();
            return hasRows;
        }
        catch (Exception e)
        {
            Logger.Default.Error("[DataMgr]CheckPassWord " + e.Message);
			Logger.Default.Error("[DataMgr]Reconnect Database");
			Connect();
            return false;
        }
    }

    //获取玩家数据
    public PlayerData GetPlayerData(string id)
    {
        PlayerData playerData = null;
        //防sql注入
        if (!IsSafeStr(id))
            return playerData;
        //查询
        string cmdStr = string.Format("select * from player where id ='{0}';", id);
        MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
        byte[] buffer;
        try
        {
            MySqlDataReader dataReader = cmd.ExecuteReader();
            if (!dataReader.HasRows)
            {
                dataReader.Close();
                return playerData;
            }
            dataReader.Read();

            long len = dataReader.GetBytes(1, 0, null, 0, 0);//1是data  
            buffer = new byte[len];
            dataReader.GetBytes(1, 0, buffer, 0, (int)len);
            dataReader.Close();
        }
        catch (Exception e)
        {
            Logger.Default.Info("[DataMgr]GetPlayerData 查询 " + e.Message);
            return playerData;
        }
        //反序列化
        MemoryStream stream = new MemoryStream(buffer);
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            playerData = (PlayerData)formatter.Deserialize(stream);
            return playerData;
        }
        catch (SerializationException e)
        {
            Logger.Default.Info("[DataMgr]GetPlayerData 反序列化 " + e.Message);
            return playerData;
        }
    }


    //保存角色
    public bool SavePlayer(Player player)
    {
        string id = player.id;
        PlayerData playerData = player.data;
        //序列化
        IFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();
        try
        {
            formatter.Serialize(stream, playerData);
        }
        catch (Exception e)
        {
            Logger.Default.Info("[DataMgr]SavePlayer 序列化 " + e.Message);
            return false;
        }
        byte[] byteArr = stream.ToArray();
        //写入数据库
        string formatStr = "update player set data =@data where id = '{0}';";
        string cmdStr = string.Format(formatStr, player.id);
        MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
        cmd.Parameters.Add("@data", MySqlDbType.Blob);
        cmd.Parameters[0].Value = byteArr;
        try
        {
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Logger.Default.Error("[DataMgr]CreatePlayer 写入 " + e.Message);
			Logger.Default.Error("[DataMgr]Reconnect Database");
            Connect();
            return false;
        }
    }
}