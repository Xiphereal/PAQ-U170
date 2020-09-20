using UnityEngine;
using UnityEngine.Assertions;

public class AntennaController : MonoBehaviour
{
    [SerializeField]
    private Transform parable;
    public Transform Parable { get { return parable; } }

    [Header("Rotation")]
    [SerializeField]
    private float rotationSpeed;

    [Header("Tilting")]
    [SerializeField]
    private float tiltBoundaryAngle;
    public float TiltBoundaryAngle { get { return tiltBoundaryAngle; } }
    [SerializeField]
    private float tiltSpeed;

    void Start()
    {
        Assert.AreNotEqual(rotationSpeed, 0);
        Assert.AreNotEqual(tiltBoundaryAngle, 0);
        Assert.IsNotNull(parable);

        enabled = false;
    }

    void Update()
    {
        RotateAntenna();
        TiltParable();

        void RotateAntenna()
        {
            float hAxis = Input.GetAxisRaw("Horizontal");

            Quaternion rotation = Quaternion.Euler(Vector3.up * rotationSpeed * hAxis * Time.deltaTime);
            transform.rotation *= rotation;
        }
        void TiltParable()
        {
            float vAxis = Input.GetAxisRaw("Vertical");

            float minTiltValue = 360 - tiltBoundaryAngle;
            float maxTiltValue = tiltBoundaryAngle;

            Quaternion rotation = Quaternion.Euler(0, 0, tiltSpeed * vAxis * Time.deltaTime);
            rotation *= parable.localRotation;

            if (IsRotationBetweenBoundaries())
                parable.localRotation = rotation;

            bool IsRotationBetweenBoundaries()
            {
                return rotation.eulerAngles.z <= maxTiltValue || rotation.eulerAngles.z >= minTiltValue;
            }
        }
    }
}
