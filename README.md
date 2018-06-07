# LuaFramework_UGUI Hotfix Testing

---
## Neccessary Links

+ [toLua#](https://github.com/topameng/tolua)
+ [LuaFramework_UGUI](https://github.com/jarjin/LuaFramework_UGUI)
+ [Lua Dev IDE: IntelliJ IEDA](https://www.jetbrains.com/idea/download/#section=windows)
+ [Code Completion Plugin](https://link.zhihu.com/?target=https%3A//plugins.jetbrains.com/plugin/9768-emmylua)
+ [How to build a local IIS server](https://github.com/ALEX-WHISPER/Intro_BuildingIISLocalServer)

## Initialize Config
+ Download [LuaFramework_UGUI](https://github.com/jarjin/LuaFramework_UGUI) project and open in Unity
+ **Lua -> Generate All**
+ **LuaFramework -> Build Windows Resouce**
+ Enter **Assets\LuaFramework\Scenes\main.unity**, hit play, if it worked well, go to next step

## HelloWorld
+ Create a new scene
+ Add an empty GameObject (which has to be) called **GameManager**, add component: **Main.cs**
+ Write custom code to log "HelloWorld"
    + Modify lua entry in "GameManager.cs"
        + Open **GameManager.cs**, comment this block of code in method **OnInitialize()** belowed:
        ``` 
            LuaManager.DoFiele(Logic/Game);         //加载游戏
            LuaManager.DoFile("Logic/Network");      //加载网络
            NetManager.OnInit();                     //初始化网络
            Util.CallMethod("Game", "OnInitOK");     //初始化完成  
        ```

        + Open ``` Assets\LuaFramework\Lua\Main.lua ```
        + Comment ``` print("logic start") ``` 
        + Add ``` LuaFramework.Util.Log("HelloWorld from local") ```
        
        + Back to editor
            - **Lua -> Generate All**
            - **LuaFramework -> Build Windows Resouce**
            - Hit play, check Console tab

## Test Hotfix
+ Build local IIS server, see [How to build a local IIS server](https://github.com/ALEX-WHISPER/Intro_BuildingIISLocalServer) if you need help on this
+ Open **AppConst.cs**
    + set **UpdateMode** to **true**
    + set **LuaBundleMode** to **true**
    + set **WebUrl** be your **local server url**
+ Make some changes(**A**) in Main.lua
    + In **Main.lua**, replace the previous log code to: ``` LuaFramework.Util.Log("HelloWorld from server") ```
    + Back to editor, **Lua->Build Windows Resource**
+ Copy all the files in **StreamingAssets** to your local server
+ Make some changes(**B**) in Main.lua again, for local overriding
    + In **Main.lua**, replace the previous log code to: ``` LuaFramework.Util.Log("HelloWorld from local") ```
    + Back to editor, **Lua->Build Windows Resource**
+ Hit Play, you'll see **A** is the log result, which is ``` HelloWorld from server ```

## Issue
     I've had stuck in the last step on Test Hotfix that Console doesn't print neither 'HelloWold from server' nor 'HelloWorld from local', seems like some files haven't been downloaded from server. And below is my solution.

+ Go to IIS
+ Select your local server website
+ Double click **MIME Types** and hit **Add** in the right panel
    + **File name extension**: ```.*```
    + **MIME type**: ```application/octet-stream```
+ Back to Unity and play again, problem solved.

---
## 相关链接
+ [toLua#](https://github.com/topameng/tolua)
+ [LuaFramework_UGUI](https://github.com/jarjin/LuaFramework_UGUI)
+ [Lua脚本IDE: IntelliJ IEDA](https://www.jetbrains.com/idea/download/#section=windows)
+ [代码补全插件](https://link.zhihu.com/?target=https%3A//plugins.jetbrains.com/plugin/9768-emmylua)
+ [搭建本地IIS网站](https://github.com/ALEX-WHISPER/Intro_BuildingIISLocalServer)

## 框架初始化
+ 下载 [LuaFramework_UGUI](https://github.com/jarjin/LuaFramework_UGUI)，并在Unity中打开
+ 上方工具栏中，**Lua -> Generate All**，**LuaFramework -> Build Windows Resouce**
+ 进入场景：**Assets\LuaFramework\Scenes\main.unity**，运行，若弹出示范的UI界面，则初始化成功，可继续下一步

## 打印 HelloWorld
+ 新建场景
+ 添加一个名称(必须)为 **GameManager** 的空物体, 并向该物体添加脚本组件: **Main.cs**
+ 添加本地Lua代码
    + 修改 GameManager 的lua入口 
        + 打开 **GameManager.cs**, 注释 **OnInitialize()** 方法中的以下代码:
        ``` 
            LuaManager.DoFiele(Logic/Game);         //加载游戏
            LuaManager.DoFile("Logic/Network");      //加载网络
            NetManager.OnInit();                     //初始化网络
            Util.CallMethod("Game", "OnInitOK");     //初始化完成  
        ```

        + 打开 ``` Assets\LuaFramework\Lua\Main.lua ```
        + 注释 ``` print("logic start") ``` 
        + 添加 ``` LuaFramework.Util.Log("HelloWorld from local") ```
        
        + 回到 Unity 编辑器
            - **Lua -> Generate All**
            - **LuaFramework -> Build Windows Resouce**
            - 运行，查看 Console 中的输出内容

## 热更新测试
+ 搭建IIS本地服务器, 如需帮助，可查看该[简易指南](https://github.com/ALEX-WHISPER/Intro_BuildingIISLocalServer)
+ 修改配置
    + 打开 **AppConst.cs**
    + 将 **UpdateMode** 设置为 **true**
    + 将 **LuaBundleMode** 设置为 **true**
    + 将 **WebUrl** 设置为**本地服务器的url**
+ 修改 Main.lua(改动A)
    + 打开 **Main.lua**, 将之前添加的 log 代码替换为: ``` LuaFramework.Util.Log("HelloWorld from server") ```
    + 回到编辑器, **Lua->Build Windows Resource**
+ 将 **StreamingAssets** 中的所有文件复制并移动到本地 IIS 服务器目录下
+ 再次修改 Main.lua(改动B), 用于覆盖本地资源
    + 打开 **Main.lua**, 将 log 代码替换为: ``` LuaFramework.Util.Log("HelloWorld from local"); ```
    + 回到编辑器, **Lua->Build Windows Resource**
+ 运行, 正常情况下应在 Console 中看到**改动A**的输出结果, 即 ``` HelloWorld from server ```

## 遇到的问题
     在热更新测试的最后一步，Console 即不打印 "HelloWorld from server" 也不打印 "HelloWorld from local"。打开服务器下文件目录，可看到存在一无后缀的文件：StreamingAssets ，该文件未能成功从服务器获取并下载至本地，造成文件缺失，无法正常执行 lua 代码。以下为本人的解决方案。

+ 进入 IIS
+ 选择先前搭建的 IIS 网站
+ 双击 **MIME 类型** 属性
+ 在右侧面板，单击 **添加**
    + **文件扩展名**: ```.*```
    + **MIME 类型**: ```application/octet-stream```
+ 回到 Unity 再次运行，问题解决