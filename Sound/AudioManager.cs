using System;
using UnityEngine;
using UnityEngine.Assertions;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField]
    private Sound[] sounds;

    [Space(10), Header("General Configuration")]
    [SerializeField, Range(0.001f, 10f)]
    private float pitchDecrementRate;
    [SerializeField, Range(0.001f, 10f)]
    private float pitchIncrementRate;

    private Coroutine activePlayCoroutine;
    private Coroutine activeStopCoroutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        InitializeSounds();

        void InitializeSounds()
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i].Source = gameObject.AddComponent<AudioSource>();
                sounds[i].Source.clip = sounds[i].Clip;

                sounds[i].Source.volume = sounds[i].Volume;
                sounds[i].Source.pitch = sounds[i].Pitch;
                sounds[i].Source.loop = sounds[i].Loop;
            }
        }
    }

    public void Play(string name)
    {
        Sound sound = FindSound(name);

        sound.Source.Play();
    }

    public void PlayIfNotAlreadyPlaying(string name)
    {
        Sound sound = FindSound(name);

        PlayIfNotAlreadyPlaying(sound);
    }

    private void PlayIfNotAlreadyPlaying(Sound sound)
    {
        if (!sound.Source.isPlaying)
            sound.Source.Play();
    }

    public void Stop(string name)
    {
        Sound sound = FindSound(name);

        Stop(sound);
    }

    private void Stop(Sound sound)
    {
        if (sound.Source.isPlaying)
            sound.Source.Stop();
    }

    private Sound FindSound(string name)
    {
        Sound sound = Array.Find(sounds, s => s.Name == name);

        Assert.IsNotNull(sound, "The sound " + name + " has not been found. Maybe wrong spelling at AudioManager?");

        return sound;
    }

    public void SetSoundPitchWithPlayAndStop(string name, float pitchPercentage)
    {
        Sound sound = FindSound(name);

        PlayIfNotAlreadyPlaying(sound);

        SetLerpedPitch(sound, pitchPercentage);

        if (sound.Source.pitch < 0.01f)
            Stop(sound);
    }

    public void SetSoundPitch(string name, float pitchPercentage)
    {
        Sound sound = FindSound(name);
        SetLerpedPitch(sound, pitchPercentage);
    }

    private static void SetLerpedPitch(Sound sound, float pitchPercentage)
    {
        sound.Source.pitch = Mathf.Lerp(0f, sound.Pitch, pitchPercentage);
    }

    public void PauseAllSounds() => Array.ForEach(sounds, sound => sound.Source.Pause());

    public void ResumeAllSounds() => Array.ForEach(sounds, sound => sound.Source.UnPause());
}
