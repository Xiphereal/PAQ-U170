using UnityEngine;

public class SoundFXPlayer : MonoBehaviour
{
    [SerializeField]
    private string soundName;

    public void PlaySoundFX() => AudioManager.Instance.Play(soundName);
}
