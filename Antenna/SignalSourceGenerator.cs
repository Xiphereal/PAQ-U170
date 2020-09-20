using UnityEngine;
using UnityEngine.Assertions;

public class SignalSourceGenerator : MonoBehaviour
{
    [SerializeField]
    private Transform customPoint;

    private Transform antenna;
    private Transform parable;
    private float tiltBoundaryAngle;
    private Vector3 signalSource;
    private Vector3 boundary;

    [Header("Debug")]
    [SerializeField]
    private bool debug;
    [SerializeField]
    private int testRuns = 10;
    [SerializeField]
    private float vectorRaysShowTime = 10f;

    void Start()
    {
        ResolveDependencies();

        CalculateBoundaryVector();
        CalculateSignalSource();
        ValidateSignalSource();

        if (debug)
            DebugSystem();
    }

    private void ResolveDependencies()
    {
        AntennaController controller = GetComponent<AntennaSystem>().AntennaController;
        antenna = controller.transform;
        Assert.IsNotNull(antenna);

        parable = controller.Parable;
        Assert.IsNotNull(parable);

        tiltBoundaryAngle = controller.TiltBoundaryAngle;
        Assert.AreNotEqual(tiltBoundaryAngle, 0);
    }

    private void CalculateBoundaryVector()
    {
        boundary = (Quaternion.Euler(0, 0, tiltBoundaryAngle) * Vector3.right);
    }

    private void CalculateSignalSource()
    {
        float xRandomValue = Random.Range(-boundary.x, boundary.x);
        float yRandomValue = Random.Range(boundary.y, 1);
        float zRandomValue = Random.Range(-boundary.x, boundary.x);

        signalSource = (new Vector3(xRandomValue, yRandomValue, zRandomValue)).normalized;
    }

    private void ValidateSignalSource()
    {
        var proyection = Vector3.Project(signalSource, Vector3.right);
        var angle = Vector3.Angle(signalSource, proyection);

        if (debug)
        {
            if (angle >= tiltBoundaryAngle)
                Debug.DrawRay(parable.position, signalSource, Color.black, vectorRaysShowTime);
            else
                Debug.DrawRay(parable.position, signalSource, Color.yellow, vectorRaysShowTime);
        }

        Assert.IsTrue(angle >= tiltBoundaryAngle, "SourceSignal angle exceded limit of "
            + tiltBoundaryAngle + ". The angle was: " + angle);
    }

    private void DebugSystem()
    {
        for (int i = 1; i < testRuns; i++)
        {
            CalculateSignalSource();
            ValidateSignalSource();
        }
    }

    void Update()
    {
        if (debug)
            DebugVectorGeneration();
    }

    private void DebugVectorGeneration()
    {
        Debug.DrawRay(parable.position, signalSource, Color.cyan);
        Debug.DrawRay(parable.position, boundary, Color.red);
    }

    public Vector3 GetSignalSource()
    {
        return signalSource;
    }

    private void OnDrawGizmosSelected()
    {
        if (debug)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(parable.position, 1);
        }
    }
}
