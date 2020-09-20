using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PERSceneManager : MonoBehaviour
{
    public static PERSceneManager Instance { get; set; }

    public static string PreviousSceneName { get; private set; }
    public List<EnterScene> SceneEntrances { get; set; } = new List<EnterScene>();

    private void Awake()
    {
        //Ensure singleton
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += SetPaquitoEntrancePoint;
    }

    private void SetPaquitoEntrancePoint(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
            return;

        EnterScene legitEntrancePoint = PreviousSceneName == null
                                        ? GetEntranceWithHighestPriority()
                                        : GetEntranceFromPreviousScene();

        if (legitEntrancePoint != null)
            legitEntrancePoint.gameObject.SetActive(true);
        else
            Debug.LogError($"{nameof(legitEntrancePoint)} was null on {nameof(PERSceneManager)}");

        EnterScene GetEntranceFromPreviousScene()
        {
            return SceneEntrances.Find(sceneEntrance => sceneEntrance.name.Contains(PreviousSceneName));
        }
        EnterScene GetEntranceWithHighestPriority()
        {
            var sceneEntrancesByPriority = SceneEntrances.OrderByDescending(sceneEntrance => sceneEntrance.Priority).ToList();

            return sceneEntrancesByPriority.Count() > 0 ? sceneEntrancesByPriority[0] : null;
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ChangeScene(string name)
    {
        GameManager.Instance.SavePlayerHoldItem();

        PreviousSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(name);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SetPaquitoEntrancePoint;
    }
}
