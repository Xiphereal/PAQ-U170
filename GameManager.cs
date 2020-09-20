using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    [SerializeField]
    private ItemReferences itemReferences;

    [SerializeField]
    private GameObject holdItem;
    public GameObject HoldItem
    {
        get => holdItem;
        set
        {
            holdItem = FindItemPrefab(value);

            GameObject FindItemPrefab(GameObject goal)
            {
                if (goal == null)
                    return null;

                foreach (GameObject reference in itemReferences.Items)
                    if (reference.tag == goal.tag)
                        return reference;

                return null;
            }
        }
    }

    void Awake()
    {
        //Ensure singleton
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void SavePlayerHoldItem()
    {
        PickableItem carriedObject = LevelManager.Instance?.PlayerInteractions?.CarriedObject;

        HoldItem = carriedObject != null ? carriedObject.gameObject : null;
    }
}
