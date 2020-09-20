using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePersistence : MonoBehaviour
{
    public static ScenePersistence Instance { get; private set; }

    private List<IPersistable> persistables = new List<IPersistable>();

    private void Awake()
    {
        EnsureSingleton();

        DontDestroyOnLoad(gameObject);

        void EnsureSingleton()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += RestorePersistedElementsState;

        void RestorePersistedElementsState(Scene arg0, LoadSceneMode arg1)
        {
            StartCoroutine(RestorePersistedElementsStateAfterSceneInitialization());

            IEnumerator RestorePersistedElementsStateAfterSceneInitialization()
            {
                yield return null;

                foreach (var persistable in FindObjectsOfType<MonoBehaviour>().OfType<IPersistable>())
                    persistable.RestoreState();
            }
        }
    }

    public void PersistScene()
    {
        foreach (var persistable in FindObjectsOfType<MonoBehaviour>().OfType<IPersistable>())
        {
            persistable.PersistState();
            persistables.Add(persistable);
        }
    }

    public void ResetPersistence()
    {
        foreach (var persistable in persistables)
            persistable.ResetState();

        persistables.Clear();
    }
}
