function initialize(box)

	dofile(box:get_config("${Path_Data}") .. "/plugins/stimulation/lua-stimulator-stim-codes.lua")
	-- initializes random seed
	math.randomseed(os.time())

end


function process(box)
	local t = 0

	box:send_stimulation(1, OVTK_StimulationId_ExperimentStart, t, 0)
	t = t + 2
	for i = 1,2 do
		---------baseline -------------
		box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStart, t, 0)
		t = t + 3 
		box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)	
		t = t + 0.1
		
		---------instruction -------------
		box:send_stimulation(1, OVTK_StimulationId_Label_0A, t, 0) --instruction observe
		t = t + 3
		box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)

		t = t + 1

		---------affichage main virtuelle -------------
		box:send_stimulation(1, OVTK_StimulationId_Label_1B, t, 0)

		for j = 1, 20 do--((trial_duration)/1.5) do-- si 30s, on a 20 cycles	
			box:send_stimulation(1, OVTK_StimulationId_SegmentStart, t, 0)-- a virer ??
			t = t + 1.5--epoching sur 7.5s de 1s à 6.5s (on jette la premiere seconde et la derniere seconde)
		end	
		t = t + 0.1
		box:send_stimulation(1, OVTK_GDF_End_Of_Trial, t, 0)
	
	end
    
    t = t + 1 -- voir s'il y a une latence de fermeture du .exe

	box:send_stimulation(1, OVTK_GDF_End_Of_Session, t, 0) -- send end for completeness
	
	t = t + 1
	box:send_stimulation(1, OVTK_StimulationId_ExperimentStop, t, 0)
end