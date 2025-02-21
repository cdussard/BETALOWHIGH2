using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using Assets.LSL4Unity.Scripts.AbstractInlets;
using Assets.LSL4Unity.Scripts;
using Assets.LSL4Unity.Scripts.Examples;
using System.IO;
using UnityEngine.EventSystems;
using System.Globalization;
using UnityEngine.UI;
using System.Text;
using TMPro;


public class neurofeedbackBETALOWHIGH2 : AFloatInlet
{
    private float switchTime = 5f; // Switch every 5 seconds
    private float timerde = 0f;
    private bool isOtherAppActive = false;

    // switch entre les scenes
    public WindowManager script_windowManager;

    // gestion du neurofeedback
    //seuils 
    string m_Path;
    string path_editor = @"..\..\..\..\csv_files\";
    string path_build = @"..\..\..\csv_files\";
    public float seuilPireBeta = 10.417f;
    public float seuilMeilleurBeta = 3.9141f;
    public float seuilPireBeta_parasite = 0.05f;
    public float seuilMeilleurBeta_parasite = 0.02f;
    int numSujet;
    int numSession;
    int num_sequence;
    float slope;
    float intercept;
    // samples
    float[] lastSample = new float[2];
    public float currentBetaValue = -1.0f;
    public float currentParasiteBetaValue = -1.0f;
    float moyenneBetaEnCours;
    float moyenneBetaEnCours_parasite;
    int indiceStartSlice = -1;
    int longueurMoyennage = 4;
    List<int> listeIndicesStartMoyennage = new List<int>();
    public bool collecterValeursBeta = false;
    public List<float> savingToutesValeursBeta = new List<float>();//on en fait rien d'habitude
    public List<float> savingToutesValeursBeta_parasite = new List<float>();//on en fait rien d'habitude
    public List<float> savingValeursBetaEssai = new List<float>();
    public List<float> savingValeursBetaEssai_parasite = new List<float>();
    public List<float> savingValeursBetaMoyenneEssai = new List<float>();
    public List<float> savingValeursBetaMoyenneEssai_parasite = new List<float>();
    List<float> savingToutesValeursVitesse = new List<float>();
    public List<float> savingValeursAmplitudeEssai = new List<float>();
    //public float moyenneBlocNF = 0;
    //affichage mouvements main
    List<bool> listeBoolFeedbacksEffectues = new List<bool>();
    int nbCycles = 16;
    Animator animHuman;
    public int cycleEnCours = 0;//correspond au nbr de cycles de mvt effectues
    float waitTime = 0.74999f;
    public float current_speed;
    public float vitesseMainMaxMovement = 1.25f;
    //gestion yoked FB
    int nbBlocsYoked = 3;
    private int nbEssaisYoked = 5;
    List<List<List<float>>> listeGlobaleYoked = new List<List<List<float>>>();
    List<int> listeMoyenneAmplitudes = new List<int>{ 55,60, 65, 70, 75 };
    public float tolerance =0.5f;
    //gestion feedback
    int perfNulSeuil = 0;
    int perfBadSeuil = 0;
    int perfMoySeuil = 0;

    string langueInstructions = "";
  

    //objets affichage
    public GameObject Parent;
    public GameObject Croix;
    public GameObject main;
    public GameObject Imaginer;
    public GameObject Observer;
    public GameObject PausePostTrial;
    public GameObject GO_feedbackPerf;
    Text FeedbackPerformance;
    public Text Debug_texte;
    string modeDebug;
    public GameObject circleDebug;
    string nomGonoGo = "6_GoNoGo";


    // gestion des stims
    private Dictionary<int, string> dictStim;
    string currentStim = String.Empty;
    public string currentState = "";
    public string yokedOrNF = "";
    public string NFtrain = "";

    //gestion des sauvegardes / comptage cycles trials blocs
    //yoked
    int blocYokedEnCours = 0;
    //nf
    int blocNfEnCours = 0;
    //global
    int trialTotalEnCours = 0;
    int blocTotalEnCours = 0;
    public int blocEnCours = -1;
    public int trialEnCoursDansBloc = -1;
    //cycleEnCours = see above : gestion affichage
    StreamWriter writerBeta;

