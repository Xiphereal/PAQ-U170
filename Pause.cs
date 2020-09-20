using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField]
    private GameObject pausePanel;

    private void Awake() => pausePanel.SetActive(false);

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (pausePanel.activeInHierarchy == false)
                Open();
            else
                Close();
        }
    }

    public void Open()
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        AudioManager.Instance.PauseAllSounds();
    }

    public void Close()
    {
        AudioManager.Instance.ResumeAllSounds();
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
