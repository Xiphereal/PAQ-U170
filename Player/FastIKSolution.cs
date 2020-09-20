#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class FastIKSolution : MonoBehaviour
{
    [SerializeField]
    private int chainLength = 2;

    [SerializeField]
    private Transform target;
    [SerializeField]
    private Transform defaultTarget;
    [SerializeField]
    private Transform pole;

    [Header("Solver Parameters")]
    [SerializeField]
    private int iterations = 10;
    [SerializeField]
    private float delta = 0.001f;
    [SerializeField]
    [Range(0, 1)]
    private float snapBackStrength = 1f;

    [Header("Options")]
    [SerializeField]
    private bool rotation = true;
    private Vector3 smoothTargetTransitionSpeed = Vector3.zero;
    [SerializeField]
    [Range(0, 1)]
    private float smoothTargetTransitionTime = 0.1f;
    [SerializeField]
    [Range(0, 1)]
    private float resetTransitionTime = 0.5f;
    [SerializeField]
    [Range(0, 360)]
    private float maxTargetAngle = 152f;
    [SerializeField]
    [Range(0, 90)]
    private float armRangeOffset = 20f;

    protected float[] bonesLength; //Target to Origin
    protected float completeLength;
    protected Transform[] bones;
    protected Vector3[] positions;
    protected Vector3[] startDirectionSucc;
    protected Quaternion[] startRotationBone;
    protected Quaternion startRotationTarget;
    protected Quaternion startRotationRoot;

    public Item FocusedItem { get; protected set; }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        bones = new Transform[chainLength + 1];
        positions = new Vector3[chainLength + 1];
        bonesLength = new float[chainLength];
        startDirectionSucc = new Vector3[chainLength + 1];
        startRotationBone = new Quaternion[chainLength + 1];

        //Create a target if none exits.
        if (target == null)
        {
            target = new GameObject(gameObject.name + " Target").transform;
            target.position = transform.position;
        }

        startRotationTarget = target.rotation;
        completeLength = 0;

        //Initialize rotation, position & length for each bone.
        Transform current = transform;
        for (int i = bones.Length - 1; i >= 0; i--)
        {
            bones[i] = current;
            startRotationBone[i] = current.rotation;

            if (i == bones.Length - 1)
            {
                //Leaf bone.
                startDirectionSucc[i] = target.position - current.position;
            }
            else
            {
                //Middle bone.
                startDirectionSucc[i] = bones[i + 1].position - current.position;
                bonesLength[i] = startDirectionSucc[i].magnitude;
                completeLength += bonesLength[i];
            }

            current = current.parent;
        }

        if (bones[0] == null)
            throw new UnityException("The chain value is longer than the ancestor chain!");

        startRotationRoot = (bones[0].parent != null) ? bones[0].parent.rotation : Quaternion.identity;
    }

    private void Update()
    {
        ManageTarget();

        void ManageTarget()
        {
            PickableItem item = LevelManager.Instance.PlayerInteractions.CarriedObject;

            if (IsPlayerCarrying(item))
            {
                if (item.GetComponent<ParentConstraint>() == null)
                    item.AttachTo(gameObject.transform);

                MoveTargetToCarryObjectPosition();
            }
            else
            {
                List<Item> items = LevelManager.Instance.Items;

                FocusedItem = null;
                bool resetAlready = false;

                foreach (Item element in items)
                {
                    if (element != null && element.AtRange())
                    {
                        //Even if the object is not reachable by the arm, do not choose another object
                        if (ReachableByArm(element))
                        {
                            FocusedItem = element;
                            target.position = Vector3.SmoothDamp(target.position, element.gameObject.transform.position, ref smoothTargetTransitionSpeed, smoothTargetTransitionTime);
                        }
                        else
                            target.position = Vector3.SmoothDamp(target.position, defaultTarget.position, ref smoothTargetTransitionSpeed, resetTransitionTime);
                        break;
                    }
                    else if (!resetAlready)
                    {
                        target.position = Vector3.SmoothDamp(target.position, defaultTarget.position, ref smoothTargetTransitionSpeed, resetTransitionTime);
                        resetAlready = true;
                    }
                }
            }

            bool IsPlayerCarrying(PickableItem pickableItem)
            {
                return pickableItem != null && ReachableByArm(pickableItem);
            }
            void MoveTargetToCarryObjectPosition()
            {
                Vector3 objectFinalPosition = LevelManager.Instance.PlayerInteractions.GetCarryObjectPosition();
                Debug.DrawLine(target.position, objectFinalPosition, Color.green);
                target.position = Vector3.SmoothDamp(target.position, objectFinalPosition, ref smoothTargetTransitionSpeed, smoothTargetTransitionTime);
            }
        }
    }

    private void LateUpdate()
    {
        ResolveIK();

        void ResolveIK()
        {
            if (target == null)
                return;

            if (bonesLength.Length != chainLength)
                Initialize();

            //Get each bone position.
            for (int i = 0; i < bones.Length; i++)
                positions[i] = bones[i].position;

            Quaternion rootRotation = (bones[0].parent != null) ? bones[0].parent.rotation : Quaternion.identity;
            Quaternion rootRotationDifference = rootRotation * Quaternion.Inverse(startRotationRoot);

            //Resolve the IK.
            for (int i = 0; i < positions.Length - 1; i++)
                positions[i + 1] = Vector3.Lerp(positions[i + 1], positions[i] + rootRotationDifference * startDirectionSucc[i], snapBackStrength);

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                //Backwards.
                for (int i = positions.Length - 1; i > 0; i--)
                {
                    if (i == positions.Length - 1)
                    {
                        //Leaf bone.
                        positions[i] = target.position;
                    }
                    else
                    {
                        positions[i] = positions[i + 1] + (positions[i] - positions[i + 1]).normalized * bonesLength[i];
                    }
                }

                //Forward.
                for (int i = 1; i < positions.Length; i++)
                    positions[i] = positions[i - 1] + (positions[i] - positions[i - 1]).normalized * bonesLength[i - 1];

                //If close enough, stop iterating.
                if ((positions[bones.Length - 1] - target.position).sqrMagnitude < delta * delta)
                    break;
            }

            //Bend towards the pole, if any.
            if (pole != null)
                BendTowardsPole();

            //Set the propper rotation & position to bones.
            ApplyCalculationsToBones();

            void BendTowardsPole()
            {
                for (int i = 1; i < bones.Length - 1; i++)
                {
                    Plane plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
                    Vector3 projectedPole = plane.ClosestPointOnPlane(pole.position);
                    Vector3 projectedBone = plane.ClosestPointOnPlane(positions[i]);
                    float angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1], plane.normal);
                    positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) + positions[i - 1];
                }
            }
            void ApplyCalculationsToBones()
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    //Rotation.
                    if (rotation)
                    {
                        if (i == positions.Length - 1)
                            bones[i].rotation = target.rotation * Quaternion.Inverse(startRotationTarget) * startRotationBone[i];
                        else
                            bones[i].rotation = Quaternion.FromToRotation(startDirectionSucc[i], positions[i + 1] - positions[i]) * startRotationBone[i];
                    }

                    //Position.
                    bones[i].position = positions[i];
                }
            }
        }
    }

    public bool ReachableByArm(Item item)
    {
        Transform player = LevelManager.Instance.PlayerInteractions.transform;
        bool isRightArm = bones[0].localPosition.x > 0;

        Vector3 fromPlayerToItem = item.transform.position - player.position;
        Debug.DrawLine(player.position, player.position + fromPlayerToItem, Color.yellow);

        float armAngle = 0;
        float frontAngle = Vector3.Angle(player.forward, fromPlayerToItem);

        armAngle = isRightArm ? Vector3.Angle(Quaternion.Euler(0, -armRangeOffset, 0) * player.right, fromPlayerToItem)
                            : Vector3.Angle(Quaternion.Euler(0, armRangeOffset, 0) * -player.right, fromPlayerToItem);

        return armAngle < maxTargetAngle / 2 && frontAngle < maxTargetAngle / 2;
    }

    public bool ItemFocused() => Vector3.Distance(target.position, defaultTarget.position) >= 0.1f;

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Transform current = transform;
        for (int i = 0; i < chainLength && current != null && current.parent != null; i++)
        {
            float scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.parent;
        }
#endif
    }
}
