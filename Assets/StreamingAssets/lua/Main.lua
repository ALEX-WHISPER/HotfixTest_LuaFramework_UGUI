--主入口函数。从这里开始lua逻辑
function Main()					
	--print("logic start")
	--LuaFramework.Util.Log("HelloWorld from local")

	local go = UnityEngine.GameObject('go')
	go.transform.position = Vector3.one
end

--场景切换通知
function OnLevelWasLoaded(level)
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end

function OnApplicationQuit()
end