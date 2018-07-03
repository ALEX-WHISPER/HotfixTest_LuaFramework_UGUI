require 'Logic.GunComponent'
require 'Logic.UITestPanel'

--主入口函数。从这里开始lua逻辑
function Main()
	--ResourceOperating()
	NetworkOperating()
end

function NetworkOperating()
	local LuaHelper = LuaFramework.LuaHelper;
	local appConst = LuaFramework.AppConst;

	local networkManager = LuaHelper.GetNetManager();

	appConst.SocketAddress = '127.0.0.1';
	appConst.SocketPort = 12121;

	networkManager:SendConnect();
end

function ResourceOperating()
	LuaHelper = LuaFramework.LuaHelper
	resMgr = LuaHelper.GetResManager()
	panelMgr = LuaHelper.GetPanelManager()

	--(void)LsoadPrefab(string abName, string assetName, Action<UObject[]> func)
	resMgr:LoadPrefab('scifi-gun', { 'SciFiGun_Diffuse' }, OnLoadFinish)

	-- 创建panel的两种途径：
	-- 1. 将panel作为普通预制件资源进行加载和实例化
	resMgr:LoadPrefab('uitest', {'UITestPanel'}, OnLoadUIPrefabsFinish)

	-- 2. 调用框架api, 会自动添加LuaBehaviour组件
	panelMgr:CreatePanel('UITest', OnLoadUIPrefabsFinish)
end

--加载完成后的回调
function Update()
	
end

--region callbacks onload
function OnLoadFinish(objs)
	LuaFramework.Util.Log("---load scifi-gun finished")
	go = UnityEngine.GameObject.Instantiate(objs[0])
	LuaComponent.Add(go,GunComponent)
	
	UpdateBeat: Add(Update, self)
end

function OnLoadUIPrefabsFinish(objs)
	local canvasNode = UnityEngine.GameObject.Find('Canvas').transform
	local panelObj = UnityEngine.GameObject.Instantiate(objs[0], canvasNode)

	if panelObj ~= nil then
		LuaUIComponent.Add(panelObj,UITestPanel)
	else
		print('cannot get panelObj')
	end
end
--endregion

--场景切换通知
function OnLevelWasLoaded(level)
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end

function OnApplicationQuit()
end