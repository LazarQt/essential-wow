function (self, unitId, unitFrame, envTable, modTable)
     
    local name = unitFrame.castBar.SpellName
   
    if name == nil
    then 
        return
    end
    
    
    
    local str = name
    local words = {}
    for word in str:gmatch("%S+") do
        table.insert(words, word)
    end
    
    -- local x = GetSpellCooldown("Wind Shear")
    local n = words[#words]
    -- if x <= 0 then
    --    n = n .. " KICK"
    --end
    --print(unitFrame.castBar.SpellID)
	
	local spells = {
		[257274] = "TRAP",
		[257426] = "CLEAVE",
		[257732] = "AOE SILENCE",
		[257478] = "BITE",
		[257476] = "EMPOWER",
		[257437] = "POISON",
		[257397] = "HEAL",
		[258672] = "GRENADE",
		[274400] = "CHARGE",
		[258777] = "GRENADE",
		[274383] = "TRAP",
		[274555] = "BITE",
		[257747] = "AOE CC",
		[257871] = "BLADESTORM",
		[257741] = "ENRAGE",
		[272402] = "SIVIR WEAPON",
		[257784] = "BOMB",
		[281420] = "CAST",
		[258199] = "AOE CC",
		[276061] = "PHYSICAL GRENADE",
		[259092] = "CAST",
		[257736] = "BOMB",
		[257899] = "TYRANNICAL BUFF",
		[257871] = "BLADESTORM",
		[257908] = "ELEMENTAL SWORD"
	}

	if spells[unitFrame.castBar.SpellID] then
		abilityToIcon = {
		  ["TRAP"] = "Ability_hunter_traplauncher",
		  ["CLEAVE"] = "Ability_warrior_cleave",
		  ["AOE SILENCE"] = "Warrior_disruptingshout",
		  ["BITE"] = "Ability_druid_primaltenacity",
		  ["EMPOWER"] = "Ability_warrior_bloodfrenzy",
		  ["POISON"] = "Ability_creature_poison_05",
		  ["HEAL"] = "Spell_holy_divineprovidence",
		  ["GRENADE"] = "Inv_misc_enggizmos_38",
		  ["CHARGE"] = "Ability_warrior_charge",
		  ["AOE CC"] = "Ability_warstomp",
		  ["BLADESTORM"] = "Ability_warrior_bladestorm",
		  ["ENRAGE"] = "Ability_warlock_improveddemonictactics",
		  ["SIVIR WEAPON"] = "Ability_glaivetoss",
		  ["BOMB"] = "Spell_shaman_improvedfirenova",
		  ["CAST"] = "Inv_summerfest_firespirit",
		  ["PHYSICAL GRENADE"] = "Inv_misc_bomb_08",
		  ["TYRANNICAL BUFF"] = "Inv_summondemonictyrant",
		  ["ELEMENTAL SWORD"] = "Inv_sword_2h_pvpcataclysms3_c_01"
		}

		envTable.NewIcon = abilityToIcon[spells[unitFrame.castBar.SpellID]]
	
	end
	
	-- if unitFrame.castBar.SpellID == 9053 then 
       -- envTable.NewIcon = "spell_holy_avengersshield"    
    -- end
    --unitFrame.castBar.Text:SetText(n)      
    --unitFrame.castBar.Icon:SetTexture("Interface\\Icons\\Ability_monk_jadeserpentbreath")
    
    --  print(unitFrame.castBar.SpellID)
    envTable.KaliSpell = n
    -- unitFrame.castBar.Icon:SetSize (unitFrame.healthBar:GetHeight(), unitFrame.healthBar:GetHeight())
    -- --  unitFrame.castBar.Icon:ClearAllPoints()
    --  unitFrame.castBar.Icon:SetPoint ("topleft", unitFrame.healthBar, "topleft", 0, 0)
    --  unitFrame.castBar.Icon:SetTexture("Interface\\Icons\\Ability_Ambush")
    
    
    
end








