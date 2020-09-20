using UnityEngine;
using UnityEngine.Assertions;

public class GateController : MonoBehaviour, IPersistable
{
    [SerializeField]
    private GameObject gate;
    [SerializeField]
    private GameObject fuseLight;
    [SerializeField]
    private GameObject fakeFuse;

    PlayerInteractions player;

    [SerializeField]
    private Material greenMaterial;
    private bool socketFree = false;
    [SerializeField]
    private float rayCastLength = 1f;
    private Animator animator;

    private static bool isStatePersisted;

    void Start()
    {
        Assert.IsNotNull(greenMaterial);
        animator = GetComponent<Animator>();

        Assert.IsNotNull(gate);
        Assert.IsNotNull(fuseLight);
        Assert.IsNotNull(animator);
        Assert.IsNotNull(fakeFuse);

        fakeFuse.SetActive(false);
    }

    void Update()
    {
        CheckSocketAvailability();

        void CheckSocketAvailability()
        {
            int layerMask = 1 << 8;

            layerMask = ~layerMask;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayCastLength, layerMask))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                socketFree = false;
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * rayCastLength, Color.white);
                socketFree = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Fuse fuse = LevelManager.Instance.PlayerInteractions.CarriedObject as Fuse;

        if (other.tag == "Player" && fuse != null)
            if (fuse.IsFunctional && socketFree)
            {
                PlaceFuse(fuse);
                LevelManager.Instance.FirstTaskCompleted();
            }
    }

    private void PlaceFuse(PickableItem item = null)
    {
        if (item != null)
        {
            item.ObjectPlaced();
            GameObject.Destroy(item.gameObject);
        }

        fakeFuse.SetActive(true);

        PlayPlaceFuseAnimation();

        SetLightToGreen();
        OpenGate();

        void PlayPlaceFuseAnimation()
        {
            animator.enabled = true;
            animator.Play("placingFuse");
        }
        void SetLightToGreen() => fuseLight.GetComponent<MeshRenderer>().material = greenMaterial;
        void OpenGate()
        {
            gate.GetComponent<Animator>().enabled = true;
            gate.GetComponent<Animator>().Play("openGate");
        }
    }

    public void PersistState() => isStatePersisted = true;

    public void ResetState() => isStatePersisted = false;

    public void RestoreState()
    {
        if (isStatePersisted)
            KeepDoorOpened();

        void KeepDoorOpened() => PlaceFuse();
    }
}
