using System.Collections;
using UnityEngine;
using LuaInterface;
using LuaFramework;

public class LuaComponent : MonoBehaviour {
    public LuaTable table;

    public const string funcName_Awake = "Awake";
    public const string funcName_Start = "Start";
    public const string funcName_Update = "Update";

    /// <summary>
    /// AddComponent
    /// </summary>
    /// <param name="go"></param>
    /// <param name="tableClass"></param>
    /// <returns></returns>
    public static LuaTable Add(GameObject go, LuaTable tableClass) {
        LuaFunction luaFunc = tableClass.GetLuaFunction("New");
        if (luaFunc == null) {
            return null;
        }

        object rets = luaFunc.Invoke<LuaTable, object>(tableClass);
        if (rets == null) {
            return null;
        }

        LuaComponent _com = go.AddComponent<LuaComponent>();
        _com.table = (LuaTable)rets;
        _com.CallAwake();

        return _com.table;
    }

    /// <summary>
    /// GetComponent
    /// </summary>
    /// <param name="go"></param>
    /// <param name="table"></param>
    /// <returns></returns>
    public static LuaTable Get(GameObject go, LuaTable table) {
        LuaComponent[] _coms = go.GetComponents<LuaComponent>();

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
    /// Remove component
    /// </summary>
    /// <param name="go"></param>
    /// <param name="table"></param>
    /// <returns></returns>
    public static LuaTable Remove(GameObject go, LuaTable table) {
        LuaComponent[] _coms = go.GetComponents<LuaComponent>();

        foreach (var _com in _coms) {
            string mat1 = table.ToString();
            string mat2 = _com.table.GetMetaTable().ToString();

            if (mat1 == mat2) {
                Destroy(_com);
            }
        }

        return null;
    }

    private void CallAwake() {
        CallLuaFunc(funcName_Awake);
    }

    private void Start() {
        CallLuaFunc(funcName_Start);
    }

    private void Update() {
        CallLuaFunc(funcName_Update);
    }

    private void CallLuaFunc(string luaFuncName) {
        LuaFunction luaFunc = table.GetLuaFunction(luaFuncName.Trim());

        if (luaFunc != null) {
            luaFunc.Call(table, gameObject);
        }
    }
}