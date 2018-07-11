# OperationTrident-BackEnd
Back-End of the game OperationTrident

## 安装

### 下载Release包，解压：
	https://github.com/HighFlyingFlounder/OperationTrident-BackEnd/releases
	unrar x OperationTrident-BackEnd.v0.1.rar   // sudo apt-get install unrar

### 安装MySQL:
	sudo apt-get install mysql-server

### 创建游戏数据库所需要的表
	mysql -u root -p     //然后输入安装数据库时所设置的初始密码
        >create database game;  //数据库名自己定， 与后面配置文件名字相同即可
	>use game;
	>source createGameTable.sql;  // .sql文件在项目目录中有

### 安装.Net环境
从官网安装最新稳定版Mono：http://www.mono-project.com/download/stable/ 
目前后台使用的5.1.2版本，安装方法：

	sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
	sudo apt install apt-transport-https
	echo "deb https://download.mono-project.com/repo/ubuntu stable-xenial main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list
	sudo apt update
	sudo apt install mono-devel

### 环境配置完成

## 启动
-修改启动配置
通过修改.config文件来设置端口，数据库名和连接数据库密码等:

	<appSettings>
        <add key="Listen_ip" value="0.0.0.0"/>
        <add key="Listen_port" value="8000"/>
        <add key="Mysql_database" value="game"/>
        <add key="Mysql_source" value="127.0.0.1"/>
        <add key="Mysql_userId" value="root"/>
        <add key="Mysql_password" value="123456"/>
        <add key="Mysql_port" value="3306"/>
	</appSettings>

- 关闭防火墙

	service iptables stop
- 启动后台
直接启动：

	mono OperationTridentBackEnd.exe
后台启动：

	nohup mono OperationTridentBackEnd.exe > out.log &

- 性能测试
参考：https://blog.csdn.net/weixin_42343424/article/details/80564771

	tc qdisc add dev eth0 root netem delay 100ms    //该命令将 eth0 网卡 的传输设置为延迟 100 毫秒发送。
	tc qdisc del dev eth0 root netem delay 100ms    //该命令将删除 eth0 网卡 的传输设置为延迟 100 毫秒发送。
	tc qdisc add dev eth0 root netem loss 1%	//该命令将 eth0 网卡 的传输设置为随机丢掉 1% 的数据包

