using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    private Rigidbody rigidbody;

    private float lastRigidbodySpeed;

    void Start()
    {
        rigidbody = LevelManager.Instance.PlayerMovement.Rigidbody;
        AudioManager.Instance.Play("MovementLoop");
    }

    void LateUpdate()
    {
        AudioManager.Instance.SetSoundPitchWithPlayAndStop("MovementLoop", 
            rigidbody.velocity.magnitude / LevelManager.Instance.PlayerMovement.TopForwardSpeed);
    }
}
