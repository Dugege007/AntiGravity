using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Mixer")]
    public AudioMixer mixer;

    [Header("Audio Clips BGM")]
    public AudioClip MenuBgmClip;
    public AudioClip startBgmClip;
    public AudioClip loopBgmClip;
    public AudioClip finalBgmClip;

    public AudioClip gameOverBgmClip;

    [Header("Audio Clips FX")]
    public AudioClip moveClip;
    public AudioClip getBuffClip;
    public List<AudioClip> getHitClips = new List<AudioClip>();
    public AudioClip buttonClip;

    [Header("Audio Source")]
    public AudioSource bgmMusic;
    public AudioSource moveFX;
    public AudioSource fx;

    private bool isPointerOnUI;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        bgmMusic.clip = MenuBgmClip;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "GamePlay Scene" && bgmMusic.clip == startBgmClip)
        {
            if (UIManager.Instance.countTime >= startBgmClip.length - 1 / TransitionManager.Instance.fadeScaler - 0.5f)
            {
                bgmMusic.clip = loopBgmClip;
                PlayBGMMusic();
            }
        }
    }

    private void OnEnable()
    {
        PlayBGMMusic();
    }

    public void PlayBGMMusic()
    {
        if (!bgmMusic.isPlaying)
        {
            bgmMusic.Play();
        }
    }

    public void PlayGetHitFX()
    {
        fx.volume = 0.4f;
        int randomIndex = Random.Range(0, getHitClips.Count);
        fx.clip = getHitClips[randomIndex];
        fx.Play();
    }

    public void PlayGetBuffFX()
    {
        fx.volume = 0.2f;
        fx.clip = getBuffClip;
        fx.Play();
    }

    public void PlayMoveFX()
    {
        moveFX.volume = 0.1f;
        moveFX.clip = moveClip;
        moveFX.Play();
    }

    public void ChangeBgmMusic(AudioClip clip)
    {
        bgmMusic.clip = clip;
        PlayBGMMusic();
    }

    public void OnBGMSliderChanged(float value)
    {
        if (value == 0)
            value = 0.001f;
        mixer.SetFloat("bgmVolume", 10 * Mathf.Log(value * 0.1f));
    }

    public void OnFXSliderChanged(float value)
    {
        if (value == 0)
            value = 0.001f;
        mixer.SetFloat("fxVolume", 10 * Mathf.Log(value * 0.1f));
    }
}
