﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Control settings in menu
public class MenuSettingsManager : MonoBehaviour
{
    // List of menu states
    public enum MenuState
    {
        Menu,
        Settings,
        Credits
    };

    //--- Images ---//

    // Start panel
    private Image _startPanel;
    // Menu panel
    private Image _menuPanel;
    // Settings panel
    private Image _settingsPanel;
    // Credits panel
    private Image _creditsPanel;
    // Warning panel
    private Image _warningPanel;

    //--- Texts ---//

    // Sounds label
    private Text _soundsLabel;
    // Music label
    private Text _musicLabel;

    //--- Different ---//

    // Sounds slider
    public Slider SoundsSld { get; set; }
    // Music slider
    public Slider MusicSld { get; set; }
    // Menu sounds source
    public AudioSource MenuSoundsSrc { get; set; }
    // Menu music source
    public AudioSource MenuMusicSrc { get; set; }
    // Menu key
    private KeyCode _menuKey = KeyCode.Escape;
    // Current menu state
    private MenuState _curMenuState;
    // Click sound
    private AudioClip _click;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        Init();
        SetEventListeners();
        SetMenuConfiguration();
    }

    // Update is called once per frame
    private void Update()
    {
        SwitchMainMenu();
    }

    // Initializate parameters
    private void Init()
    {
        // Images
        _startPanel = GameObject.Find("StartPanel").GetComponent<Image>();
        _menuPanel = GameObject.Find("MenuPanel").GetComponent<Image>();
        _settingsPanel = GameObject.Find("SettingsPanel").GetComponent<Image>();
        _creditsPanel = GameObject.Find("CreditsPanel").GetComponent<Image>();
        _warningPanel = GameObject.Find("WarningPanel").GetComponent<Image>();
        // Texts
        _soundsLabel = GameObject.Find("SoundsVolumeLabel").GetComponent<Text>();
        _musicLabel = GameObject.Find("MusicVolumeLabel").GetComponent<Text>();
        // Sliders
        SoundsSld = GameObject.Find("SoundsVolumeSlider").GetComponent<Slider>();
        MusicSld = GameObject.Find("MusicVolumeSlider").GetComponent<Slider>();
        // Audio sources
        MenuSoundsSrc = GameObject.Find("SoundsSource").GetComponent<AudioSource>();
        MenuMusicSrc = GameObject.Find("MusicSource").GetComponent<AudioSource>();
        // Hide selected panels
        _startPanel.gameObject.SetActive(false);
        _settingsPanel.gameObject.SetActive(false);
        _creditsPanel.gameObject.SetActive(false);
        _warningPanel.gameObject.SetActive(false);
        // Load click sound
        _click = Resources.Load<AudioClip>("Sounds/Click");
        // Set current state
        _curMenuState = MenuState.Menu;
    }

    // Set proper configuration before start program
    private void SetMenuConfiguration()
    {
        // Set reference to this script (needed to apply changes)
        MenuSettingsManager settingsManager = this;
        // Try load data
        SettingsDatabase.LoadResult result = SettingsDatabase.TryLoadConfig(Application.persistentDataPath,
            SettingsDatabase.MenuConfigName, SettingsDatabase.ConfigType.Menu);
        // There are no configuration file
        if (result.Equals(SettingsDatabase.LoadResult.NoFile))
            // Set default parameters from database
            SettingsDatabase.SetDefaultMenuSettings();
        // Configuration file is damaged
        else if (result.Equals(SettingsDatabase.LoadResult.Error))
        {
            // Set default parameters from database
            SettingsDatabase.SetDefaultMenuSettings();
            // Display warning window
            _warningPanel.gameObject.SetActive(true);
        }

        // Configuration load correctly!

        // Set parameters from configuration file (default or saved)
        SettingsDatabase.SetMenuFromConfig(ref settingsManager);
        // Set proper labels for sliders
        AdjustSoundsVolume();
        AdjustMusicVolume();
    }

    // Switch panels in main menu
    public void SwitchMainMenu()
    {
        // Check if key is pressed
        if (!Input.GetKeyDown(_menuKey))
            // Break action
            return;
        // Switch state
        switch (_curMenuState)
        {
            case MenuState.Menu:
                break;
            case MenuState.Settings:
                ReturnToMainMenu();
                break;
            case MenuState.Credits:
                ReturnToMainMenu();
                break;
        }
    }

    // Play click sound after press button
    public void PlayClickSound()
    {
        // Play click sound
        MenuSoundsSrc.PlayOneShot(_click);
    }

    // Set proper event listeners to sliders
    private void SetEventListeners()
    {
        // Add event listeners
        SoundsSld.onValueChanged.AddListener(delegate { AdjustSoundsVolume(); });
        MusicSld.onValueChanged.AddListener(delegate { AdjustMusicVolume(); });
    }

    // Adjust sounds volume via slider
    public void AdjustSoundsVolume()
    {
        // Prepare volume label
        int soundsValue = (int)Mathf.Round(SoundsSld.value * 100f);
        // Set proper label
        _soundsLabel.text = soundsValue + "%";
        // Change sounds volume
        MenuSoundsSrc.volume = SoundsSld.value;
    }

    // Adjust music volume via slider
    public void AdjustMusicVolume()
    {
        // Prepare volume label
        int musicValue = (int)Mathf.Round(MusicSld.value * 100f);
        // Set proper label
        _musicLabel.text = musicValue + "%";
        // Change music volume
        MenuMusicSrc.volume = MusicSld.value;
    }

    // Show settings in main menu
    public void ShowSettings()
    {
        // Activate settings panel
        _settingsPanel.gameObject.SetActive(true);
        // Hide main menu
        _menuPanel.gameObject.SetActive(false);
        // Change current state
        _curMenuState = MenuState.Settings;
    }

    // Show credits in main menu
    public void ShowCredits()
    {
        // Activate credits panel
        _creditsPanel.gameObject.SetActive(true);
        // Hide main menu
        _menuPanel.gameObject.SetActive(false);
        // Change current state
        _curMenuState = MenuState.Credits;
    }

    // Return to main menu from another window
    public void ReturnToMainMenu()
    {
        // Activate menu panel
        _menuPanel.gameObject.SetActive(true);
        // Hide other panels
        _settingsPanel.gameObject.SetActive(false);
        _creditsPanel.gameObject.SetActive(false);
        // Change current state
        _curMenuState = MenuState.Menu;
    }

    // Hide main menu warning about damaged file
    public void HideMenuWarning()
    {
        // Hide warning panel
        _warningPanel.gameObject.SetActive(false);
    }

    // Start simulation, display new scene and close main menu
    public void StartSimulation()
    {
        // Copy variables to configuration structure
        SettingsDatabase.CopyMenuToConfig(this);
        // Save configuration
        SettingsDatabase.TrySaveConfig(Application.persistentDataPath,
            SettingsDatabase.MenuConfigName, SettingsDatabase.ConfigType.Menu);
        // Hide main menu
        _menuPanel.gameObject.SetActive(false);
        // Show loading panel
        _startPanel.gameObject.SetActive(true);
        // Display simulation scene
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    // Save menu configuration and exit program
    public void ExitProgram()
    {
        // Copy variables to configuration structure
        SettingsDatabase.CopyMenuToConfig(this);
        // Save configuration
        SettingsDatabase.TrySaveConfig(Application.persistentDataPath,
            SettingsDatabase.MenuConfigName, SettingsDatabase.ConfigType.Menu);
        // Quit application
        Application.Quit();
    }
}