using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class ExitScene : SceneTransition
{
    [SerializeField]
    private string nextSceneName;
    [SerializeField]
    private Transform scapePoint;
    [SerializeField]
    private Animator fadeOut;

    [SerializeField]
    private bool lookAtTarget = true;

    [Header("Configuration")]
    [SerializeField]
    [Range(0f, 4f)]
    private float timeToChangeScenes;

    private void Awake()
    {
        Assert.IsFalse(string.IsNullOrEmpty(nextSceneName), "The next scene name is null or empty. It must have a valid scene name");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayExitingTransition();

            if (!fadeOut.isActiveAndEnabled)
                fadeOut.enabled = true;

            fadeOut.Play("FadeOut");
        }

        void PlayExitingTransition()
        {
            if (!lookAtTarget)
                staticCamera.LookAt = null;

            staticCamera.Follow = null;

            StartCoroutine(ChangeScene(scapePoint.position, nextSceneName));

            IEnumerator ChangeScene(Vector3 scapePoint, string nextSceneName, float requiredDistance = 7f)
            {
                float elapsedTime = 0f;

                LevelManager.Instance.MovePlayerTowards(scapePoint, requiredDistance);

                while (elapsedTime <= timeToChangeScenes)
                {
                    elapsedTime += Time.deltaTime;

                    yield return new WaitForFixedUpdate();
                }

                ScenePersistence.Instance.PersistScene();
                PERSceneManager.Instance.ChangeScene(nextSceneName);
            }
        }
    }
}
