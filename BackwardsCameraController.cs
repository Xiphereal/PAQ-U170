using UnityEngine;

public class BackwardsCameraController : MonoBehaviour
{
    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera backwardsCamera;

    private Transform player;

    [SerializeField, Range(0f, 3f)]
    private float detectionDelay;
    private float elapsedTime;

    [SerializeField, Range(0f, 1f)]
    private float differenceThreshold;
    private float previousDistance;

    private void Start()
    {
        player = LevelManager.Instance.PlayerMovement.transform;
        backwardsCamera.Priority = -1;
    }

    private void Update()
    {
        if (elapsedTime >= detectionDelay)
        {
            if (IsPlayerGoingTowardsCamera())
                backwardsCamera.Priority = 40;
            else
                backwardsCamera.Priority = -1;

            elapsedTime = 0;
        }
        else
            elapsedTime += Time.deltaTime;

        bool IsPlayerGoingTowardsCamera()
        {
            return Vector3.Dot(player.forward, backwardsCamera.transform.forward) < 0;
        }
    }
}
