using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Globalization;
using UnityEngine.Serialization;
using System.Net.Sockets;
public class SART_TCP : MonoBehaviour
{

    //TCP tagging
    public TcpClient tcpClient;
    public NetworkStream stream;

    // ecriture des stim
    public bool openVibeStarted;
    public Nogo_ov script;
    public GameObject controller;
    public List<double> ls_diffTemps = new List<double>();
    public List<string> ls_stim = new List<string>();

    public DateTime timeStart;

    // 

    public TMPro.TextMeshProUGUI displayNumber;
    public TMPro.TextMeshProUGUI displayConsignes;
    private string charToDisplay;
    public int nbEssais = -1;
    public int nbRuns = -1;
    public int numEssai = 0;
    public int numRun = 0;
    public int moyenneISI = 1;
    private System.Random rand;
    private float duration = 1f;
    private float displayDuration = 0.2f;
    private float displayTime = 0f;
    public List<LogEntry> logEntries = new List<LogEntry>();
    public List<coupleDisplay> coupleDisplays = new List<coupleDisplay>();
    private bool endTask = true;
    public bool spacebarPressed = false;
    public bool askedToDisplay = false;
    public float askedToDisplayTime = -1f;
    private int num_sujet;
    private int num_session;
    public bool isNoGo;
    public string noGoCharacter;
    public List<string> listeConsignes;


    public string m_Path;
    //string path = @"..\..\csv_files\";//@".\..\..\csv_files\";//trois si build deux sinon //@"..\..\csv_files\"
    int noGoFileToRead;
    int numSujet;
    int numSession;
    StreamWriter writer;
    public StreamWriter writer_stim;

    private void Awake()
    {

        Debug.Log("awake sart");
        m_Path = script.m_Path;
        Debug.Log(m_Path);
        string filePath = m_Path + "save_log_sujet.csv";
        Debug.Log(filePath);
        // Vérifier si le fichier existe, ouvrir en append s'il existe, sinon le créer
        if (File.Exists(filePath))
        {
            writer = new StreamWriter(filePath, true); // Append mode
            Debug.Log("Appending to existing file.");
        }
        else
        {
            writer = new StreamWriter(filePath, false); // Création du fichier
            Debug.Log("Creating new file.");
        }

        writer_stim = script.writer_stim;


        // load ISIs, characters
        ReadSujetSession();
        readCharacterList();

        assignConsignes();
        rand = new System.Random();
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        Debug.Log("start sart");
        tcpClient = script.tcpClient;
        stream = script.stream;

        Debug.Log(openVibeStarted);
        //ConnectToServer();

        yield return StartCoroutine(DoWholeSart());


    }


    IEnumerator DoWholeSart()
    {
        // Wait until openVibeStarted is true
        while (!openVibeStarted)
        {
            openVibeStarted = script.openVibeStarted;
            Debug.Log("Waiting for openVibe to start...");
            yield return null; // Wait for the next frame and check again
        }

        Debug.Log("Launching task");

        timeStart = script.timeStart;
        ls_diffTemps.Add(0);
        ls_stim.Add("start");

        // Display instructions
        displayConsignes.text = listeConsignes[0];
        yield return new WaitForSeconds(4f);
        displayConsignes.text = listeConsignes[1];
        yield return new WaitForSeconds(4f);
        displayConsignes.text = listeConsignes[2];
        yield return new WaitForSeconds(3f);
        displayConsignes.text = listeConsignes[3];
        yield return new WaitForSeconds(3f);
        displayConsignes.text = "";

        // Run task
        for (int j = 0; j < nbRuns; j++)
        {
            numRun = j + 1;
            numEssai = 0;
            yield return StartCoroutine(SartTask(j + 1));
            Debug.Log("Run end.");
            writeDataLog();
            script.saveStims(false, ls_diffTemps, ls_stim);

            // Pause between runs
            displayConsignes.text = listeConsignes[4];
            endTask = true;
            yield return new WaitForSeconds(10f);
            displayConsignes.text = "";
        }

        displayConsignes.text = listeConsignes[5];
    }

    //https://gamedevbeginner.com/how-to-use-fixed-update-in-unity/#:~:text=The%20rate%20at%20which%20Fixed,of%2050%20frames%20per%20second. ==> modifie fixed timestep a 0.001
    void FixedUpdate()
    {
        if (!endTask)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !spacebarPressed)

