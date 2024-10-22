
function initialize(box)

	dofile(box:get_config("${__volatile_ScenarioDir}/lua_files/lua-stimulator-stim-codes.lua"))

	baseline_duration = box:get_setting(2)
    instruction_duration = box:get_setting(3)

	-- initializes random seed
	math.randomseed(os.time())

end

function process(box)

	local t = 0

	-- manages baseline
	box:send_stimulation(1, OVTK_StimulationId_ExperimentStart, t, 0)
	
	t = t + 2

	-- affichage instruction rest
	box:send_stimulation(1, OVTK_StimulationId_Label_0D, t, 0)
	t = t + instruction_duration
	box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)

	t = t + 1

	-- affichage de la croix et resting state
	box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStart, t, 0)
	t = t + baseline_duration
	box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)

	t = t + 1
	
	box:send_stimulation(1, OVTK_GDF_End_Of_Session, t, 0) -- send end for completeness
	
	-- used to cause the acquisition scenario to stop
	t = t + 1
	box:send_stimulation(1, OVTK_StimulationId_ExperimentStop, t, 0)
	
end
