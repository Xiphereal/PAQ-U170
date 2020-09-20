using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private string musicName;

    void Start() => MusicManager.Instance.Play(musicName);
}
