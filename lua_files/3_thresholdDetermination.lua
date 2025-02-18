function initialize(box)

	dofile(box:get_config("${Path_Data}") .. "/plugins/stimulation/lua-stimulator-stim-codes.lua")
	baseline_start_duration = box:get_setting(2) --30s
	instruction_duration =  box:get_setting(3) --3s
	end
	
	
function process(box)

	local t=0

    box:send_stimulation(1, OVTK_StimulationId_ExperimentStart, t, 0)
	t = t + 2

	box:send_stimulation(1, OVTK_StimulationId_Label_0D, t, 0) --instruction Rest 
		
	t = t + instruction_duration
	box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)
		
	t = t + 1 
	box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStart, t, 0)--croix pendant baseline
	
	t = t + 0.1
	box:send_stimulation(1, OVTK_StimulationId_BaselineStart, t, 0)
	
	t = t + baseline_start_duration

	box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)
	
	t = t + 1
	box:send_stimulation(1, OVTK_GDF_End_Of_Session, t, 0) -- send end for completeness
	
	t = t + 1
	box:send_stimulation(1, OVTK_StimulationId_ExperimentStop, t, 0)-- used to cause the acquisition scenario to stop

end