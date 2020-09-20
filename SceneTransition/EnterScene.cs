using System.Collections;
using UnityEngine;

public class EnterScene : SceneTransition
{
    [SerializeField]
    private Transform enteringInitialPoint;
    private Vector3 enteringFinalPoint;
    [SerializeField]
    private GameObject previousSceneTrigger;
    [SerializeField]
    private bool makeCameraStatic = true;

    [SerializeField]
    private int priority;
    public int Priority => priority;

    private void Awake()
    {
        PERSceneManager.Instance.SceneEntrances.Add(this);
        gameObject.SetActive(false);
    }

    private void Start()
    {
        SetPreviousSceneTriggerTo(false);
        MovePaquitoToEntrance();
        InitializeEnteringTransition();

        void MovePaquitoToEntrance()
        {
            enteringFinalPoint = transform.position;
            LevelManager.Instance.MovePlayerTo(enteringInitialPoint.position);
        }
        void InitializeEnteringTransition()
        {
            if (makeCameraStatic)
                DecoupleStaticCameraFromTarget();

            MakeStaticCameraLive();

            StartCoroutine(PlayEnteringTransition());

            void DecoupleStaticCameraFromTarget()
            {
                staticCamera.Follow = null;
                staticCamera.LookAt = null;
            }
            void MakeStaticCameraLive() => staticCamera.Priority = 100;
            IEnumerator PlayEnteringTransition()
            {
                LevelManager.Instance.MovePlayerTowards(enteringFinalPoint, 2f);

                while (!IsPlayerAtFinalPoint())
                    yield return null;

                SetMainCameraLive();
                SetPreviousSceneTriggerTo(true);

                bool IsPlayerAtFinalPoint() => LevelManager.Instance.PlayerMovement.isActiveAndEnabled == true;
                void SetMainCameraLive() => staticCamera.Priority = -1;
            }
        }
    }

    private void SetPreviousSceneTriggerTo(bool active)
    {
        if (previousSceneTrigger != null)
            previousSceneTrigger.SetActive(active);
    }

    private void OnDestroy() => PERSceneManager.Instance.SceneEntrances.Remove(this);
}
