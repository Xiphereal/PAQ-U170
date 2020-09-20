using UnityEngine;
using UnityEngine.Animations;

public abstract class PickableItem : Item
{
    protected bool IsAvailableToPick { get; set; }

    public abstract void ObjectPlaced();

    public void DropObject()
    {
        if (GetComponent<FixedJoint>() != null)
            Destroy(GetComponent<FixedJoint>());

        if (GetComponent<ParentConstraint>() != null)
            Destroy(GetComponent<ParentConstraint>());

        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().enabled = true;
        LevelManager.Instance.PlayerInteractions.ObjectPlaced();
    }

    public void AttachTo(Transform element)
    {
        ConstraintSource constraintSource = new ConstraintSource
        {
            sourceTransform = element,
            weight = 1f
        };
        gameObject.AddComponent<ParentConstraint>().AddSource(constraintSource);
        GetComponent<ParentConstraint>().constraintActive = true;

        GetComponent<Rigidbody>().isKinematic = true;
    }
}
