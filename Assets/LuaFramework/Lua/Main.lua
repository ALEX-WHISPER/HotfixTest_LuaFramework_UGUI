require 'Logic.GunComponent'

--主入口函数。从这里开始lua逻辑
function Main()
	LuaHelper = LuaFramework.LuaHelper
	resMgr = LuaHelper.GetResManager()

	resMgr:LoadPrefab('scifi-gun', { 'SciFiGun_Diffuse' }, OnLoadFinish)	--(void)LoadPrefab(string abName, string assetName, Action<UObject[]> func)
end

--加载完成后的回调
function Update()
	
end

function OnLoadFinish(objs)
	LuaFramework.Util.Log("---load scifi-gun finished")
	go = UnityEngine.GameObject.Instantiate(objs[0])
	LuaComponent.Add(go,GunComponent)

	UpdateBeat: Add(Update, self)
end

--场景切换通知
function OnLevelWasLoaded(level)
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end

function OnApplicationQuit()
end