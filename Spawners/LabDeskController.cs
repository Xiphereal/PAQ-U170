using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabDeskController : Spawner
{
    [SerializeField]
    private float processDelay = 2.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Ore ore = LevelManager.Instance.PlayerInteractions.CarriedObject as Ore;
            if (ore != null)
            {
                ore.ObjectPlaced();
                LevelManager.Instance.ThirdTaskCompleted();
                Invoke("SpawnItem", processDelay);
            }
        }
    }

    protected override void SpawnItem()
    {
         Instantiate(prefab, transform.position + spawnPosition, Quaternion.identity);
    }
}
