using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip[] audioClips;
    public AudioSource audioSource, bgAudioSource;
    bool soundToggle = true;
    public Sprite audioOn, audioOff;
    public Image audioBtnImage;

    private void Awake()
    {
        print("Test");
        audioBtnImage = GameObject.Find("AudioBtn").GetComponent<Image>();

        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);

        if (PlayerPrefs.HasKey("Sound"))
            SoundToggle();
    }

    public void Play(string name)
    {
        if(audioBtnImage == null)
            audioBtnImage = GameObject.Find("AudioBtn").GetComponent<Image>();

        AudioClip clip = Array.Find(audioClips, sound => sound.name == name);

        if (audioSource.clip != clip)
            audioSource.clip = clip;

        audioSource.Play();
    }

    public void SoundToggle()
    {
        soundToggle = !soundToggle;
        if (soundToggle)
        {
            audioBtnImage.sprite = audioOn;
            //audioSource.mute = false;
            bgAudioSource.mute = false;
            PlayerPrefs.DeleteKey("Sound");
        }
        else
        {
            audioBtnImage.sprite = audioOff;
            //audioSource.mute = true;
            bgAudioSource.mute = true;
            print("Mute");

            PlayerPrefs.SetString("Sound", "");
        }
    }
}
