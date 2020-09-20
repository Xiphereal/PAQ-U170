using System.Collections.Generic;
using UnityEngine;

public class Fruit : PickableItem, IPersistable
{
    [SerializeField]
    private string name;
    public string Name => name;

    private static Dictionary<string, bool> collectedFruits = new Dictionary<string, bool>();

    public override void ObjectPlaced()
    {
        collectedFruits.Add(Name, true);
        Destroy(gameObject);
        LevelManager.Instance.PlayerInteractions.ObjectPlaced();
    }

    public void PersistState() { }

    public void ResetState() => collectedFruits.Clear();

    public void RestoreState()
    {
        collectedFruits.TryGetValue(Name, out bool hasBeenCollected);

        if (hasBeenCollected)
            Destroy(gameObject);
    }
}