    public List<string> values;


    void Awake()
    {
        Debug.Log("awake");
        dictStim = CreateDictStim();
        for (int i = 0; i < nbCycles; i++)
        {
            listeBoolFeedbacksEffectues.Add(false);
            listeIndicesStartMoyennage.Add(1 + 6 * i);
        }
        
        //GENERER SEQUENCE
        listeGlobaleYoked = genererListeAmplitudeYokedFB_V2(nbBlocsYoked,nbCycles,listeMoyenneAmplitudes,listeGlobaleYoked,tolerance);
        Debug.Log("generated");
        Application.runInBackground = true;
        animHuman = main.GetComponent<Animator>();
        definePath();
        ReadSequence();
        ReadSujetSession();
        LireConfig();
        if (langueInstructions=="FR")
        {
            PausePostTrial.GetComponent<Text>().text = "Vous pouvez bouger !";
            
        }
        else if (langueInstructions=="EN")
        {
            PausePostTrial.GetComponent<Text>().text = "You can move!";
            Transform canvasTransform = GameObject.Find("Canvas").transform;
            Imaginer = canvasTransform.Find("imagine_en")?.gameObject;
            Observer = canvasTransform.Find("observer_en")?.gameObject;
        }
        ComputeRegression();
        saveDataTrial(true);
        FeedbackPerformance =  GO_feedbackPerf.GetComponent<Text>();
        script_windowManager.MinimizeApp(nomGonoGo);
    }


