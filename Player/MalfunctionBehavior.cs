using UnityEngine;

public class MalfunctionBehavior : MonoBehaviour
{
    [SerializeField]
    private LightingSystem lightingSystem;

    [Header("Configuration")]
    [SerializeField]
    [Range(0f, 100f)]
    private float speedReduction;
    private float speedReductionPercentage;
    [SerializeField]
    [Range(0f, 100f)]
    private float lightIntensityReduction;
    private float lightIntensityReductionPercentage;

    [SerializeField]
    [Range(0f, 300f)]
    private float rotationMagnitude;
    [SerializeField]
    [Range(0f, 6f)]
    private float rotationMinimumDelta = 1f;

    [Header("Random Probabilities")]
    [SerializeField]
    [Range(0f, 0.5f)]
    private float lightSystemFailureProbability;
    [SerializeField]
    [Range(0f, 3f)]
    private float randomRotationProbability;

    private Transform head;
    private float headRotationSpeed;

    private bool isHeadRotationInProcess;
    private Quaternion targetHeadRotation;

    private void OnValidate() => AdjustParametersPercentages();

    private void AdjustParametersPercentages()
    {
        speedReductionPercentage = speedReduction / 100f;
        lightIntensityReductionPercentage = lightIntensityReduction / 100f;
    }

    private void Awake()
    {
        lightingSystem = GetComponent<LightingSystem>();

        AdjustParametersPercentages();
    }

    private void OnEnable()
    {
        float remainingAccelerationPercentage = 1f - speedReductionPercentage;
        LevelManager.Instance.PlayerMovement.AccelerationSpeed *= remainingAccelerationPercentage;

        float remainingLightIntensityPercentage = 1f - lightIntensityReductionPercentage;
        lightingSystem.ChangeLightsIntensityIn(remainingLightIntensityPercentage);
    }

    private void Start()
    {
        head = LevelManager.Instance.PlayerMovement.Head;
        headRotationSpeed = LevelManager.Instance.PlayerMovement.HeadRotationSpeed;
    }

    void Update()
    {
        InduceRandomRotation();
        InduceRandomLightsFailure();

        void InduceRandomRotation()
        {
            if (isHeadRotationInProcess)
                RotateHeadWithInterpolation();
            else if (RandomChancesHappens())
                TriggerRotation();

            void RotateHeadWithInterpolation()
            {
                if (!IsHeadRotationNearEnoughToTarget())
                {
                    float actualRotation = Mathf.LerpAngle(head.localRotation.eulerAngles.y,
                                                           targetHeadRotation.eulerAngles.y,
                                                           headRotationSpeed * Time.deltaTime);

                    ApplyLocalRotationToHead(actualRotation);
                }
                else
                    isHeadRotationInProcess = false;

                bool IsHeadRotationNearEnoughToTarget()
                {
                    float deltaBetweenHeadAndTarget = Mathf.Abs(head.localRotation.eulerAngles.y - targetHeadRotation.eulerAngles.y);

                    return deltaBetweenHeadAndTarget < rotationMinimumDelta;
                }
                void ApplyLocalRotationToHead(float actualRotation) => head.localRotation = Quaternion.Euler(new Vector3(0, actualRotation, 0));
            }
            bool RandomChancesHappens() => Random.Range(0f, 100f) < randomRotationProbability;
            void TriggerRotation()
            {
                targetHeadRotation = Quaternion.Euler(new Vector3(0, GetRandomDirection(), 0));

                //CorrectRotationIfExceeds180degrees();

                AudioManager.Instance.Play("MovementMalfunction");

                isHeadRotationInProcess = true;

                float GetRandomDirection()
                {
                    return Random.Range(0, 100) % 2 == 0
                                        ? head.transform.localRotation.eulerAngles.y + rotationMagnitude
                                        : head.transform.localRotation.eulerAngles.y - rotationMagnitude;
                }
                void CorrectRotationIfExceeds180degrees()
                {
                    if (Mathf.Abs(targetHeadRotation.eulerAngles.y - head.localRotation.eulerAngles.y) > 180f)
                    {
                        Vector3 targetRotationInEuler = targetHeadRotation.eulerAngles;
                        targetRotationInEuler.y -= 360f;
                        targetHeadRotation.eulerAngles = targetRotationInEuler;
                    }
                }
            }
        }
        void InduceRandomLightsFailure()
        {
            if (!lightingSystem.IsAnimatorPlaying && RandomChancesHappens())
                lightingSystem.LightFailure();

            bool RandomChancesHappens() => Random.Range(0f, 100f) < lightSystemFailureProbability;
        }
    }

    private void OnDisable()
    {
        float accelerationPercentageIncrement = 1f + speedReductionPercentage;
        LevelManager.Instance.PlayerMovement.AccelerationSpeed *= accelerationPercentageIncrement;

        float lightIntensityPercentageIncrement = 1f + lightIntensityReductionPercentage;
        lightingSystem.ChangeLightsIntensityIn(lightIntensityPercentageIncrement);
    }
}
