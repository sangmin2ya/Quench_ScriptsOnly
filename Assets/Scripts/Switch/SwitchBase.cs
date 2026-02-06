using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBase : MonoBehaviour, IInteractable
{
    /// <summary>
    /// Item Type of this item
    /// </summary>
    /// 
    [Header("Item Type")]
    [SerializeField] private ItemType _itemType;

    public ItemType ItemType => _itemType;

    private Outline _outLine;

    private void Awake()
    {
        _outLine = GetComponent<Outline>();
    }

    public virtual void OnHover()
    {
        Debug.Log("On Hover");
        if (_outLine == null)
            _outLine = GetComponent<Outline>();
        _outLine.enabled = true;
    }

    public virtual void OnHoverExit()
    {
        Debug.Log("Hover Exit!!");
        _outLine.enabled = false;
    }

    public virtual void OnInteract()
    {
        Debug.Log("Switch: OnInteract");
        //ScreenManager.Instance.EnablePopUp(_itemName, _itemDescription, _itemText, _popUpType, _sourceImage);

        GetComponent<SwitchInteract>()?.OpenShortCut();
    }
}