    /// <param name="newSample"></param>
    /// <param name="timeStamp"></param>
    protected override void Process(float[] newSample, double timeStamp)
    {
        Debug.Log("received");
        lastSample = newSample;
        currentBetaValue = lastSample[0];
        currentParasiteBetaValue = lastSample[1];
        int indiceStim = 2;
        if (modeDebug=="debug")
        {
            Debug_texte.text = String.Format("target beta : {0} parasite beta : {1} marker : {2}",currentBetaValue.ToString(),currentParasiteBetaValue.ToString(),lastSample[indiceStim].ToString());
        }
        
        if (collecterValeursBeta)
        {
            savingToutesValeursBeta.Add(currentBetaValue);
            savingValeursBetaEssai.Add(currentBetaValue);
            savingToutesValeursBeta_parasite.Add(currentParasiteBetaValue);
            savingValeursBetaEssai_parasite.Add(currentParasiteBetaValue);
        }
        else
        {
            //Debug.Log("discarding beta value");
        }

        if (lastSample[indiceStim] != 0)
        {
            int stim = (int)lastSample[indiceStim];
            if ((lastSample[indiceStim] - stim) == 0)
            {
                //string indiceDico = lastSample[1];
                Debug.Log("marker" + stim.ToString());
                try
                {  //currentStim = dictStim[stim]; //Debug.Log(currentStim);

                if (stim == 33024)//OVTK_StimulationId_LabelStart
                {
                    script_windowManager.MaximizeApp(nomGonoGo);
                }
                    if (stim == 32779)//currentStim == "OVTK_StimulationId_VisualStimulationStart"
                    {
                        currentState = "croix";
                        Debug.Log(currentState);
                        Croix.SetActive(true);

                    }
                    else if (stim == 32771)//currentStim == "OVTK_StimulationId_SegmentStart"
                    {
                        Debug.Log("debut cycle : " + cycleEnCours);
                        if (cycleEnCours < nbCycles && listeBoolFeedbacksEffectues[cycleEnCours] == false)
                        {   
                            if(yokedOrNF=="OBS")
                            {
                                changeVitesseYoked();
                                // still save ERD
                            }
                            else if (yokedOrNF=="NF")
                            {
                                //call the NF function using the variables of ERD
                                changeVitesseNF();
                            }

                        }
                        

                    }
                    else if (stim == 4)//currentStim == "OVTK_StimulationId_Number_04" // NF TRAINING
                    {
                        NFtrain = "train";
                    }

                    else if (stim == 32780)//currentStim == "OVTK_StimulationId_VisualStimulationStop"
                    {
                        Croix.SetActive(false);
                        main.SetActive(false);
                        Imaginer.SetActive(false);
                        Observer.SetActive(false);   
                        GO_feedbackPerf.SetActive(false);
                        PausePostTrial.SetActive(false);
                        currentState = "none";
                        Debug.Log(currentState);
                    }

                    else if (stim == 33051)//currentStim == "OVTK_StimulationId_Label_1B" : affichage main
                    {
                        currentState = "main";
                        Debug.Log(currentState);
                        main.SetActive(true);
                        collecterValeursBeta = true;
                        
                    }
                    else if (stim == 33054)//currentStim =="OVTK_StimulationId_Label_1E"
                    {
                     script_windowManager.MinimizeApp(nomGonoGo);
                    }
        
                    else if (stim == 33036)//currentStim == "OVTK_StimulationId_Label_0C" : debut bloc NF
                    {
                        //moyenneBlocNF = 0;// remet a 0 la moyenne des valeurs beta du bloc NF
                        currentState = "imaginer";
                        yokedOrNF = "NF";
                        trialEnCoursDansBloc = 0;//on reinitialise quand on change de type d'essai = de bloc
                        blocNfEnCours += 1;
                        blocTotalEnCours += 1;
                        Debug.Log(currentState);
                        Imaginer.SetActive(true);

                    }
                    
                    else if (stim == 33034)//currentStim == "OVTK_StimulationId_Label_0A" : debut bloc YOKED
                    {
                        //moyenneBlocNF = 0;// remet a 0 la moyenne des valeurs beta du bloc NF
                        currentState = "observer";
                        yokedOrNF = "OBS";
                        trialEnCoursDansBloc = 0;//on reinitialise quand on change de type d'essai = de bloc
                        blocYokedEnCours += 1;
                        blocTotalEnCours += 1;
                        Debug.Log(currentState);
                        Observer.SetActive(true);

                    }

                    else if (stim == 260)//currentStim == "OVTK_GDF_Artifact_Movement"
                    {
                        currentState = "pause post trial";
                        Debug.Log(currentState);
                        PausePostTrial.SetActive(true);
                    }

                    else if (stim == 800)//"OVTK_GDF_End_Of_Trial")
                    {
                        Debug.Log("END TRIAL");
                        currentState = "endTrial";
                        //moyenneBlocNF += savingValeursAmplitudeEssai.Average();
                        main.SetActive(false);
                        saveDataTrial(false);//TO DO SAVE MESSAGE OBTENU ET MOYENNE BLOC ASSOCIEE
                        ReinitialiserVariables();
                        
    
                    }
                    else if (stim == 1010 || Input.GetKeyDown(KeyCode.Escape))//currentStim ==  "OVTK_GDF_End_Of_Session"
                    {
                        Debug.Log("end of session");
                        //writerBeta.Close();
                        Application.Quit();//fermer la fenetre

                    }

                    else if (stim == 19)//OVTK_StimulationId_Number_13 : essai type NF
                    {
                        trialEnCoursDansBloc += 1;
                        trialTotalEnCours += 1;
                    }

                    else if (stim == 20)//OVTK_StimulationId_Number_14 : essai type OBS YOKED
                    {
                        trialEnCoursDansBloc  += 1;
                        trialTotalEnCours += 1;
                    }

                }
            catch (KeyNotFoundException e)
            {
                // Handle the exception.
                Debug.Log(e.Message);
            }

            }
            else if ((lastSample[2] - stim) != 0)
            {
                Debug.Log("decimal");
                Debug.Log("BUG!!!");
                circleDebug.SetActive(true);
                Application.Quit();//fermer la fenetre
            }
            


        }


    }

