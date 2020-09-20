using UnityEngine;
using System.Collections;

public class PlayerLightingSwitch : MonoBehaviour
{
    [SerializeField]
    private bool switchOnLights;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            other.GetComponent<LightingSystem>().SwitchLights(switchOnLights);
    }
}
