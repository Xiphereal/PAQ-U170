using UnityEngine;
using UnityEngine.Assertions;

public class AntennaFeedback : MonoBehaviour
{
    private AntennaSystem antennaSystem;

    [SerializeField]
    private GameObject antennaBase;

    [Header("Parameters")]
    [SerializeField]
    private Color[] colors;

    [SerializeField]
    private float minDistance;
    [SerializeField]
    private float defaultRate;

    private int colorCounter;
    private float colorTimer;

    void Start()
    {
        antennaSystem = GetComponent<AntennaSystem>();

        Assert.IsNotNull(antennaBase);
        Assert.AreNotEqual(defaultRate, 0, "There must be some blinking rate for the AntennaFeedback");
        Assert.AreNotEqual(minDistance, 0, "There must be some minDistance for the AntennaFeedback");

        enabled = false;
    }

    void Update()
    {
        CheckAntennaDistance();

        void CheckAntennaDistance()
        {
            float distance = antennaSystem.GetDistanceFromAntennaToSource();

            colorTimer++;

            if (distance <= minDistance)
            {
                if (colorTimer / distance >= defaultRate)
                {
                    Color color = UpdateColor();

                    if (IsFinalColor(color))
                        AudioManager.Instance.Play("AntennaBeep");

                    colorTimer = 0;
                }
            }
            else
                ResetColor();

            Color UpdateColor()
            {
                return antennaBase.GetComponent<Renderer>().material.color = colors[colorCounter++ % colors.Length];
            }
            bool IsFinalColor(Color color) => color == colors[colors.Length - 1];
        }
    }

    public void ResetColor() => antennaBase.GetComponent<Renderer>().material.color = colors[0];

    public void FinalColor() => antennaBase.GetComponent<Renderer>().material.color = colors[colors.Length - 1];
}