    private void saveDataTrial(bool FirstLine)
    {
        var line = "";
        string filePath = m_Path +"save_beta_sujet.csv";
        int lineCount = 0;
        if (yokedOrNF=="NF")
            {
                blocEnCours = blocNfEnCours;
            }
        else if (yokedOrNF=="OBS")
            {
                blocEnCours = blocYokedEnCours;
            }
        if (File.Exists(filePath))
        {
            lineCount = File.ReadAllLines(filePath).Length;
        }
        
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            
            if (FirstLine && lineCount < 2)
            {
                line = "";
                Debug.Log("writing title :");
                line ="type_trial,cycle_within_trial,n_bloc_total,n_bloc_by_type,n_trial_total,n_trial_within_bloc,moyenne_beta,moyenne_beta_parasite,amplitude,heure_date,seuilPireBeta,seuilMeilleurBeta,seuilPireBeta_parasite,seuilMeilleurBeta_parasite,num_sequence,num_sujet,num_session";
                writer.WriteLine(line);
                writer.Flush();

            }
            else
            {
                Debug.Log("saving data :");
                Debug.Log(savingValeursBetaMoyenneEssai.Count());
    
                for (int i = 0; i < savingValeursBetaMoyenneEssai.Count(); i++)
                    {
                        Debug.Log(i);
                        line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}", 
                        yokedOrNF+NFtrain,
                        (i+1).ToString(), //attention ne peut pas etre cycleEnCours pck on save pas a chaque cycle
                        blocTotalEnCours.ToString(),
                        blocEnCours.ToString(),
                        trialTotalEnCours.ToString(),
                        trialEnCoursDansBloc.ToString(),
                        savingValeursBetaMoyenneEssai[i].ToString("F4", CultureInfo.InvariantCulture),
                        savingValeursBetaMoyenneEssai_parasite[i].ToString("F4", CultureInfo.InvariantCulture),
                        savingValeursAmplitudeEssai[i].ToString("F4", CultureInfo.InvariantCulture),
                        DateTime.Now.ToString(),
                        seuilPireBeta.ToString("F4", CultureInfo.InvariantCulture),
                        seuilMeilleurBeta.ToString("F4", CultureInfo.InvariantCulture),
                        seuilPireBeta_parasite.ToString("F4", CultureInfo.InvariantCulture),
                        seuilMeilleurBeta_parasite.ToString("F4", CultureInfo.InvariantCulture),
                        num_sequence.ToString(),
                        numSujet.ToString(),
                        numSession.ToString()
                        );
                        writer.WriteLine(line);
                        writer.Flush();
                    }
            }
        } 

    }

    // Update is called once per frame
   void Update()
    {
  

    }


    private void definePath()
    {
        m_Path = Application.dataPath;
        Debug.Log(m_Path);
        #if UNITY_EDITOR
        {

            m_Path += path_editor;
        }
        #else 
        {
            m_Path += path_build;
        }
        #endif
    }

    private void LireConfig()
    {
        Debug.Log(m_Path + "values_threshold.csv");
        var reader = new StreamReader(File.OpenRead(m_Path + "values_threshold.csv"));
        string headerLine = reader.ReadLine();
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            var values = line.Split(',');
            seuilPireBeta = float.Parse(values[7], CultureInfo.InvariantCulture);
            seuilMeilleurBeta = float.Parse(values[8], CultureInfo.InvariantCulture);
            seuilPireBeta_parasite = float.Parse(values[11], CultureInfo.InvariantCulture);
            seuilMeilleurBeta_parasite = float.Parse(values[12], CultureInfo.InvariantCulture);

        }
        reader = new StreamReader(File.OpenRead(m_Path + "config_neurofeedback.csv"));
        headerLine = reader.ReadLine();
        while ((line = reader.ReadLine()) != null)
        {
            var values = line.Split(',');
            perfNulSeuil = int.Parse(values[2], CultureInfo.InvariantCulture);
            perfBadSeuil = int.Parse(values[3], CultureInfo.InvariantCulture);
            perfMoySeuil = int.Parse(values[4], CultureInfo.InvariantCulture);
            vitesseMainMaxMovement = float.Parse(values[5], CultureInfo.InvariantCulture);
            langueInstructions = (string)values[7];
            modeDebug = (string)values[8];
        }
    }

    private void ReinitialiserVariables()
    {
        Debug.Log("reinitialising environment variables");
        lastSample = new float[2];
        listeBoolFeedbacksEffectues = new List<bool>();
        savingValeursBetaEssai = new List<float>();
        savingValeursBetaEssai_parasite = new List<float>();
        savingValeursAmplitudeEssai = new List<float>();
        savingValeursBetaMoyenneEssai = new List<float>();
        savingValeursBetaMoyenneEssai_parasite = new List<float>();
        currentStim = String.Empty;
        currentBetaValue = 1.0f;

        for (int i = 0; i < nbCycles; i++)
        {
            listeBoolFeedbacksEffectues.Add(false);
        }
        cycleEnCours = 0;
        collecterValeursBeta = false;

    }

    IEnumerator ResetHands()
    {
        Debug.Log("reset hand start");
        yield return new WaitForSeconds(waitTime);
        animHuman.SetTrigger("ToRest");
        animHuman.speed = 1.0f;
    }

    private float getAmplitudeYoked()
    {
        if (yokedOrNF == "OBS")
        {
            current_speed = vitesseMainMaxMovement * (listeGlobaleYoked[blocYokedEnCours-1][trialEnCoursDansBloc-1][cycleEnCours]/100);//ATTENTION BLOC -1 et trial -1 (ils commencent a 1 mais liste indexee a 0)
        }
        return current_speed;
    }

    private void ComputeRegression()
    {
        slope = (0.0f - vitesseMainMaxMovement) / (seuilPireBeta - seuilMeilleurBeta);
        intercept = -slope * seuilPireBeta;

   }

    private void changeVitesseNF()
    {
        listeBoolFeedbacksEffectues[cycleEnCours] = true;
        indiceStartSlice = listeIndicesStartMoyennage[cycleEnCours];
        moyenneBetaEnCours = savingValeursBetaEssai.GetRange(indiceStartSlice, longueurMoyennage).Average();
        moyenneBetaEnCours_parasite = savingValeursBetaEssai_parasite.GetRange(indiceStartSlice, longueurMoyennage).Average();
        Debug.Log("moyenne beta : " + moyenneBetaEnCours);
        savingValeursBetaMoyenneEssai.Add(moyenneBetaEnCours);
        savingValeursBetaMoyenneEssai_parasite.Add(moyenneBetaEnCours_parasite);
        if(moyenneBetaEnCours>=seuilPireBeta)
        {
            Debug.Log("Beta too high : not moving");
            current_speed = 0.0f;
        }

        else if (moyenneBetaEnCours<= seuilMeilleurBeta)
        {
            Debug.Log("Perfect beta : moving at max speed");
            current_speed = vitesseMainMaxMovement;
        }

        else
        {
            Debug.Log("Beta in the good range : moving at linear speed");
            current_speed = intercept + (moyenneBetaEnCours * slope);
        }
        Debug.Log("current speed : " + current_speed.ToString());
        triggerMovement();      

    }

    private void changeVitesseYoked()/// TO DO CHANGER L'AMPLITUDEEEEEEEEEEE
    {
        listeBoolFeedbacksEffectues[cycleEnCours] = true;
        indiceStartSlice = listeIndicesStartMoyennage[cycleEnCours];
        moyenneBetaEnCours = savingValeursBetaEssai.GetRange(indiceStartSlice, longueurMoyennage).Average();
        Debug.Log("moyenne beta : " + moyenneBetaEnCours);
        moyenneBetaEnCours_parasite = savingValeursBetaEssai_parasite.GetRange(indiceStartSlice, longueurMoyennage).Average();
        savingValeursBetaMoyenneEssai.Add(moyenneBetaEnCours);
        savingValeursBetaMoyenneEssai_parasite.Add(moyenneBetaEnCours_parasite);
        //DEFINE CURRENT SPEED with amplitude
        current_speed = getAmplitudeYoked();
        if (current_speed >vitesseMainMaxMovement)
        {
            current_speed = vitesseMainMaxMovement;
        }
        else if (current_speed < 0.0f)
        {
            current_speed = 0.0f;
        }
        triggerMovement();

    }

    private void triggerMovement()
    {
        animHuman.speed = current_speed;
        StartCoroutine(ResetHands());
        animHuman.SetTrigger("ToStateRight");
        cycleEnCours += 1;
        savingToutesValeursVitesse.Add(current_speed);
        savingValeursAmplitudeEssai.Add(current_speed);
        if (cycleEnCours == nbCycles)
        {
            collecterValeursBeta = false;
        }

    }

      private List<List<List<float>>> genererListeAmplitudeYokedFB_V2(int nbBlocsYoked, int nbCycles, List<int> listeMoyennes,List<List<List<float>>> listeGlobaleYoked,float tolerance)
    {

        for (int i = 0 ; i < nbBlocsYoked; i++)
        {
            List<List<float>> liste_bloc_i = new List<List<float>>();

            for (int j = 0 ; j< nbEssaisYoked ; j++)
            {
                List<float> liste_essai_j = new List<float>();
                float sum = 0;
 
                for (int k = 0; k < nbCycles; k++)
                {
                        // Generate a random value between 0 and 100
                        float value = UnityEngine.Random.Range(0f, 100f);
                        liste_essai_j.Add(value);
                        sum += value;
                }
  
    
                while (Mathf.Abs((sum / nbCycles) - listeMoyennes[j]) > tolerance)
                {
                    float currentMean = sum / nbCycles;
                    float adjustment = listeMoyennes[j]  - currentMean;

                    for (int l = 0; l < nbCycles; l++)
                    {
                        liste_essai_j[l] += adjustment;
                    }

                    sum = liste_essai_j.Sum();
                }

                liste_bloc_i.Add(liste_essai_j);
 
             }
             listeGlobaleYoked.Add(liste_bloc_i);
        }
        
        return listeGlobaleYoked;
    }               