            //GetKey() will return true in every frame that the key is held down 
            //while GetKeyDown() only returns true in the first frame that is is pressed, then false after that even if the key is still down

            // 
            {
                spacebarPressed = true;

                DateTime spaceBar = DateTime.Now;
                script.SendStimulation(0, 1, 0);//keyboard
                //Debug.Log(askedToDisplayTime);
                //Debug.Log("pressed");
                LogEntry entry = new LogEntry
                {
                    num_sujet = num_sujet,
                    num_session = num_session,
                    numEssai = numEssai,
                    numRun = numRun,
                    Symbol = charToDisplay,
                    displayTime = displayTime,
                    ISI = duration,
                    SpacebarPressTime = Time.time,
                    SpacebarRT = Time.time - displayTime,
                    askedToDisplayTime = askedToDisplayTime,
                    isNoGo = isNoGo,
                    isAffichage = false,
                    displayDuration = displayDuration

                };
                ls_diffTemps.Add((spaceBar - timeStart).TotalSeconds);
                ls_stim.Add("SPACEBAR");
                // add en entry to the log
                logEntries.Add(entry);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        openVibeStarted = script.openVibeStarted;
        if (!endTask)

        {
            if (spacebarPressed && Input.GetKeyUp(KeyCode.Space)) // Reset the spacebarPressed flag when spacebar is released
            {
                spacebarPressed = false;
            }
            if (askedToDisplay)
            {
                if (isNoGo)
                {
                    script.SendStimulation(0, 2, 0);// NO GO 
                }

                else
                {
                    script.SendStimulation(0, 3, 0);//GO
                }
                displayTime = Time.time;
                DateTime displayTime_ = DateTime.Now;

                LogEntry entry = new LogEntry
                {
                    num_sujet = num_sujet,
                    num_session = num_session,
                    Symbol = charToDisplay,
                    displayTime = displayTime,
                    SpacebarPressTime = -1,
                    ISI = duration,
                    numEssai = numEssai,
                    numRun = numRun,
                    isNoGo = isNoGo,
                    isAffichage = true,
                    displayDuration = displayDuration,
                    SpacebarRT = -1

                };
                ls_diffTemps.Add((displayTime_ - timeStart).TotalSeconds);
                if (isNoGo)
                {
                    ls_stim.Add("NoGo");
                }
                else
                {
                    ls_stim.Add("Go");
                }

                logEntries.Add(entry);
                askedToDisplay = false; // pour eviter plusieurs logs

            }

        }
        else
        {
            if (Input.GetKey("escape"))
            {
                Application.Quit();

            }

        }


    }

