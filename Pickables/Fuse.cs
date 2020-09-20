using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Fuse : PickableItem
{
    [SerializeField]
    private bool isFunctional = false;
    public bool IsFunctional { get => isFunctional; }
    
    public override void ObjectPlaced()
    {
        Destroy(GetComponent<FixedJoint>());
        Destroy(GetComponent<ParentConstraint>());

        GetComponent<Rigidbody>().isKinematic = true;

        IsAvailableToPick = false;
        LevelManager.Instance.PlayerInteractions.ObjectPlaced();
    }
}