void ReadSujetSession()
{
    Debug.Log("reading sujet session");
    string filePath = Path.Combine(m_Path, "infos_sujet_session.txt");
    if (File.Exists(filePath))
    {
        string[] lines = File.ReadAllLines(filePath);
        numSujet = int.Parse(lines[0].Split(':')[1]);
        numSession = int.Parse(lines[1].Split(':')[1]);

        Console.WriteLine($"numSujet: {numSujet}, numSession: {numSession}");
    }
}

void ReadSequence()
    {        
        string filePath = Path.Combine(m_Path, "filenogotoread.txt");
        if (File.Exists(filePath))
        {
            string content = File.ReadAllText(filePath);
            if (int.TryParse(content, out int result))
            {
                num_sequence = result;
            }
        }
        
    }
    private Dictionary<int, string> CreateDictStim()
    {
        Dictionary<int, string> dict = new Dictionary<int, string>();
        dict[0] = "test Arduino";
        dict[13] = "OVTK_StimulationId_Number_0D";
        dict[16] = "OVTK_StimulationId_Number_10";
        dict[17] = "OVTK_StimulationId_Number_11";
        dict[18] = "OVTK_StimulationId_Number_12";
        dict[19] = "OVTK_StimulationId_Number_13";//essai type NF
        dict[20] = "OVTK_StimulationId_Number_14";//essai type OBS YOKED
        dict[260] = "OVTK_GDF_Artifact_Movement";//pause postTrial random (instruction bouger)
        dict[800] = "OVTK_GDF_End_Of_Trial";// fin d'un essai
        dict[1010] = "OVTK_GDF_End_Of_Session";//arret du scenario
        dict[32769] = "OVTK_StimulationId_ExperimentStart";//ouvrir le .exe (trigger par OV)	
        dict[32770] = "OVTK_StimulationId_ExperimentStop";
        dict[32771] = "OVTK_StimulationId_SegmentStart";//debut FB cycle
        dict[32775] = "OVTK_StimulationId_BaselineStart";//debut de baseline (different affichage croix)
        dict[32779] = "OVTK_StimulationId_VisualStimulationStart";//affichage de la croix
        dict[32780] = "OVTK_StimulationId_VisualStimulationStop";//arret affichage
        //dict[32773] = "OVTK_StimulationId_TrialStart";//debut d'un essai
        dict[33034] = "OVTK_StimulationId_Label_0A"; // affichage instruction obs essai type YOKED
        dict[33035] = "OVTK_StimulationId_Label_0B";
        dict[33036] = "OVTK_StimulationId_Label_0C";// affichage imaginer essai type NF
        dict[33037] = "OVTK_StimulationId_Label_0D";
        dict[33051] = "OVTK_StimulationId_Label_1B";//affichage de la main
        dict[33054] = "OVTK_StimulationId_Label_1E";//remplir questionnaire
        dict[33055] = "OVTK_StimulationId_Label_1F";// donner FB performance post bloc
        
        return dict;
    }


}
