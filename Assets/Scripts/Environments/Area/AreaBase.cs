using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaBase : MonoBehaviour, IEnterable
{
    [SerializeField] private AreaType _areaType;
    public AreaType AreaType => _areaType;
    
    public void OnPlayerEnter()
    {
        Debug.Log("Player entered");
        AreaVisitManager.Instance.VisitArea(_areaType);
    }

    public void OnPlayerExit()
    {
        Debug.Log("Player exited AreaExample.");
        // Add event that will happen when player exits area
    }
}
