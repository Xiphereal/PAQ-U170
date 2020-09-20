using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public PlayerMovement PlayerMovement { get; set; }
    public PlayerInteractions PlayerInteractions { get; set; }

    [SerializeField]
    private BatterySystem batterySystem;
    [SerializeField]
    private BonusLog bonusLog;

    public List<Item> Items { get; private set; } = new List<Item>();

    private void Awake()
    {
        EnsureSingleton();

        void EnsureSingleton()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }
    }

    public void AddItemInstance(Item instance) => Items.Add(instance);

    public void RemoveItemInstance(Item instance) => Items.Remove(instance);

    public void MovePlayerTo(Vector3 point) => PlayerMovement.MoveTo(point);

    public void MovePlayerTowards(Vector3 target, float requiredDistance = 7f)
    {
        StartCoroutine(PlayerMovement.MoveTowardsPoint(target, requiredDistance));
    }

    public GameObject GetHoldItemPrefab()
    {
        GameObject holdItem = GameManager.Instance.HoldItem;
        if (holdItem != null)
            return Instantiate(GameManager.Instance.HoldItem, PlayerInteractions.transform.position + Vector3.forward, Quaternion.identity);

        return null;
    }

    public void FirstTaskCompleted() => PlayTaskAcomplishedSound();

    public void SecondTaskCompleted() => PlayTaskAcomplishedSound();

    private void PlayTaskAcomplishedSound() => AudioManager.Instance.Play("TaskAcomplished");

    public void ThirdTaskCompleted()
    {
        batterySystem.SetBatteryToHalf();
        PlayTaskAcomplishedSound();
    }

    public void FourthTaskCompleted()
    {
        batterySystem.SetBatteryToLow();
        PlayTaskAcomplishedSound();
    }

    [ContextMenu("Complete Fifth Task")]
    public void FifthTaskCompleted()
    {
        batterySystem.SetBatteryToEmpty();
        PlayTaskAcomplishedSound();
        bonusLog.TryShowLog();
    }

    public void ResetLoop()
    {
        batterySystem.SetBatteryToFull();
        ScenePersistence.Instance.ResetPersistence();
    }
}
