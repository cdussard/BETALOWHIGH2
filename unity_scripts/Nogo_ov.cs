using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Assets.LSL4Unity.Scripts.AbstractInlets;
using Assets.LSL4Unity.Scripts;
using Assets.LSL4Unity.Scripts.Examples;
using System.IO;
using System.Globalization;
using UnityEngine.UI;
using System.Text;
using System.Net.Sockets;
using TMPro;


public class Nogo_ov : AFloatInlet
{
    public TcpClient tcpClient;
    public NetworkStream stream;

    private const string serverAddress = "localhost";//"127.0.0.1"; // Replace with your server IP
    private const int port = 15361;

    // gestion paths//
    public string m_Path;
    string path_editor = @"..\..\..\..\csv_files\";
    string path_build = @"..\..\..\csv_files\";

    float[] lastSample; 
    public float currentBetaValue = -1.0f;
    public float currentParasiteBetaValue = -1.0f;
    int currentStim = 0;
    public DateTime timeStart;

    //public SART script;
    public SART_TCP script;
    public GameObject controller;

    public bool openVibeStarted = false;
    public int runEnCours = 0;

    public StreamWriter writer_stim;
    // gestion du neurofeedback
    //seuils 

    void Awake()
    {
        Debug.Log("awake nogo");
        ConnectToServer();
        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";

        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        Debug.Log(m_Path);
        definePath();
        Debug.Log(m_Path);
        writer_stim = new StreamWriter(m_Path + "save_stim_sujet.csv");//fix le path


    }


    /// <param name="newSample"></param>
    /// <param name="timeStamp"></param>
    protected override void Process(float[] newSample, double timeStamp)
    {
        Debug.Log(newSample.Count());
        //Debug.Log("process");
        lastSample = newSample;
        currentBetaValue = lastSample[0];

        int indiceStim = 2;
        if (lastSample[1] != 0)
        {
            int stim = (int)lastSample[indiceStim];
            
            if ((lastSample[indiceStim] - stim) == 0)
            {
                Debug.Log("marker" + stim.ToString());
                try
                {
                    if (stim == 33024)//Label_00 
                    {
                        runEnCours ++;
                        Debug.Log("WRITING DEBUT");
                        SendStimulation(0, 0, 0);
                        timeStart = DateTime.Now;
                        saveStims(true, null, null);
                        openVibeStarted = true;


                    }

                    if (stim == 33054)
                    {
                        openVibeStarted = false;
                    }

                }


                catch (KeyNotFoundException e)
                {
                    // Handle the exception.
                    Debug.Log(e.Message);
                }

            }
            else if ((lastSample[1] - stim) != 0)
            {
                Debug.Log("decimal");
                Debug.Log("BUG!!!");
            }




        }


    }


    // Update is called once per frame
    void Update()
    {



    }


    private void definePath()
    {
        Debug.Log("defining");
        m_Path = Application.dataPath;
        Debug.Log(m_Path);
#if UNITY_EDITOR
        {
            Debug.Log("editor");

            m_Path += path_editor;
        }
#else
        {
            m_Path += path_build;
        }
#endif
    }

    public void saveStims(bool FirstLine, List<double> ls_diffTemps, List<string> ls_stim)//RESTE A LES APPEND (STIM ET APPUIS CLAVIER)
    {
        var line = "";
        if (FirstLine)
        {
            line = "";
            Debug.Log("writing title :");
            line = string.Format("{0},{1},{2}", "diffTemps", "duration", "stim");
            writer_stim.WriteLine(line);
            writer_stim.Flush();

        }
        else
        {
            Debug.Log(ls_diffTemps.Count().ToString());

            for (int i = 0; i < ls_diffTemps.Count(); i++)
            {
                Debug.Log(i);
                line = string.Format("{0},{1},{2}",
                ls_diffTemps[i].ToString(),
                0.ToString(), //duration tj 0 / a voir si on fait evoluer pour que lettres = duree de affichage
                ls_stim[i].ToString()
                );
                writer_stim.WriteLine(line);
                writer_stim.Flush();
            }
        }
    }




    void ConnectToServer()
    {
        try
        {
            tcpClient = new TcpClient(serverAddress, port);
            stream = tcpClient.GetStream();
            Debug.Log("Connected to TCP server.");
        }
        catch (Exception e)
        {
            Debug.LogError($"TCP connection error: {e.Message}");
        }
    }

    
    public void SendStimulation(uint flags, uint stimulation_id, uint timestamp)
    {
        tcpClient.NoDelay = true;
        if (stream == null) return;

        byte[] message = new byte[24];
        /*Buffer.BlockCopy(BitConverter.GetBytes(flags), 0, message, 0, 8);
        Buffer.BlockCopy(BitConverter.GetBytes(stimulation_id), 0, message, 8, 8);
        Buffer.BlockCopy(BitConverter.GetBytes(timestamp), 0, message, 16, 8);*/
        message[8] = (byte)stimulation_id;  
        stream.Write(message, 0, message.Length);
        Debug.Log($"Sent stimulation: flags={flags}, stim_id={stimulation_id}, timestamp={timestamp}");
    }
}
