using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField]
    private List<Sound> musics;

    private string activeMusic;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Play(string name)
    {
        if (activeMusic == name)
            return;

        activeMusic = name;

        var music = musics.Find(sound => sound.Name == name);

        var source = GetComponent<AudioSource>();
        source.Stop();

        ConfigureSource();

        source.Play();

        void ConfigureSource()
        {
            source.clip = music.Clip;
            source.loop = music.Loop;
            source.volume = music.Volume;
            source.pitch = music.Pitch;
        }
    }
}
