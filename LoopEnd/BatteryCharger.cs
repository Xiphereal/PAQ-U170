using UnityEngine;
using System.Collections;
using System;

public class BatteryCharger : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    private bool isActive;

    private void Awake() => animator.enabled = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive)
        {
            isActive = true;
            animator.enabled = true;
        }
    }
}
