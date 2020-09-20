using UnityEngine;
using UnityEngine.Animations;

public class Vial : PickableItem
{
    public override void ObjectPlaced()
    {
        Destroy(GetComponent<FixedJoint>());
        Destroy(GetComponent<ParentConstraint>());
        LevelManager.Instance.PlayerInteractions.ObjectPlaced();
        Destroy(gameObject);
    }
}
