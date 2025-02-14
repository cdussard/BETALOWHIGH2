function initialize(box)

	dofile(box:get_config("${Path_Data}") .. "/plugins/stimulation/lua-stimulator-stim-codes.lua")
	-- initializes random seed
	math.randomseed(os.time())
    pre_trial_baseline_duration = box:get_setting(2) 
    instruction_duration =  box:get_setting(3) 
    --nb_of_blocks = box:get_setting(4) 
    nb_trials_per_block = box:get_setting(5)
	inter_block_duration = box:get_setting(6)
    breakPostTrial_min_duration = box:get_setting(7) 
	breakPostTrial_max_duration = box:get_setting(8)
	neurofeedback_trial_duration = box:get_setting(9)
	initial_FB_wait_duration = box:get_setting(10) 
    sequence_yoked_nf_blocks = {}
	table.insert(sequence_yoked_nf_blocks, OVTK_StimulationId_Number_13)--NF
	table.insert(sequence_yoked_nf_blocks, OVTK_StimulationId_Number_13)--NF
	table.insert(sequence_yoked_nf_blocks, OVTK_StimulationId_Number_13)-- NF
    table.insert(sequence_yoked_nf_blocks, OVTK_StimulationId_Number_15)-- gonogo
    table.insert(sequence_yoked_nf_blocks, OVTK_StimulationId_Number_14)--YOKED
    table.insert(sequence_yoked_nf_blocks, OVTK_StimulationId_Number_14)--YOKED
    table.insert(sequence_yoked_nf_blocks, OVTK_StimulationId_Number_15)-- gonogo

end


function process(box)

    local t=0

    t = t + 1 
    box:send_stimulation(1, OVTK_StimulationId_ExperimentStart, t, 0)
    t = t + 2

    box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStart, t, 0)--croix pendant 3 premieres s
    t = t + pre_trial_baseline_duration
    box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)

    t = t + 1
    for i = 1, #sequence_yoked_nf_blocks do --nb_of_blocks do
        if sequence_yoked_nf_blocks[i] == OVTK_StimulationId_Number_13 then
            box:send_stimulation(1, OVTK_StimulationId_Label_0C, t, 0) -- instruction imagine
        else
            box:send_stimulation(1, OVTK_StimulationId_Label_0A, t, 0) -- instruction obs
        end
        t = t + instruction_duration
        box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)

        t = t + 1
        for j = 1, nb_trials_per_block do
            t = t + 1
            box:send_stimulation(1, sequence_yoked_nf_blocks[i], t, 0) -- debut trial nf ou yoked / 13 ou 14
            t = t + 1
            
            -- display nb de trials restant ?
            box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStart, t, 0)--croix pendant 3 premieres s
            t = t + pre_trial_baseline_duration
            box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)
        
            t = t + 1

            box:send_stimulation(1, OVTK_StimulationId_Label_1B, t, 0) -- affichage main virtuelle
        
            t = t + initial_FB_wait_duration
            t = t + 0.001 -- latence

            for i = 1, (neurofeedback_trial_duration/1.5) do-- ATTENTION 1.5 EN DUR DANS LE CODE
                box:send_stimulation(1, OVTK_StimulationId_SegmentStart, t, 0)
                t = t + 1.55-- est ce qu'on peut mettre 1.5??
            end	

            t = t + 0.1
            box:send_stimulation(1, OVTK_GDF_End_Of_Trial, t, 0)
        
            t = t + 1 
            box:send_stimulation(1, OVTK_GDF_Artifact_Movement, t, 0)--VOUS POUVEZ BOUGER INSTRUCTION
        
            t = t + math.random(breakPostTrial_min_duration, breakPostTrial_max_duration)
            box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)
        end
        t = t + 1
        box:send_stimulation(1, OVTK_StimulationId_Label_1E, t, 0) --instruction questionnaire 
        t = t + inter_block_duration --1 min
        box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)

        t = t + 1
        if sequence_yoked_nf_blocks[i] == OVTK_StimulationId_Number_13 then
            box:send_stimulation(1, OVTK_StimulationId_Label_1F, t, 0)-- display FB on performance ONLY IN NF trials
            t = t + instruction_duration
            box:send_stimulation(1, OVTK_StimulationId_VisualStimulationStop, t, 0)
        end

        t = t + 1

    end
    t = t + 1 -- voir s'il y a une latence de fermeture du .exe

	box:send_stimulation(1, OVTK_GDF_End_Of_Session, t, 0) -- send end for completeness
	
	t = t + 1
	box:send_stimulation(1, OVTK_StimulationId_ExperimentStop, t, 0)

end