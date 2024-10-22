
function initialize(box)

	dofile(box:get_config("${__volatile_ScenarioDir}/lua_files/lua-stimulator-stim-codes.lua"))
	baseline_duration = box:get_setting(2)
	instruction_duration = box:get_setting(3)
	
	codesStimVisuelle = {}
	table.insert(codesStimVisuelle,OVTK_StimulationId_Label_0A)--obs
	table.insert(codesStimVisuelle,OVTK_StimulationId_Label_0B)--imit
	table.insert(codesStimVisuelle,OVTK_StimulationId_Label_0C)--imagine


end

function process(box)

	local t = 0

	-- manages baseline

	box:send_stimulation(1, OVTK_StimulationId_ExperimentStart, t, 0)--output_index, stimulation_identifier, stimulation_date, opt:stimulation_duration=0
	
	t = t + 1
	
	for i=1,3 do
	---------baseline -------------
	box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStart, t, 0)
	t = t + baseline_duration
	box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)	

	t = t + 1
	
	---------instruction -------------
	box:send_stimulation(1, codesStimVisuelle[i], t, 0)	--affichage d'instruction 
	t = t + instruction_duration
	box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)
	
	t = t + 1

	---------affichage main virtuelle -------------
	box:send_stimulation(1, OVTK_StimulationId_Label_1B, t, 0)

	for j = 1, 20 do--((trial_duration)/1.5) do-- si 30s, on a 20 cycles	
		box:send_stimulation(1, OVTK_StimulationId_SegmentStart, t, 0)
		t = t + 1.5--epoching sur 7.5s de 1s à 6.5s (on jette la premiere seconde et la derniere seconde)
	end	
	t = t + 0.1
	box:send_stimulation(1, OVTK_GDF_End_Of_Trial, t, 0)

	t = t + 1 

	end

	t = t + 1
	box:send_stimulation(1, OVTK_GDF_End_Of_Session, t, 0) -- send end for completeness
	
	t = t + 1
	box:send_stimulation(1, OVTK_StimulationId_ExperimentStop, t, 0)-- used to cause the acquisition scenario to stop
	
end