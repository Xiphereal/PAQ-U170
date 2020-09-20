using UnityEngine;
using UnityEngine.Assertions;

public class LightingSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Animator animator;
    public bool IsAnimatorPlaying => animator.GetCurrentAnimatorStateInfo(0).length >
                                     animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    [SerializeField]
    private Light eyeSpotLight;
    [SerializeField]
    private Material material;

    [Header("Config")]
    [SerializeField]
    private float smooth = 2f;

    private float targetIntensity;

    void Start()
    {
        Assert.IsNotNull(animator, "The animator reference for the light system is missing. It must have a valid reference to its own animator");

        targetIntensity = eyeSpotLight.intensity;

        SetEyeLight(eyeSpotLight.enabled);
    }

    private void SetEyeLight(bool active)
    {
        if (active)
            material.EnableKeyword("_EMISSION");
        else
            material.DisableKeyword("_EMISSION");
    }

    void Update()
    {
        InterpolateIntensity();

        void InterpolateIntensity()
        {
            eyeSpotLight.intensity = Mathf.Lerp(eyeSpotLight.intensity, targetIntensity, smooth * Time.deltaTime);
        }
    }

    public void LightFailure()
    {
        animator.Play("lightSystemFailure");
        AudioManager.Instance.Play("LanternMalfunction");
    }

    public void ChangeLightsIntensityIn(float percentage) => targetIntensity = eyeSpotLight.intensity * percentage;

    public void SwitchLights(bool isOn)
    {
        eyeSpotLight.enabled = isOn;

        AudioManager.Instance.Play("LanternSwitch");

        SetEyeLight(isOn);
    }
}
