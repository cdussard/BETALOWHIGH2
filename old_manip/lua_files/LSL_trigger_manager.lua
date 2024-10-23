function initialize(box)

	dofile(box:get_config("${__volatile_ScenarioDir}/lua_files/lua-stimulator-stim-codes.lua"))

end

function process(box)

	box:send_stimulation(1, OVTK_StimulationId_ExperimentStart, t, 0)

end
