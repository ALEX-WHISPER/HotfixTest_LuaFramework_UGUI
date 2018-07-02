GunComponent = {
	name = 'scifi-gun',
	damageAmount = 10,
	health = 100,
}

function GunComponent: Awake()
	print("GunCmp Awake name = "..self.name );
end

function GunComponent:Start()
	print("GunCmp Start name = "..self.name );
end

function GunComponent:Update()
	print("GunCmp Update name = "..self.name );
end

--创建对象
function GunComponent:New(obj)
	local o = {} 
    setmetatable(o, self)  
    self.__index = self  
	return o
end  