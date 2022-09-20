using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour //Daan
{
    private SpectatorCamera spectatorCamera;

    [SerializeField] private GameObject credtisScreen;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject settingsScreen;
    [Space]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI sensitivityText;
    private float sensitivity;
    [Space]
    [SerializeField] private Slider speedSlider;
    [SerializeField] private TextMeshProUGUI speedText;
    private float speed;
    [Space]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TextMeshProUGUI musicText;
    private float musicVolume;
    [Space]
    [SerializeField] private Slider soundEffectSlider;
    [SerializeField] private TextMeshProUGUI soundEffectText;
    private float soundEffectVolume;
    [Space]
    [SerializeField] private TextMeshProUGUI spawnTimerText;
    private SimpleBehaviorTree.Examples.UnitSpawner playerSpawner;

    private void Awake()
    {
        spectatorCamera = FindObjectOfType<SpectatorCamera>();

        if (PlayerPrefs.HasKey("sensitivity") == true)
        {
            sensitivity = PlayerPrefs.GetFloat("sensitivity");
            musicVolume = PlayerPrefs.GetFloat("musicVolume");
            soundEffectVolume = PlayerPrefs.GetFloat("soundEffectVolume");
            speed = PlayerPrefs.GetFloat("speed");

            sensitivitySlider.value = sensitivity;
            musicSlider.value = musicVolume;
            soundEffectSlider.value = soundEffectVolume;
            speedSlider.value = speed;
        }

        MusicSlider();
        SoundEffectSlider();
    }

    private void Start()
    {
        // refrence for the UI Timer
        if(RefrenceManager.instance != null)
        playerSpawner = RefrenceManager.instance.playerBase.GetComponent<SimpleBehaviorTree.Examples.UnitSpawner>();
    }

    private void Update()
    {
        Debug.Log(Time.timeScale);

        if (Input.GetKeyDown(KeyCode.Escape))
            Settings();

        if (Input.GetKeyDown(KeyCode.Backspace))
            ResetSliderValues();

        // Handle the UI timer
        if (spawnTimerText != null)
        {
            float spawnTime = playerSpawner.spawnTimer;
            float time = playerSpawner.time;
            float showTime = Mathf.RoundToInt((spawnTime - time) * 10);
            showTime /= 10;
            string showText = $"Next wave in {showTime}";
            spawnTimerText.text = showText;
        }
    }

    //loads the game scene and plays action music
    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
        AudioManager.instance.actionMusic = true;
        AudioManager.instance.PlayRandomBackgroundMusic(2, 3);
        Time.timeScale = 1;
    }

    //loads the mainmenu scene and plays non action music
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        AudioManager.instance.actionMusic = false;
        AudioManager.instance.PlayRandomBackgroundMusic(0, 1);
        Time.timeScale = 1;
    }

    /// <summary>
    /// closes the application
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// turn the credits screen ON if its OFF and ON when its OFF
    /// </summary>
    public void Credits()
    {
        if (credtisScreen.active == true)
        {
            credtisScreen.SetActive(false);
            startScreen.SetActive(true);
        }
        else if (credtisScreen.active == false)
        {
            credtisScreen.SetActive(true);
            startScreen.SetActive(false);
        }
    }

    /// <summary>
    /// turn the settings screen ON if its OFF and ON when its OFF
    /// </summary>
    public void Settings()
    {
        if (settingsScreen.active == true)
        {
            Time.timeScale = 1; //pauzes the game

            if (startScreen != null)
                startScreen.SetActive(true);

            settingsScreen.SetActive(false);

            AudioManager.instance.PlaySoundEffect(0); //plays mouse click sound effect
        }
        else if (settingsScreen.active == false)
        {
            Time.timeScale = 0; //pauzes the game

            if (startScreen != null)
                startScreen.SetActive(false);

            settingsScreen.SetActive(true);

            AudioManager.instance.PlaySoundEffect(0); //plays mouse click sound effect
        }
    }


    /// <summary>
    /// chanches the mouse sensitivity to the slider value
    /// </summary>
    public void SensitivitySlider()
    {
        sensitivity = sensitivitySlider.value;
        sensitivityText.text = "Sensitivity: " + sensitivity.ToString("0.0");
        PlayerPrefs.SetFloat("sensitivity", sensitivity);

        if (spectatorCamera != null)
            spectatorCamera.SettingsUpdate();
    }

    /// <summary>
    /// chanches the camera movement speed to the slider value
    /// </summary>
    public void SpeedSlider()
    {
        speed = speedSlider.value;
        speedText.text = "Speed: " + speed.ToString("0.0");
        PlayerPrefs.SetFloat("speed", speed);

        if (spectatorCamera != null)
            spectatorCamera.SettingsUpdate();
    }

    /// <summary>
    /// chanches the background music volume to the slider value
    /// </summary>
    public void MusicSlider()
    {
        musicVolume = musicSlider.value;
        musicText.text = "Music volume:  " + (musicVolume * 100).ToString("0") + "%";
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        try
        {
        AudioManager.instance.SetMusicVolume(musicVolume);
        }
        catch { }
    }

    /// <summary>
    /// chanches the sound effect volume to the slider value
    /// </summary>
    public void SoundEffectSlider()
    {
        soundEffectVolume = soundEffectSlider.value;
        soundEffectText.text = "Sound effect volume: " + (soundEffectVolume * 100).ToString("0") + "%";
        PlayerPrefs.SetFloat("soundEffectVolume", soundEffectVolume);
        try
        {
            AudioManager.instance.SetSoundEffectVolume(soundEffectVolume);
        }
        catch { }

    }

    /// <summary>
    /// resets all setting and playerprefs to there default values
    /// </summary>
    public void ResetSliderValues()
    {
        soundEffectVolume = soundEffectSlider.maxValue / 2;
        musicVolume = musicSlider.maxValue / 2;
        sensitivity = sensitivitySlider.minValue;
        speed = speedSlider.maxValue / 2;

        PlayerPrefs.SetFloat("soundEffectVolume", soundEffectVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("sensitivity", sensitivity);
        PlayerPrefs.SetFloat("speed", speed);

        soundEffectSlider.value = soundEffectVolume;
        musicSlider.value = musicVolume;
        sensitivitySlider.value = sensitivity;
        speedSlider.value = speed;

        Debug.Log("slider values have been reset");
    }
}
