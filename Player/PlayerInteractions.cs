using UnityEngine;
using UnityEngine.Assertions;

public class PlayerInteractions : MonoBehaviour
{
    [SerializeField]
    private PickableItem carriedObject;
    public PickableItem CarriedObject { get => carriedObject; set { carriedObject = value; } }
    [SerializeField]
    private FastIKSolution rightArm;
    [SerializeField]
    private FastIKSolution leftArm;

    [Header("Interact")]
    [SerializeField, Range(0, 1)]
    private float carriedObjectDistance = 0.5f;

    void Start()
    {
        Assert.IsNotNull(leftArm);
        Assert.IsNotNull(rightArm);

        LevelManager.Instance.PlayerInteractions = this;

        GetCarriedObjectFromPreviousScene();

        void GetCarriedObjectFromPreviousScene()
    {
        GameObject heldItem = LevelManager.Instance.GetHoldItemPrefab();

        if (heldItem != null)
            MarkItemForBeingCarried(heldItem.GetComponent<PickableItem>());
    }
    }

    void Update()
    {
        Interact();

        void Interact()
        {
            if (Input.GetButtonDown("Fire1"))
                if (carriedObject == null)
                    TakeObject();
                else
                    DropObject();

            void TakeObject()
            {
                foreach (Item item in LevelManager.Instance.Items)
                {
                    if (item.AtRange() && item is PickableItem &&
                        (rightArm.ReachableByArm(item) || leftArm.ReachableByArm(item)))
                    {
                        MarkItemForBeingCarried(item as PickableItem);
                        return; //If the object has been grabbed, do not search for others
                    }
                }
            }
            void DropObject()
            {
                carriedObject.DropObject();
                ObjectPlaced();
            }
        }
    }

    public void ObjectPlaced()
    {
        carriedObject = null;
    }

    public bool ItemFocused()
    {
        return rightArm.ItemFocused() || leftArm.ItemFocused();
    }

    public Item GetFocusedItem()
    {
        Item rightArmFocusedItem = rightArm.FocusedItem;
        Item leftArmFocusedItem = leftArm.FocusedItem;

        return rightArmFocusedItem != null ? rightArmFocusedItem : leftArmFocusedItem;
    }

    public Vector3 GetCarryObjectPosition()
    {
        return transform.position + transform.forward * carriedObjectDistance;
    }

    public void MarkItemForBeingCarried(PickableItem item)
    {
        carriedObject = item;
        carriedObject.GetComponent<Collider>().enabled = false;
    }
}
