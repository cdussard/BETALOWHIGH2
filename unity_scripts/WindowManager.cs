using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_MINIMIZE = 6;
    private const int SW_MAXIMIZE = 3;
    private const int SW_RESTORE = 9;


    public void MinimizeApp(string processName)
    {
        Process[] processes = Process.GetProcessesByName(processName);
        if (processes.Length > 0)
        {
            ShowWindow(processes[0].MainWindowHandle, SW_MINIMIZE);
            UnityEngine.Debug.Log($"{processName} minimized.");
        }
        else
        {
            UnityEngine.Debug.LogWarning($"No process found with name: {processName}");
        }
    }

    private float switchTime = 1f; // Switch every 5 seconds
    private float timer = 0f;
    private bool isOtherAppActive = false;

   /* private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= switchTime)
        {
            timer = 0f;
            if (isOtherAppActive)
            {
                MinimizeApp(otherAppProcessName);
                MaximizeApp(Process.GetCurrentProcess().ProcessName); // Maximize THIS Unity app
            }
            else
            {
                MaximizeApp(otherAppProcessName);
                MinimizeApp(Process.GetCurrentProcess().ProcessName); // Minimize THIS Unity app
            }

            isOtherAppActive = !isOtherAppActive;
        }
    }*/


    public void MaximizeApp(string processName)
    {
        Process[] processes = Process.GetProcessesByName(processName);
        if (processes.Length > 0)
        {
            ShowWindow(processes[0].MainWindowHandle, SW_MAXIMIZE);
            ShowWindow(processes[0].MainWindowHandle, SW_RESTORE); // Restore if needed
            UnityEngine.Debug.Log($"{processName} maximized.");
        }
        else
        {
            UnityEngine.Debug.LogWarning($"No process found with name: {processName}");
        }
    }
}
