---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by 钟以琛.
--- DateTime: 2018/7/3 17:08
---

Network = {};

--  protocol
Protocol  = {
    Connect = '101',
    Exception = '102',
    Disconnect = '103',
    Message = '104'
}

--  Socket message
function Network.OnSocket(key, data)
    if key == 101 then
        LuaFramework.Util.Log('Connected!');
    elseif key == 102 then
        LuaFramework.Util.Log('Exception!');
    elseif key == 103 then
        LuaFramework.Util.Log('Disconnected!');
    elseif key == 104 then
        LuaFramework.Util.Log('Send msg!');
    else
        LuaFramework.Util.Log('Other~~~');
    end
end