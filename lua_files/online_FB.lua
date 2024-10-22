
function initialize(box)

	dofile(box:get_config("${Path_Data}") .. "/plugins/stimulation/lua-stimulator-stim-codes.lua")
	initial_croix_trial_duration = box:get_setting(2) -- 3s
	nb_of_blocks = box:get_setting(3) -- 2 blocs
	nb_trials_per_block = box:get_setting(4)-- 10 (soit 2 blocks * 10 donc 20 total)
	instruction_duration =  box:get_setting(5) --3s
	inter_block_duration = box:get_setting(6)--1 min 
	initial_FB_wait_duration = box:get_setting(7) -- 2s de main immobile au départ
	trial_FB_duration = box:get_setting(8)	--24s (unity en autonomie sur la gestion des FB)
	-- alternative mettre un param nbOfCycles + duration of cycles et trigger le FB unity 
	end_of_trial_min_duration = box:get_setting(9) --1.5s
	end_of_trial_max_duration = box:get_setting(10)--3.5s
	display_FB = _G[box:get_setting(11)]--main ou jauge
	trigger_FB = _G[box:get_setting(12)]--main ou jauge
	-- initializes random seed
	math.randomseed(os.time())
end

function process(box)

	local t=0
	box:send_stimulation(1, OVTK_StimulationId_ExperimentStart, t, 0)
	-- baseline (une seule par condition ici
	t = t + 3
	box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStart, t, 0)--croix pendant 3 premieres s
	t = t + 5
	box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)

	for i = 1, nb_of_blocks do
		box:send_stimulation(1, OVTK_StimulationId_Label_0C, t, 0) -- instruction imagine
		
		t = t + instruction_duration
		box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)
		t = t + 1
		for i = 1, nb_trials_per_block do
			t = t + 1
		-- display nb de trials restant ?
			box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStart, t, 0)--croix pendant 3 premieres s
	
			t = t + initial_croix_trial_duration
			box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)
		
			t = t + 1
			box:send_stimulation(1, display_FB, t, 0) -- affiche la main ou la jauge
		
			t = t + 1.5--initial_FB_wait_duration
			box:send_stimulation(1, trigger_FB, t, 0)--le FB commence (juste pour epoching mais géré par Unity)
			
			t = t + 0.001 -- latence
			for i = 1, (trial_FB_duration/1.5) do-- si 24s, on a 16 cycles	
				box:send_stimulation(1, OVTK_StimulationId_SegmentStart, t, 0)
				t = t + 1.55
			end	-- normalement trial_FB duration --t = t + trial_FB_duration
			t = t + 0.1
			box:send_stimulation(1, OVTK_GDF_End_Of_Trial, t, 0)-- ends trial : needs to trigger the closing of the .exe file
		
			t = t + 1 -- voir s'il y a une latence de fermeture du .exe
			box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStart, t, 0)--croix de fin
		
			t = t + math.random(end_of_trial_min_duration, end_of_trial_max_duration)
			box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)
		end
		t = t + 1
		box:send_stimulation(1, OVTK_StimulationId_Label_0D, t, 0) --instruction Rest
		t = t + inter_block_duration --1 min
	end
	t = t + 1
	box:send_stimulation(1, OVTK_GDF_End_Of_Session, t, 0) -- send end for completeness
	
	t = t + 1
	box:send_stimulation(1, OVTK_StimulationId_ExperimentStop, t, 0)-- used to cause the acquisition scenario to stop
end
