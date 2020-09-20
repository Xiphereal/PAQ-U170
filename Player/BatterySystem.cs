using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class BatterySystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Light statusLight;
    [SerializeField]
    private MalfunctionBehavior malfunctionBehavior;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite fullBattery;
    [SerializeField]
    private Sprite halfBattery;
    [SerializeField]
    private Sprite lowBattery;

    public enum States
    {
        FullBattery,
        HalfBattery,
        LowBattery,
        EmptyBattery
    }

    [Header("Configuration")]
    private static States state;
    public static States State { set => state = value; }

    [SerializeField]
    private Color green;
    [SerializeField]
    private Color red;

    void Start()
    {
        ResolveDependencies();
        AssertDependenciesAreResolved();

        animator.enabled = false;

        ManageBatteryStatus();

        void ResolveDependencies()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        void AssertDependenciesAreResolved()
        {
            Assert.IsNotNull(statusLight, "Battery status light reference is missing.");
            Assert.IsNotNull(halfBattery, "Low battery sprite reference is missing.");
            Assert.IsNotNull(halfBattery, "Half battery sprite reference is missing.");
            Assert.IsNotNull(fullBattery, "Full battery sprite reference is missing.");
            Assert.IsNotNull(malfunctionBehavior, "Malfunction behaviour reference is missing.");
        }
        void ManageBatteryStatus()
        {
            switch (state)
            {
                case States.FullBattery:
                    SetBatteryToFull();
                    break;

                case States.HalfBattery:
                    SetBatteryToHalf();
                    break;

                case States.LowBattery:
                    SetBatteryToLow();
                    break;

                case States.EmptyBattery:
                    SetBatteryToEmpty();
                    break;

                default:
                    throw new System.InvalidOperationException($"The battery state in {name} for {state} is not defined");
            }
        }
    }

    [ContextMenu("SetBatteryToFull")]
    public void SetBatteryToFull()
    {
        state = States.FullBattery;
        SetStatusLightTo(green);
        SetBatterySpriteTo(fullBattery);
    }

    [ContextMenu("SetBatteryToHalf")]
    public void SetBatteryToHalf()
    {
        state = States.HalfBattery;
        SetStatusLightTo(green);
        SetBatterySpriteTo(halfBattery);
    }

    [ContextMenu("SetBatteryToLow")]
    public void SetBatteryToLow()
    {
        state = States.LowBattery;
        SetStatusLightTo(red);
        SetBatterySpriteTo(lowBattery);
    }

    [ContextMenu("SetBatteryToEmpty")]
    public void SetBatteryToEmpty()
    {
        state = States.EmptyBattery;
        SetStatusLightTo(red);

        animator.enabled = true;
        malfunctionBehavior.enabled = true;
    }

    private void SetStatusLightTo(Color color) => statusLight.color = color;

    private void SetBatterySpriteTo(Sprite sprite) => spriteRenderer.sprite = sprite;
}
