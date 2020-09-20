using UnityEngine;
using System.Collections;

public class BonusLog : Log
{
    [SerializeField]
    protected const int TotalNumberOfLogs = 6;

    public void TryShowLog()
    {
        if (numberOfDiscoveredLogs >= TotalNumberOfLogs)
        {
            OpenLog();
            enabled = true;
        }
    }

    protected new void Update()
    {
        if (logUI.activeInHierarchy && Input.GetButtonDown("Fire1"))
        {
            CloseLog();
            enabled = false;
        }
    }
}
