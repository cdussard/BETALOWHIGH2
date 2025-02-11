
function initialize(box)

	dofile(box:get_config("${__volatile_ScenarioDir}/lua_files/lua-stimulator-stim-codes.lua"))

	baseline_duration = box:get_setting(2)
    pause_duration = box:get_setting(3)
	-- initializes random seed
	math.randomseed(os.time())

end

function process(box)

	local t = 0
	
	t = t + 2
		-- start xp
	box:send_stimulation(1, OVTK_StimulationId_ExperimentStart, t, 0)
	
	t = t + 2
	box:send_stimulation(1, OVTK_StimulationId_LabelStart, t, 0)
	
	t = t +1
	for i = 1,50 do
		box:send_stimulation(1, OVTK_StimulationId_SegmentStop, t, 0)
	
		t = t + 2

	end
	
	box:send_stimulation(1, OVTK_StimulationId_ExperimentStop, t, 0)
	
	
end
