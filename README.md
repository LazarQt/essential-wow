# essential-wow
Min maxing WoW UI &amp; more

https://github.com/tukui-org/ElvUI/wiki/performance-optimization

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
