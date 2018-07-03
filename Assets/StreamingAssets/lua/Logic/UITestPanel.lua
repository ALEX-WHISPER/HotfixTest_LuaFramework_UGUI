UITestPanel = {
	name = 'UITestPanel',
	gameObject,
	
	redBtnName = 'redBtn',
	greyBtnName = 'greyBtn',
	brownBtnName = 'brownBtn'
}

function UITestPanel: Awake(gameObject)
	print('UITestPanel.Awake: '..gameObject.name)
		
	-- get child btns
	local redBtn = gameObject.Find(self.redBtnName)
	local greyBtn = gameObject.Find(self.greyBtnName)
	local brownBtn = gameObject.Find(self.brownBtnName)

	-- add listeners
	LuaUIComponent.AddListener(redBtn, OnRedBtnClick)
	LuaUIComponent.AddListener(greyBtn, OnGreyBtnClick)
	LuaUIComponent.AddListener(brownBtn, OnBrownBtnClick)
end

function UITestPanel: Start(gameObject)
end

function UITestPanel: Update()

end

function OnRedBtnClick()
	print('---click red btn---')
end

function OnGreyBtnClick()
	print('---click grey btn---')
end

function OnBrownBtnClick()
	print('---click brown btn---')
end

--创建对象
function UITestPanel: New(obj)
	local o = {} 
    setmetatable(o, self)  
    self.__index = self  
	return o
end