using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour //Daan
{
    static public AudioManager instance;

   [Header("Audio files")]
    public AudioClip[] audioClips;
    public AudioClip[] backgroundMusic;

    [Header("Audio Volume Mixers")]
    public AudioMixer musicMixer;
    public AudioMixer sfxMixer;

    [Header("Audio Source's")]
    public AudioSource soundEffectPlayer;
    public AudioSource backgroundMusicPlayer;

    [Header("Settings")]
    public bool playMusicOnStart = false;
    public bool randomMusic = false;
    public bool loopMusic = false;
    public bool actionMusic = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        if (playMusicOnStart == true && actionMusic == false)
        {
            PlayRandomBackgroundMusic(0,1);
        }else if(playMusicOnStart == true && actionMusic == true)
        {
            PlayRandomBackgroundMusic(2, 3);
        }

        SetMusicVolume(PlayerPrefs.GetFloat("musicVolume"));
        SetSoundEffectVolume(PlayerPrefs.GetFloat("soundEffectVolume"));
    }

    private void Update()
    {
        if(loopMusic == false && backgroundMusicPlayer.isPlaying == false)
        {
            if (actionMusic == false)
                PlayRandomBackgroundMusic(0, 1);
            else
                PlayRandomBackgroundMusic(2, 3);
        }
    }

    /// <summary>
    /// changes the MusicVolume mixer its value to the slider value
    /// </summary>
    /// <param name="sliderValue">the value of the volume slider</param>
    public void SetMusicVolume(float sliderValue)
    {
        musicMixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
    }

    /// <summary>
    /// changes the SFXVolume mixer its value to the slider value
    /// </summary>
    /// <param name="sliderValue"></param>
    public void SetSoundEffectVolume(float sliderValue)
    {
        sfxMixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
    }

    /// <summary>
    /// plays a sound ones
    /// </summary>
    /// <param name="number">the number of the audioclip in the audioclips array</param>
    public void PlaySoundEffect(int number)
    {
        if (soundEffectPlayer.isPlaying == false)
        {
            soundEffectPlayer.PlayOneShot(audioClips[number]);
        }
    }

    /// <summary>
    /// play a requested background music
    /// </summary>
    /// <param name="number">the number the song has in the array</param>
    public void PlayBackgroundMusic(int number)
    {
        randomMusic = false;

        backgroundMusicPlayer.Stop(); //stop playing the song if one is playing
        backgroundMusicPlayer.clip = null; // to make sure the clip = empty
        backgroundMusicPlayer.loop = loopMusic; // to make sure its on loop;
        backgroundMusicPlayer.clip = backgroundMusic[number]; //add the wanted music in clip
        backgroundMusicPlayer.Play(); //play
    }

    /// <summary>
    /// plays a random background song
    /// </summary>
    /// <param name="minArrayNumber"></param>
    /// <param name="maxArrayNumber"></param>
    public void PlayRandomBackgroundMusic(int minArrayNumber, int maxArrayNumber)
    {
        if (randomMusic == true)
        {
            int random = Random.Range(minArrayNumber, maxArrayNumber + 1);

            backgroundMusicPlayer.loop = loopMusic; // to make sure its on loop;
            backgroundMusicPlayer.clip = null; // to make sure the clip = empty
            backgroundMusicPlayer.clip = backgroundMusic[random]; //add the wanted music in clip
            backgroundMusicPlayer.Play(); //play
        }
    }

    /// <summary>
    /// plays the requested audio on the given audio source
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="number"></param>
    public void PlaySound(AudioSource audioSource, int number)
    {
        audioSource.PlayOneShot(audioClips[number]);
    }
}
