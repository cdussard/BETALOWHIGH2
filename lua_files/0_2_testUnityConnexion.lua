function initialize(box)

	dofile(box:get_config("${Path_Data}") .. "/plugins/stimulation/lua-stimulator-stim-codes.lua")
	-- initializes random seed
	math.randomseed(os.time())

end

function process(box)
	local t = 0
    box:send_stimulation(1, OVTK_StimulationId_ExperimentStart, t, 0)
	t = t + 2

	box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStart, t, 0)	-- display cross

	t = t + 3

	box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)	

	t = t + 1

    box:send_stimulation(1, OVTK_StimulationId_Label_1B, t, 0)-- display virtual hand

    t = t + 5
    box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)	

    t = t + 1
    box:send_stimulation(1, OVTK_GDF_End_Of_Session, t, 0) -- send end for completeness
	
	t = t + 1
	box:send_stimulation(1, OVTK_StimulationId_ExperimentStop, t, 0)

end