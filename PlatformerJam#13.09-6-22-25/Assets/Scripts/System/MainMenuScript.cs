using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    public string startMenuString =
    "    __  ____           _                  _  __ _                  _  _____  ___\n" +
    "   /  |/  (_)_________(_)___  ____  _    | |/ /(_)___  ____       | |/ /__ \\<  /\n" +
    "  / /|_/ / / ___/ ___/ / __ \\/ __ \\(_)   |   // / __ \\/ __ \\______|   /__/ // / \n" +
    " / /  / / (__  |__  ) / /_/ / / / /     /   |/ / /_/ / / / /_____/   |/ __// /  \n" +
    "/_/  /_/_/____/____/_/\\____/_/ /_(_)   /_/|_/_/\\____/_/ /_/     /_/|_/____/_/   \n" +
    "                                                                    \n" +
    "Main menu:\n" +
    "Choose:\n" +
    "(1) Start\n" +
    "(2) Settings\n" +
    "(3) Info\n" +
    "(4) Exit";
    public string settingsString = "SETTINGS: XION-X21\r\n\r\nvolume:\r\n\r\nOptions:\r\n(1) Change value\r\n(2) Return to main menu";
    public string infoString = "GAME - MISSION: XION-X21\r\nDevelopers - Astragor, Chipy, Nathan, zombielovejuice64, BlackPossum, cc.cretina\r\nMade for: Platformer Game Jam (72-Hour Challenge)\r\nEngine: Unity\r\nGenre: Horror Platformer with Reverse Perspective\r\nEstimated Playtime: 15-20 minutes\r\n\r\n/// STATION LOG ENTRY #X21-477 ///\r\nAbout the game: The deep space research station Xion-X21 went dark 73 days ago. \r\nWhen the rescue team arrives, they find the facility in ruins - \r\ncorridors breached to vacuum, lab equipment smashed, and... \r\nsomething moving in the shadows. \r\n\r\nYou'll experience: - Tense 2D platforming through derelict station sectors\r\n- Unusual reverse-horror gameplay mechanics\r\n- A chilling original soundtrack\r\n- Multiple escape routes with different challenges\r\n- The truth about what happened to Station Xion-X21\r\n\r\n/// CREATURE SPECIMEN REPORT ///\r\nTheme implementation: Forget everything you know about survival horror. \r\nYou won't be playing as a human researcher. \r\nThe roles have been violently reversed. \r\nNow YOU are the perfect organism - \r\na terrifying fusion of human DNA and cosmic mutation. \r\nHunt. Evolve. Survive.\r\n\r\n/// FINAL TRANSMISSION ///\r\nDev note: We poured our love for classic sci-fi horror into this project. \r\nThe station's decaying atmosphere, the creature's unnatural movements, \r\nthe desperate escape attempts - it's all designed to unsettle you. \r\nHave fun, and try not to scream too loud when they start running...\r\n\r\nOptions:\r\n(1) Go back to main menu\r\n(2) View controls";
    public string controlsString = "//////////////////////////////////////////\r\n  CONTROLS - mission XION-X21\r\n//////////////////////////////////////////\r\n\r\n[ MOVEMENT ]\r\n► WASD/Arrow Keys  - Navigate the station\r\n► SPACE            - Leap/Special ability\r\n► SHIFT            - Sprint (consumes energy)\r\n\r\n[ HUNTING MODE ]\r\n► MOUSE AIM        - Direct your appendages\r\n► LEFT CLICK       - Primary attack\r\n► RIGHT CLICK      - Special mutation\r\n► R                - Toggle camouflage\r\n\r\n[ ENVIRONMENT ]\r\n► E                - Interact/Open doors\r\n► Q                - Environmental scan\r\n► TAB              - View station map\r\n\r\n[ SYSTEM ]\r\n► ESC              - Pause/Station menu\r\n► F                - Toggle flashlight\r\n► M                - Mute audio logs\r\n\r\n⚠ WARNING:\r\nSome control functions may evolve\r\nas the organism adapts to its host.\r\n\r\nOptions:\r\n (1) Return to Main Menu \r\n (2) return to info ";
    public TextMeshProUGUI mainText;
    public TMP_InputField inputField;

    private bool ExpectsInput = false;
    private bool ConfirmQuit = false;

    public GameMenuState state;
    private GameManager gmanager;

    private void Start()
    {
        gmanager = gameObject.GetComponent<GameManager>();
        GoToMain();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter)) {
            ExecuteCommand();
        }
    }

    public void ExecuteCommand()
    {
        string command = inputField.text;
        if(state == GameMenuState.Main) {
            if (ConfirmQuit == false)
            {
                switch (command)
                {
                    case ("1"):
                        Debug.Log("StartGame");
                        gmanager.LoadScene("Game");
                        break;
                    case ("2"):
                        Debug.Log("Options");
                        GoToSettings();
                        break;
                    case ("3"):
                        Debug.Log("Info");
                        GoToInfo();
                        break;
                    case ("4"):
                        Debug.Log("Exiting game");
                        mainText.text += "\r\n Confirm exiting: \r\n Options: \r\n (Y) Confirm \r\n (N) No";
                        ConfirmQuit = true;
                        break;
                    default:
                        mainText.text += "\r\n ERROR - answer not found";
                        break;
                }
            }
            else
            {
                switch (command)
                {
                    case ("Y"):
                        gmanager.CloseGame();
                        GoToMain();
                        break;
                    case ("y"):
                        gmanager.CloseGame();
                        GoToMain();
                        break;
                    default:
                        GoToMain();
                        mainText.text += "\r\n Exit was denied";
                        break;
                }
                ConfirmQuit = false;
            }
        }
        else if (state == GameMenuState.Info)
        {
            switch (command)
            {
                case ("1"):
                    Debug.Log("MainMenu");
                    GoToMain();
                    break;
                case ("2"):
                    Debug.Log("Controls");
                    GoToControls();
                    break;
                default:
                    mainText.text += "\r\n ERROR - answer not found";
                    break;
            }
        }
        else if (state == GameMenuState.Controls)
        {
            switch (command)
            {
                case ("1"):
                    Debug.Log("MainMenu");
                    GoToMain();
                    break;
                case ("2"):
                    Debug.Log("Info");
                    GoToInfo();
                    break;
                default:
                    mainText.text += "\r\n ERROR - answer not found";
                    break;
            }
        }
        else if (state == GameMenuState.Settings)
        {
            if (ExpectsInput == false)
            {
                switch (command)
                {
                    case ("1"):
                        Debug.Log("Change Volume");
                        mainText.text += "\r\n Enter Volume :";
                        ExpectsInput = true;
                        break;
                    case ("2"):
                        Debug.Log("MainMenu");
                        GoToMain();
                        break;
                    default:
                        mainText.text += "\r\n ERROR - answer not found";
                        break;
                }
            }
            else
            {
                //CHANGE VOLUME
                ExpectsInput = false;
                GoToSettings();
            }
        }
    }
    public void GoToSettings()
    {
        inputField.text = "";
        state = GameMenuState.Settings;
        mainText.text = settingsString;
    }
    public void GoToInfo()
    {
        inputField.text = "";
        state = GameMenuState.Info;
        mainText.text = infoString;

    }
    public void GoToMain()
    {
        inputField.text = "";
        state = GameMenuState.Main;
        mainText.text = startMenuString;
    }
    public void GoToControls()
    {
        inputField.text = "";
        state = GameMenuState.Controls;
        mainText.text = controlsString;
    }
}

public enum GameMenuState
{
    Main,
    Settings,
    Info,
    Controls
}