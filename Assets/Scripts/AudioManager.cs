using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip menuMusic;
    public AudioClip gameMusic;

    public float fadeDuration = 0.5f;

    const string MUSIC_VOL_KEY = "MusicVolume";

    Coroutine fadeCo;
    float targetVolume;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureSources();
    }

    void Start()
    {
        targetVolume = PlayerPrefs.GetFloat(MUSIC_VOL_KEY, 0.5f);
        SetVolume(targetVolume);
        PlayMenuMusic();
    }

    void EnsureSources()
    {
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;

        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f;
    }

    public void SetVolume(float v)
    {
        targetVolume = Mathf.Clamp01(v);
        musicSource.volume = targetVolume;
        PlayerPrefs.SetFloat(MUSIC_VOL_KEY, targetVolume);
        PlayerPrefs.Save();
    }

    public float GetVolume() => targetVolume;

    public void PlayMenuMusic() => CrossfadeTo(menuMusic);
    public void PlayGameMusic() => CrossfadeTo(gameMusic);

    void CrossfadeTo(AudioClip next)
    {
        if (next == null) return;
        if (musicSource.clip == next && musicSource.isPlaying) return;

        if (fadeCo != null) StopCoroutine(fadeCo);
        fadeCo = StartCoroutine(Crossfade(next));
    }

    IEnumerator Crossfade(AudioClip next)
    {
        float t = 0f;
        float start = musicSource.volume;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(start, 0f, t / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = next;
        musicSource.Play();

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = targetVolume;
        fadeCo = null;
    }

    public void PlaySFXOneShot(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    public void DuckMusic(float multiplier, float time)
    {
        if (fadeCo != null) StopCoroutine(fadeCo);
        fadeCo = StartCoroutine(DuckRoutine(multiplier, time));
    }

    IEnumerator DuckRoutine(float mult, float time)
    {
        float start = musicSource.volume;
        float target = targetVolume * Mathf.Clamp01(mult);
        float t = 0f;

        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(start, target, t / time);
            yield return null;
        }

        musicSource.volume = target;
        fadeCo = null;
    }

    public void RestoreMusic(float time)
    {
        if (fadeCo != null) StopCoroutine(fadeCo);
        fadeCo = StartCoroutine(RestoreRoutine(time));
    }

    IEnumerator RestoreRoutine(float time)
    {
        float start = musicSource.volume;
        float t = 0f;

        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(start, targetVolume, t / time);
            yield return null;
        }

        musicSource.volume = targetVolume;
        fadeCo = null;
    }
}