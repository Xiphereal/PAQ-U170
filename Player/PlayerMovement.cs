using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Transform head;
    public Transform Head => head;
    [SerializeField]
    private Rigidbody rigidBody;
    public Rigidbody Rigidbody { get => rigidBody; }

    [Header("Movement")]
    [SerializeField]
    private float topForwardSpeed = 5f;
    public float TopForwardSpeed { get => topForwardSpeed; set => topForwardSpeed = value; }
    [SerializeField]
    private float topBackwardSpeed = 3f;
    [SerializeField, Range(1f, 900f)]
    private float accelerationSpeed = 300f;
    public float AccelerationSpeed { get => accelerationSpeed; set => accelerationSpeed = value; }

    [SerializeField, Range(1f, 300f)]
    private float headRotationSpeed = 5f;
    public float HeadRotationSpeed => headRotationSpeed;
    [SerializeField, Range(0, 1)]
    private float bodyRotationSpeed = 0.5f;
    [SerializeField, Range(0, 1)]
    private float bodyTiltSpeed = 0.5f;

    [Header("Ground Detection")]
    [SerializeField]
    private float raycastLength = 1.2f;
    [SerializeField]
    private float raycastFrontPadding = 0.5f;
    [SerializeField]
    private float maxGroundAngle = 120;
    [SerializeField]
    private LayerMask ground;

    private RaycastHit hitInfo;
    private bool grounded;

    public bool IsAbleToRotate { get; set; } = true;

    void Start()
    {
        Assert.IsNotNull(head);
        Assert.IsNotNull(rigidBody);

        LevelManager.Instance.PlayerMovement = this;
    }

    void Update()
    {
        RotateHeadLinearly();

        void RotateHeadLinearly()
        {
            if (IsAbleToRotate)
            {
                float hAxis = Input.GetAxisRaw("Horizontal");

                RotateHead(Quaternion.Euler(Vector3.up * headRotationSpeed * hAxis * Time.deltaTime));
            }

            void RotateHead(Quaternion rotation) => head.rotation *= rotation;
        }
    }

    private void FixedUpdate()
    {
        CheckForGroundCollision();
        RotateBody();
        Move();

        void CheckForGroundCollision()
        {
            Vector3 frontPadding = IsMovingForward()
                ? transform.forward * raycastFrontPadding
                : -transform.forward * raycastFrontPadding;

            grounded = Physics.Raycast(transform.position + frontPadding, Vector3.down, out hitInfo, raycastLength, ground);

            Debug.DrawLine(transform.position + frontPadding, transform.position + frontPadding + Vector3.down * raycastLength, Color.red);

        }
        void Move()
        {
            float vAxis = Input.GetAxisRaw("Vertical");

            if (IsAbleToMove())
                MoveForward();

            void MoveForward()
            {
                Vector3 movementForce = CalculateDirectionRelativeToSlope() * accelerationSpeed * vAxis;

                rigidBody.AddForce(movementForce);
                Debug.DrawLine(transform.position, transform.position + movementForce.normalized, Color.blue);

                Vector3 CalculateDirectionRelativeToSlope()
                {
                    if (!grounded)
                        return transform.forward;

                    return Vector3.Cross(transform.right, hitInfo.normal);
                }
            }
        }
    }

    private bool IsMovingForward() => Input.GetAxisRaw("Vertical") >= 0;

    private bool IsAbleToMove()
    {
        float topSpeed = IsMovingForward() ? topForwardSpeed : topBackwardSpeed;
        float groundAngle = Vector3.Angle(hitInfo.normal, transform.forward);

        return rigidBody.velocity.magnitude <= topSpeed && groundAngle <= maxGroundAngle;
    }

    private void RotateBody()
    {
        RotateBodyToFaceForward();
        RotateToAdjustToGroundSlope();

        void RotateBodyToFaceForward()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, head.rotation, bodyRotationSpeed);
        }
        void RotateToAdjustToGroundSlope()
        {
            Vector3 normal = hitInfo.normal;

            Quaternion targetRotation = new Quaternion(normal.z, 0f, -normal.x, 1f + normal.y).normalized;

            rigidBody.transform.rotation = Quaternion.Slerp(rigidBody.transform.rotation, targetRotation, bodyTiltSpeed);
        }
    }

    public IEnumerator MoveTowardsPoint(Vector3 point, float requiredDistance)
    {
        enabled = false;

        yield return null;

        float distance = Vector3.Distance(transform.position, point);
        while (distance > requiredDistance)
        {
            if (IsAbleToMove())
            {
                Vector3 lookAtPoint = point - rigidBody.position;
                lookAtPoint.y = 0;
                head.rotation = Quaternion.Slerp(head.rotation, Quaternion.LookRotation(lookAtPoint), headRotationSpeed * 2 * Time.deltaTime);
                Debug.DrawLine(head.position, head.position + lookAtPoint);

                RotateBody();

                Vector3 movementForce = transform.forward * accelerationSpeed * 100 * Time.deltaTime;
                rigidBody.AddForce(movementForce);

                Debug.DrawLine(transform.position, transform.position + movementForce.normalized, Color.blue);

                distance = Vector3.Distance(transform.position, point);
            }

            yield return new WaitForFixedUpdate();
        }

        enabled = true;
    }

    public void MoveTo(Vector3 point) => rigidBody.position = point;
}