    void assignConsignes()
    {

        listeConsignes.Add("Une série de lettres va défiler rapidement à l'écran");
        listeConsignes.Add("Appuyez sur ESPACE dès que vous voyez une lettre. \nN'appuyez PAS si c'est la lettre " + noGoCharacter);
        listeConsignes.Add("Soyez le plus rapide ET précis possible \n (place sur le podium à la clé...)");
        listeConsignes.Add("Début dans 3s");
        listeConsignes.Add("10s de pause ... \n Rappel : n'appuyez pas à la lettre " + noGoCharacter);
        listeConsignes.Add("Bravo, c'est fini ! Appuyez sur ECHAP pour fermer la fenêtre :)");

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

    void readCharacterList()
    {
        string filePath = Path.Combine(m_Path, "filenogotoread.txt");
        if (File.Exists(filePath))
        {
            string content = File.ReadAllText(filePath);
            if (int.TryParse(content, out int result))
            {
                noGoFileToRead = result;
            }
        }
        Debug.Log("reading nogo sequence");
        // deduire nb essais et runs 

        string filePathSuj = Path.Combine(m_Path, $"../gonogo_sequences/sequence_{noGoFileToRead}_sujet_{numSujet}_session_{numSession}.csv");
        var reader = new StreamReader(filePathSuj);
        string headerLine = reader.ReadLine();
        string line;

        while ((line = reader.ReadLine()) != null)
        {
            var values = line.Split(',');
            if (values[0] != "num_sujet")
            {
                coupleDisplay disp = new coupleDisplay
                {
                    numRun = int.Parse(values[2], CultureInfo.InvariantCulture),
                    numTrial = int.Parse(values[3], CultureInfo.InvariantCulture),
                    Symbol_i = values[4],
                    isNoGo = bool.Parse(values[5]),
                    durationDisplay = float.Parse(values[6], CultureInfo.InvariantCulture),

                };
                if (bool.Parse(values[5]))
                {
                    noGoCharacter = values[4];

                }

                coupleDisplays.Add(disp);
            }
            num_sujet = int.Parse(values[0], CultureInfo.InvariantCulture);
            num_session = int.Parse(values[1], CultureInfo.InvariantCulture);
            nbEssais = int.Parse(values[3], CultureInfo.InvariantCulture); // le num du dernier trial est le nb max de trials
            nbRuns = int.Parse(values[2], CultureInfo.InvariantCulture);// le num du dernier run est le nb max de runs
        }

                // Increment the value
        noGoFileToRead++;

        // Write the updated value back to the file
        File.WriteAllText(filePath, noGoFileToRead.ToString());

        Console.WriteLine("Updated value: " + noGoFileToRead);

    }

    void writeDataLog()
    {
        Debug.Log("writing");
        var line = "";

        if (numRun == 1)
        {
            Debug.Log("writing title :)");
            line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", "symbol", "ISI", "SpacebarPressTime", "displayTime", "num_essai", "num_run", "isNoGo", "askedToDisplayTime", "isAffichage", "displayDuration", "SpacebarRT");
            writer.WriteLine(line);
            writer.Flush();
        }


        foreach (var entry in logEntries)
        {
            Debug.Log("writing entries");
            line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
            entry.Symbol.ToString(),
            entry.ISI.ToString("F6", CultureInfo.InvariantCulture),
            entry.SpacebarPressTime.ToString("F6", CultureInfo.InvariantCulture),
            entry.displayTime.ToString("F6", CultureInfo.InvariantCulture),
            entry.numEssai.ToString("F0", CultureInfo.InvariantCulture),
            entry.numRun.ToString("F0", CultureInfo.InvariantCulture),
            entry.isNoGo.ToString(),
            entry.askedToDisplayTime.ToString("F6", CultureInfo.InvariantCulture),
            entry.isAffichage.ToString(),
            entry.displayDuration.ToString("F6", CultureInfo.InvariantCulture),
            entry.SpacebarRT.ToString("F6", CultureInfo.InvariantCulture));
            writer.WriteLine(line);
            writer.Flush();

        }
        logEntries.Clear();
    }




    IEnumerator SartTask(int numRun)
    {
        endTask = false;
        for (int i = 0; i < nbEssais; i++)
        {
            numEssai = i + 1;
            coupleDisplay coupleDisplay_i = coupleDisplays[(numRun - 1) * nbEssais + numEssai - 1];
            duration = coupleDisplay_i.durationDisplay;
            charToDisplay = coupleDisplay_i.Symbol_i;
            isNoGo = coupleDisplay_i.isNoGo;
            //Debug.Log(charToDisplay);
            yield return StartCoroutine(DisplayTextForDuration(charToDisplay, duration));

        }

        yield return new WaitForSeconds(1f);
        endTask = true;
        Debug.Log("end sart");
    }


    IEnumerator DisplayTextForDuration(string charToDisplay, float duration)
    {
        displayNumber.text = charToDisplay;
        askedToDisplay = true;
        askedToDisplayTime = Time.time;

        yield return new WaitForSeconds(displayDuration);
        displayNumber.text = "";
        yield return new WaitForSeconds(duration - displayDuration);


    }

    [System.Serializable]
    public class coupleDisplay
    {
        public float durationDisplay;
        public string Symbol_i;
        public int numTrial;
        public int numRun;
        public bool isNoGo;

    }


    [System.Serializable]
    public class LogEntry
    {
        public int num_sujet;
        public int num_session;
        public string Symbol;
        public float ISI;
        public float SpacebarPressTime;
        public float SpacebarRT;
        public float displayTime;
        public float askedToDisplayTime;
        public int numEssai;
        public int numRun;
        public bool isNoGo;
        public bool isAffichage;
        public float displayDuration;

    }




    void OnApplicationQuit()
    {
        if (stream != null) stream.Close();
        if (tcpClient != null) tcpClient.Close();
        Debug.Log("Disconnected from TCP server.");
    }

}
