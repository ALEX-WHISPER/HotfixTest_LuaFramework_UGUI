using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;
using UnityEngine.UI;

public class LuaUIComponent: LuaComponent {
    private static Dictionary<string, LuaFunction> buttons = new Dictionary<string, LuaFunction>();

    public static new LuaTable Add(GameObject go, LuaTable tableClass) {
        LuaFunction luaFunc = tableClass.GetLuaFunction("New");
        if (luaFunc == null) {
            return null;
        }

        object rets = luaFunc.Invoke<LuaTable, object>(tableClass);
        if (rets == null) {
            return null;
        }

        LuaUIComponent _com = go.AddComponent<LuaUIComponent>();
        _com.table = (LuaTable)rets;
        _com.CallAwake();

        return _com.table;
    }

    public static new LuaTable Get(GameObject go, LuaTable table) {
        LuaUIComponent[] _coms = go.GetComponents<LuaUIComponent>();

        foreach (var _com in _coms) {
            string mat1 = table.ToString();
            string mat2 = _com.table.GetMetaTable().ToString();

            if (mat1 == mat2) {
                return _com.table;
            }
        }

        return null;
    }

    /// <summary>
    /// 添加单击事件
    /// </summary>
    public static void AddListener(GameObject go, LuaFunction luafunc) {
        if (go == null || luafunc == null) return;
        buttons.Add(go.name, luafunc);
        go.GetComponent<Button>().onClick.AddListener(
            delegate () {
                luafunc.Call(go);
            }
        );
    }

    /// <summary>
    /// 删除单击事件
    /// </summary>
    /// <param name="go"></param>
    public static void RemoveListeners(GameObject go) {
        if (go == null) return;
        LuaFunction luafunc = null;
        go.GetComponent<Button>().onClick.RemoveAllListeners();

        if (buttons.TryGetValue(go.name, out luafunc)) {
            luafunc.Dispose();
            luafunc = null;
            buttons.Remove(go.name);
        }
    }

    /// <summary>
    /// 清除单击事件
    /// </summary>
    public static void ClearClick() {
        foreach (var de in buttons) {
            if (de.Value != null) {
                de.Value.Dispose();
            }
        }
        buttons.Clear();
    }
}
