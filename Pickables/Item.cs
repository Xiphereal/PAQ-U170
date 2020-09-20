using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
    [SerializeField]
    protected float minDistanceToBeAtRange = 2.2f;

    void Start() => LevelManager.Instance.AddItemInstance(this);

    public bool AtRange()
    {
        Transform player = LevelManager.Instance.PlayerInteractions.transform;
        return Vector3.Distance(transform.position, player.position) < minDistanceToBeAtRange;
    }

    private void OnDestroy() => LevelManager.Instance.RemoveItemInstance(this);
}
