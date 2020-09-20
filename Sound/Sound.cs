using UnityEngine;

[System.Serializable]
public class Sound
{
    [SerializeField]
    private string name;
    public string Name { get => name; set => name = value; }

    [SerializeField]
    private AudioClip clip;
    public AudioClip Clip { get => clip; set => clip = value; }

    [SerializeField]
    private bool loop;
    public bool Loop { get => loop; set => loop = value; }

    [Range(0, 1),SerializeField]
    private float volume = 1f;
    public float Volume { get => volume; set => volume = value; }
    [Range(.1f, 3f), SerializeField]
    private float pitch = 1f;
    public float Pitch { get => pitch; set => pitch = value; }

    public AudioSource Source { get; set; }
}
