# essential-wow
Min maxing WoW UI &amp; more

https://github.com/tukui-org/ElvUI/wiki/performance-optimization


main.lua

	function CheckUpdateName(frame)
		local o = frame.state.name
		local n = o

		words = {}
		for word in o:gmatch("%w+") do name = word end

		frame.state.name = name
		frame.NameText:SetText(name)

		-- local name = frame.state.name

		-- for k, v in pairs(opt.env.Renames) do
			-- if (k == name) then
				-- frame.state.name = v
				-- frame.NameText:SetText(v)
			-- end
		-- end
	end
	
create.lua

    local function Arrows_UpdatePosition(self)
	self.l:SetPoint('RIGHT',self.parent.bg,'LEFT',25,0)
	self.r:SetPoint('LEFT',self.parent.bg,'RIGHT',-25,0)
    end

![alt text](ICONS/ABILITY_DRUID_DEMORALIZINGROAR.png)

todo: automatic img downloader https://www.wowdb.com/spells/class-abilities/shaman
todo: automatic spell priority generator
