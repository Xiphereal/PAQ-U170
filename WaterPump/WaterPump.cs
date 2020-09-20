using UnityEngine;

public class WaterPump : MonoBehaviour
{
    [SerializeField]
    private int id;
    public int Id => id;

    [SerializeField]
    private Renderer lightBulb;

    public WaterMineralVialCollector Collector { get; set; }

    private bool isPlayerAtRange;
    public bool IsActive { get; private set; }

    private Color defaultMaterialColor;
    public Color DefaultMaterialColor
    {
        get => defaultMaterialColor;
        set
        {
            defaultMaterialColor = value;
            ResetPump();
        }
    }

    private void Awake() => enabled = false;

    private void OnEnable() => DriveArmsToGameobject();

    void Update()
    {
        CheckForActivation();

        void CheckForActivation()
        {
            if (isPlayerAtRange && Input.GetButtonDown("Fire1") && !IsActive)
                ActivePump();

            void ActivePump()
            {
                SetActive();

                if (Collector.PumpActivated(id))
                    GetComponent<WaterPumpSFX>().TurnOn();
            }
        }
    }

    public void SetActive()
    {
        IsActive = true;
        Destroy(GetComponent<NonPickableItem>());
        lightBulb.material.color = Color.green;
    }

    public void ResetPump()
    {
        IsActive = false;

        if (enabled)
            DriveArmsToGameobject();

        GetComponent<WaterPumpSFX>().TurnOff();
        lightBulb.material.color = DefaultMaterialColor;
    }

    private void DriveArmsToGameobject()
    {
        gameObject.AddComponent<NonPickableItem>().MinDistanceToBeAtRange = 3.5f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            isPlayerAtRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            isPlayerAtRange = false;
    }
}
