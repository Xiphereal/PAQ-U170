using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WaterPumpSFX : MonoBehaviour
{
    [SerializeField]
    private AudioClip start;
    [SerializeField]
    private AudioClip loop;
    [SerializeField]
    private AudioClip end;

    private AudioSource source;

    private void Awake() => source = GetComponent<AudioSource>();

    public void TurnOn()
    {
        source.clip = start;
        source.loop = false;
        source.Play();

        StartCoroutine(StartLoopOnClipEnd());

        IEnumerator StartLoopOnClipEnd()
        {
            while (source.isPlaying)
                yield return null;

            source.clip = loop;
            source.loop = true;
            source.Play();
        }
    }

    public void TurnOff()
    {
        if (!source.isPlaying)
            return;

        source.Stop();

        source.clip = end;
        source.loop = false;
        source.Play();
    }
}
