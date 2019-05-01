function;EtInit{
	isfunction;
	newmodule;et;C:\\Users\\Лехап\\Documents\\Visual Studio 2015\\Projects\\game\\game\\bin\\Debug\\EthernetModule.dll
	module;et;Start:p=t
	mess;$EthernetRequest
	string;ConnectParams:localhost
	string;DownloadFiles:localhost
}
function;EtConnectServer{
	isfunction;
	module;et;ConnectWeb:url=$ConnectParams
	string;ServerRequstConnect:$EthernetRequest
	replace;$ServerRequstConnect;|;^;ServerRequstConnect
}
function;EtDownloadFile{
	isfunction;
	cashe;$ConnectParams;$DownloadFiles
}