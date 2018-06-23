--主入口函数。从这里开始lua逻辑
function Main()
	LuaHelper = LuaFramework.LuaHelper
	resMgr = LuaHelper.GetResManager()

	--(void)LoadPrefab(string abName, string assetName, Action<UObject[]> func)
	resMgr:LoadPrefab('scifi-gun', { 'SciFiGun_Diffuse' }, OnLoadFinish)

end

--加载完成后的回调
function Update()
	local hInput = UnityEngine.Input.GetAxis("Horizontal")
	local vInput = UnityEngine.Input.GetAxis("Vertical")

	local destVec = go.transform.position + Vector3.New(hInput, vInput) * 0.5 * UnityEngine.Time.deltaTime
	go.transform.position = destVec
end

function OnLoadFinish(objs)
	LuaFramework.Util.Log("---load scifi-gun finished")
	go = UnityEngine.GameObject.Instantiate(objs[0])
	
	moveSpeed = Vector3.New(2,2,0)
	UpdateBeat: Add(Update, self)
end

--场景切换通知
function OnLevelWasLoaded(level)
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end

function OnApplicationQuit()
end