using UnityEngine;

public class AnimationEventSceneLoader : MonoBehaviour
{
    public void ResetGameLoop()
    {
        ChangeSceneToFacilityEntrance();
        LevelManager.Instance.ResetLoop();
    }

    public void ChangeSceneToFacilityEntrance() => PERSceneManager.Instance.ChangeScene("FacilityEntrance");

    public void ChangeSceneToMainMenu() => PERSceneManager.Instance.ChangeScene("MainMenu");

    public void RestoreTime() => Time.timeScale = 1f;

    public void DeactivateGameObject() => gameObject.SetActive(false);
}
