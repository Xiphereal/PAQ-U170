using Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;

public class AntennaSystem : MonoBehaviour, IPersistable
{
    private SignalSourceGenerator signalSourceGenerator;
    private AntennaFeedback antennaFeedback;

    [SerializeField]
    private AntennaController controller;
    [SerializeField]
    private GameObject playerVcam;
    [SerializeField]
    private CinemachineVirtualCamera antennaVcam;

    [Header("Parameters")]
    [SerializeField]
    private float minDistanceToSource;
    [SerializeField]
    private int waitForSelectorRangeTime;

    private bool isPlayerAtRange;
    private bool isActivated;

    public AntennaController AntennaController => controller;

    private static bool isStatePersisted;
    private static Vector3 previousParableOrientation;

    void Start()
    {
        signalSourceGenerator = GetComponent<SignalSourceGenerator>();
        antennaFeedback = GetComponent<AntennaFeedback>();

        Assert.IsNotNull(controller);
        Assert.IsNotNull(playerVcam);
        Assert.IsNotNull(antennaVcam);

        Assert.AreNotEqual(minDistanceToSource, 0,
            "The minDistanceToSource in AntenaSystemManager shouldn't be 0.");
    }

    void Update()
    {
        CheckForActivation();

        if (controller.enabled)
            CheckForPositioningCompletion();

        void CheckForActivation()
        {
            if (isPlayerAtRange && Input.GetButtonDown("Fire1"))
            {
                if (!isActivated)
                {
                    isActivated = true;
                    WithdrawPlayerControl();
                    EnableSubsystems();
                    DisablePlayerCamera();
                }
                else
                {
                    isActivated = false;
                    RestorePlayerControl();
                    DisableSubsystems();
                    EnablePlayerCamera();
                }
            }

            void WithdrawPlayerControl()
            {
                LevelManager.Instance.PlayerInteractions.enabled = false;
                LevelManager.Instance.PlayerMovement.enabled = false;
                LevelManager.Instance.PlayerMovement.Rigidbody.isKinematic = true;
            }
            void EnableSubsystems()
            {
                controller.enabled = true;
                antennaFeedback.enabled = true;
            }
            void DisablePlayerCamera() => playerVcam.SetActive(false);
            void DisableSubsystems()
            {
                controller.enabled = false;
                antennaFeedback.enabled = false;
                antennaFeedback.ResetColor();
            }
        }
        void CheckForPositioningCompletion()
        {
            if (IsOrientationNearEnoughtToSource())
            {
                RestorePlayerControl();
                PermanentlyDisableSystem();
                EnablePlayerCamera();

                LevelManager.Instance.FifthTaskCompleted();

                isStatePersisted = true;
            }

            bool IsOrientationNearEnoughtToSource()
            {
                Vector3 signalSource = signalSourceGenerator.GetSignalSource();
                Vector3 antennaOrientation = controller.Parable.up;

                return Vector3.Distance(signalSource, antennaOrientation) <= minDistanceToSource;
            }
            void PermanentlyDisableSystem()
            {
                PermanentlyDisableSubsystems();
                Destroy(GetComponent<NonPickableItem>());
                enabled = false;

                void PermanentlyDisableSubsystems()
                {
                    controller.enabled = false;
                    antennaFeedback.enabled = false;
                    antennaFeedback.FinalColor();
                }
            }
        }
    }

    private void RestorePlayerControl()
    {
        LevelManager.Instance.PlayerInteractions.enabled = true;
        LevelManager.Instance.PlayerMovement.enabled = true;
        LevelManager.Instance.PlayerMovement.Rigidbody.isKinematic = false;
    }

    private void EnablePlayerCamera() => playerVcam.SetActive(true);

    public float GetDistanceFromAntennaToSource()
    {
        Vector3 signalSource = signalSourceGenerator.GetSignalSource();
        Vector3 antennaOrientation = controller.Parable.up;

        return Vector3.Distance(signalSource, antennaOrientation);
    }

    private void OnTriggerEnter(Collider other) => isPlayerAtRange = true;

    private void OnTriggerExit(Collider other) => isPlayerAtRange = false;

    public void PersistState()
    {
        if (isStatePersisted)
            previousParableOrientation = controller.Parable.up;
    }

    public void ResetState() => isStatePersisted = false;

    public void RestoreState()
    {
        if (isStatePersisted)
        {
            controller.Parable.up = previousParableOrientation;
            antennaFeedback.FinalColor();
        }
    }
}
