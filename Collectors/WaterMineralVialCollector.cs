using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class WaterMineralVialCollector : MonoBehaviour, IPersistable
{
    [SerializeField]
    private GameObject water;

    private WaterPump[] waterPumps;

    [SerializeField]
    private Color[] colorOrder;

    private List<int> pumpsOrder = new List<int>();
    private int nextPump = 0;
    private bool placedAlready;

    private static bool isStatePersisted;

    private Animator animator;

    private void Awake()
    {
        int pumpsQuantity = colorOrder.Length;
        waterPumps = new WaterPump[pumpsQuantity];

        RandomizeOrder();

        animator = water.GetComponent<Animator>();

        void RandomizeOrder()
        {
            List<int> pumpsIds = new List<int>();

            for (int i = 0; i < pumpsQuantity; i++)
                pumpsIds.Add(i);

            for (int i = 0; i < pumpsQuantity; i++)
                pumpsOrder.Add(GetRandomPumpId());

            int GetRandomPumpId()
            {
                int randomId = pumpsIds[UnityEngine.Random.Range(0, pumpsIds.Count - 1)];
                pumpsIds.Remove(randomId);

                return randomId;
            }
        }
    }

    void Start()
    {
        Assert.IsNotNull(water);

        ReferenceWaterPumps();

        void ReferenceWaterPumps()
        {
            WaterPump[] retrievedPumps = gameObject.transform.parent.GetComponentsInChildren<WaterPump>();

            foreach (WaterPump pump in retrievedPumps)
            {
                pump.Collector = this;
                pump.DefaultMaterialColor = GetCorrespondingColorToStayInOrder(pump);

                waterPumps[pump.Id] = pump;
            }

            Color GetCorrespondingColorToStayInOrder(WaterPump pump) => colorOrder[pumpsOrder.IndexOf(pump.Id)];
        }
    }

    public bool PumpActivated(int index)
    {
        if (IsPumpInCorrectOrder())
        {
            CheckPumpSystemCompletion();
            return true;
        }
        else
        {
            ResetAllPumps();
            return false;
        }

        bool IsPumpInCorrectOrder() => pumpsOrder[nextPump++] == index;
        void CheckPumpSystemCompletion()
        {
            if (waterPumps.ToList().All(pump => pump.IsActive))
                TaskCompleted();

            void TaskCompleted()
            {
                LowerWaterCourse();
                AudioManager.Instance.Play("Watercourse");
                LevelManager.Instance.FourthTaskCompleted();
                isStatePersisted = true;

                void LowerWaterCourse() => animator.Play("withdrawWatercourse");
            }
        }
        void ResetAllPumps()
        {
            foreach (WaterPump waterPump in waterPumps)
                waterPump.ResetPump();

            nextPump = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Vial vial = LevelManager.Instance.PlayerInteractions.CarriedObject as Vial;

            if (vial != null && !placedAlready)
            {
                placedAlready = true;
                vial.ObjectPlaced();
                EnablePumpSystem();
            }
        }

        void EnablePumpSystem()
        {
            for (int i = 0; i < waterPumps.Length; i++)
                waterPumps[i].enabled = true;
        }
    }

    public void PersistState() { }

    public void ResetState() => isStatePersisted = false;

    public void RestoreState()
    {
        if (isStatePersisted)
        {
            animator.Play("withdrawWatercourseInstantly");

            foreach (var pump in waterPumps)
                pump.SetActive();
        }
    }
}
