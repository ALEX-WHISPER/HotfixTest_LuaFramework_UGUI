GunComponent = {
	name = 'scifi-gun',
	damageAmount = 10,
	health = 100,
	moveSpeed = Vector3.New(0.5,1,0)
}

function GunComponent: Awake(gameObject)
	print("GunCmp Awake name = "..self.name );
	print("GunCmp Awake gameObject.name = "..gameObject.name );
end

function GunComponent: Start(gameObject)
	print("GunCmp Start name = "..self.name );
end

function GunComponent: Update(gameObject)
	local hInput = UnityEngine.Input.GetAxis("Horizontal")
	local vInput = UnityEngine.Input.GetAxis("Vertical")

	local destVec = gameObject.transform.position + Vector3.New(hInput * self.moveSpeed.x, vInput * self.moveSpeed.y) * UnityEngine.Time.deltaTime
	gameObject.transform.position = destVec
end

--创建对象
function GunComponent: New(obj)
	local o = {} 
    setmetatable(o, self)  
    self.__index = self  
	return o
end  