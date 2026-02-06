using System;
using UnityEngine;

[Serializable]
public class Item
{
    [SerializeField]
    public string objectName;
    [SerializeField]
    public string itemName;
    [SerializeField]
    public string itemType;
    [SerializeField]
    public string popUpType;
    [SerializeField]
    public string sourceImage;
    [SerializeField]
    public string itemDescription;
    [SerializeField]
    public string itemText;
    [SerializeField]
    public bool isVolatile;
}
