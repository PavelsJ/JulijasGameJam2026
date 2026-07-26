using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    
    public AudioSource musicSource;
    public AudioSource voiceSource;
    public AudioSource sfxSource;
    
    public AudioMixer audioMixer;

    [Header("Music")]
    public AudioClip[] gameMusic;
    public AudioClip[] transitionMusic;

    [Header("SFX Sounds")] 
    [SerializeField] private float sameSoundCooldown = 0.1f;
    public List<SoundGroup> soundGroups; 
    private Dictionary<SoundType, AudioClip[]> soundDictionary;
    
    private Dictionary<SoundType, float> lastSoundTime = new Dictionary<SoundType, float>();
    
    [Header("Typing Sounds")]
    public List<TypingGroup> typingGroups;
    private Dictionary<TypingType, AudioClip[]> typingDictionary;
    
    public enum SoundType
    {
        ButtonPress,
        WrongButtonPress,
        
        KnightFootsteps,
        KnightJump,
        KnightLand,
        KnightHit,
        KnightDeath,
        KnightFinish,
        KnightRespawn,
        KnightThrow,
        
        ObjectExplosion,
        
        ItemCollection,
        
        GameOver,
        
        OpenTab,
        CloseTab,
        
        Kazoo,
    }
    
    public enum TypingType
    {
        Normal,
        Soft,
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSoundDictionary();
            InitializeTypingDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        ApplySavedVolumes();
        
        if (gameMusic.Length == 0) return;
        
        StartCoroutine(FadeInMusic(gameMusic[0], 1f));
    }
    
    private IEnumerator FadeInMusic(AudioClip clip, float fadeTime)
    {
        float targetVolume = musicSource.volume;
        
        musicSource.clip = clip;
        musicSource.volume = 0f;
        musicSource.Play();

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, t / fadeTime);
            yield return null;
        }

        musicSource.volume = targetVolume;
    }
     
    private void ApplySavedVolumes()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.25f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        float voiceVolume = PlayerPrefs.GetFloat("VoiceVolume", 0.5f);
        
        SetMixerVolume("Music", musicVolume);
        SetMixerVolume("SFX", sfxVolume);
        SetMixerVolume("Voice", voiceVolume);
    }
    
    private void SetMixerVolume(string parameter, float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        audioMixer.SetFloat(parameter, Mathf.Log10(value) * 20f);
    }
    
    private void InitializeSoundDictionary()
    {
        soundDictionary = new Dictionary<SoundType, AudioClip[]>();
        foreach (var group in soundGroups)
        {
            if (!soundDictionary.ContainsKey(group.type))
            {
                soundDictionary.Add(group.type, group.clips);
            }
        }
    }
    
    public void SwapMusic(AudioClip newClip, float fadeTime = 1f)
    {
        StartCoroutine(SwapMusicRoutine(newClip, fadeTime));
    }
    
    private IEnumerator SwapMusicRoutine(AudioClip newClip, float fadeTime)
    {
        float startVolume = musicSource.volume;

        // Fade OUT
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeTime);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade IN
        t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, startVolume, t / fadeTime);
            yield return null;
        }

        musicSource.volume = startVolume;
    }
    
    public void PlayRandomSound(SoundType type, float pitch = 1f)
    {
        if (lastSoundTime.TryGetValue(type, out float lastTime))
        {
            if (Time.time - lastTime < sameSoundCooldown)
                return; 
        }

        AudioClip clip = GetRandomClip(type);
        if (clip == null) return;

        lastSoundTime[type] = Time.time;

        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip);
    }
    
    private AudioClip GetRandomClip(SoundType type)
    {
        if (soundDictionary.TryGetValue(type, out AudioClip[] clips) && clips.Length > 0)
        {
            return clips[Random.Range(0, clips.Length)];
        }
        return null;
    }
    
    public void PlayOneSound(SoundType type, int index = 0, float pitch = 1f)
    {
        if (soundDictionary.TryGetValue(type, out AudioClip[] clips))
        {
            if (clips != null && clips.Length > index)
            {
                AudioClip clip = clips[index];
                sfxSource.pitch = pitch;
                sfxSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"Index {index} outside the bounds of {type}.");
            }
        }
    }
    
    private void InitializeTypingDictionary()
    {
        typingDictionary = new Dictionary<TypingType, AudioClip[]>();
        foreach (var group in typingGroups)
        {
            if (!typingDictionary.ContainsKey(group.type))
                typingDictionary.Add(group.type, group.clips);
        }
    }
    
    public void PlayTyping(char c, TypingType type)
    {
        if (c == ' ') return;

        if (!typingDictionary.TryGetValue(type, out var clips)) return;
        if (clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];

        float pitch = 1f;

        if (char.IsPunctuation(c))
            pitch = Random.Range(0.6f, 0.8f);
        else if ("AEIOUYaeiouy".Contains(c))
            pitch = Random.Range(1.1f, 1.3f);
        else
            pitch = Random.Range(0.9f, 1.1f);

        voiceSource.pitch = pitch;
        voiceSource.PlayOneShot(clip, 0.6f);
    }

    public void StopTyping()
    {
        
    }
}

[System.Serializable]
public class SoundGroup
{
    public MusicManager.SoundType type;
    public AudioClip[] clips;
}

[System.Serializable]
public class TypingGroup
{
    public MusicManager.TypingType type;
    public AudioClip[] clips;
}