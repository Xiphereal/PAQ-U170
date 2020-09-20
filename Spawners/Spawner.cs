using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class Spawner : MonoBehaviour
{
    [SerializeField]
    protected GameObject prefab;
    [SerializeField]
    protected Vector3 spawnPosition;

    void Start()
    {
        Assert.IsNotNull(prefab);
    }

    protected abstract void SpawnItem();
}
