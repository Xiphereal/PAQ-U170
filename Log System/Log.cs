using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Log : MonoBehaviour
{
    [SerializeField]
    protected GameObject logUI;

    [SerializeField]
    protected static int numberOfDiscoveredLogs = 0;

    private Text logContent;

    private bool alreadyOpened;

    private bool playerInRange;

    private void Awake()
    {
        logContent = GetComponent<Text>();

        Assert.IsNotNull(logUI, "The UI canvas for the log cannot be found. Make sure it's on scene and referenced by the LogManager");
        logUI.SetActive(false);

        enabled = false;
    }

    protected void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            if (playerInRange && !logUI.activeInHierarchy && playerInRange)
                OpenLog();
            else if (logUI.activeInHierarchy)
                CloseLog();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = true;
            enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            playerInRange = false;
    }

    public void OpenLog()
    {
        SetContentsToUI();

        SetPlayerMovementBasedOnLogActivation();

        CheckForLogDiscovering();

        logUI.SetActive(true);

        void SetContentsToUI()
        {
            var canvasText = logUI.transform.GetChild(0).GetComponent<Text>();
            canvasText.text = logContent.text;
            canvasText.fontSize = logContent.fontSize;
            canvasText.lineSpacing = logContent.lineSpacing;
        }
        void CheckForLogDiscovering()
        {
            if (!alreadyOpened)
            {
                numberOfDiscoveredLogs++;
                alreadyOpened = true;
            }
        }
    }

    protected void CloseLog()
    {
        SetPlayerMovementBasedOnLogActivation();
        logUI.SetActive(false);
        enabled = false;
    }

    private void SetPlayerMovementBasedOnLogActivation() => LevelManager.Instance.PlayerMovement.enabled = logUI.activeInHierarchy;
}
