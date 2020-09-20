using UnityEngine;
using UnityEngine.SceneManagement;

public class TrailerSceneManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        ChangeScene();

        void ChangeScene()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SceneManager.LoadScene("TRAILERFacilityEntrance");
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                SceneManager.LoadScene("TRAILERGreenHouseAndQuarry");
        }
    }
}
