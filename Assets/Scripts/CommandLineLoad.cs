using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommandLineLoad : MonoBehaviour
{
    private string errorText;
    private void Start()
    {
        if (CommandLineLoadData.hasLoaded)
            return;

        CommandLineLoadData.hasLoaded = true;

        string[] args = System.Environment.GetCommandLineArgs();
        string sceneName = "";
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-startScene")
            {
                sceneName = args[i + 1];
            }
        }

        if (string.IsNullOrEmpty(sceneName) == false)
        {
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                showErrorDialog = false;
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                errorText = "The scene \"" + sceneName + "\" does not exist.\n\nValid scene names:\n\tRockCrush\n\tBuggyBuddy\n\tSpace\n\tThrowing\n\tThrowingPlus";
                Debug.Log("errorText");
                showErrorDialog = true;
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // 200x300 px window will apear in the center of the screen.
    private Rect windowRect = new Rect((Screen.width - 350) / 2, (Screen.height - 200) / 2, 350, 200);
    
    private bool showErrorDialog = false;

    private void OnGUI()
    {
        if (showErrorDialog)
        {
            windowRect = GUI.Window(0, windowRect, DialogWindow, "ERROR: Loading scene from command line");
        }
    }

    // This is the actual window.
    private void DialogWindow(int windowID)
    {
        float titleY = 20;
        float labelY = 150;
        float buttonY = titleY + labelY;
        GUI.Label(new Rect(5, titleY, windowRect.width, labelY), errorText);

        float halfWidth = (windowRect.width - 40) / 2;
        if (GUI.Button(new Rect(10, buttonY, halfWidth, 20), "Ok"))
        {
            showErrorDialog = false;
        }

        if (GUI.Button(new Rect(30 + halfWidth, buttonY, halfWidth, 20), "Exit"))
        {
            showErrorDialog = false;
            Application.Quit();
        }
    }
}

public static class CommandLineLoadData
{
    public static bool hasLoaded = false;
}