using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemBase : MonoBehaviour, IInteractable
{
    /// <summary>
    /// Item Type of this item
    /// </summary>
    /// 
    [Header("Item Type")]
    [SerializeField] private ItemType _itemType;
    [SerializeField] private CollectionType _collectionType = CollectionType.None;
    [SerializeField] private PopUpType _popUpType;

    [Space]

    [Header("Item Image")]
    [SerializeField] private Sprite _sourceImage;

    [Space]

    [Header("Item Name")]
    [TextArea(5, 5)]
    [SerializeField] private string _itemName;

    [Space]

    [Header("Item Description")]
    [TextArea(5, 5)]
    [SerializeField] private string _itemDescription;

    [Header("Item Main Text")]
    [TextArea(20, 10)]
    [SerializeField] private string _itemText;

    [Space]

    [Header("Volatile Item")]
    [SerializeField] private bool _isVolatile;
    [Header("Key Item")]
    [SerializeField] private bool _isKeyItem;

    public ItemType ItemType => _itemType;

    public PopUpType PopUpType => _popUpType;

    public Item ItemInfo => _itemInfo;

    public AreaType ItemArea
    {
        get
        {
            if (Enum.TryParse(gameObject.name.Split('_')[0], out AreaType area))
                return area;
            else return AreaType.None;
        }
    }

    private Outline _outLine;
    private Item _itemInfo;

    private void Awake()
    {
        DataManager.Instance.LanguageChangedEvent += OnLanguageChanged;

        InitializeData();

        if (_itemType != ItemType.Collection)
        {
            _collectionType = CollectionType.None;
        }
        else
        if (DataManager.Instance.IsCollected(_collectionType))
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
        InitializeOutline();
    }
    private void OnLanguageChanged(LanguageType newLanguage)
    {
        InitializeData();
    }
    private void InitializeOutline()
    {
        _outLine = GetComponent<Outline>();
        _outLine.enabled = false;
        //_outLine.OutlineMode = Outline.Mode.OutlineVisible;
        _outLine.OutlineWidth = 10;
        _outLine.OutlineColor = _itemType == ItemType.Clue ? Color.yellow : Color.white;
    }

    private void InitializeData()
    {
        Item item = JsonReader.Instance.itemDB.items.FirstOrDefault(x => x.objectName == gameObject.name);
        if (item == null)
        {
            return;
        }
        gameObject.layer = LayerMask.NameToLayer("Item");
        _itemInfo = item;
        _itemName = item.itemName;
        _itemType = (ItemType)System.Enum.Parse(typeof(ItemType), item.itemType);
        _itemDescription = item.itemDescription;
        _itemText = item.itemText;
        _popUpType = (PopUpType)System.Enum.Parse(typeof(PopUpType), item.popUpType);
        _sourceImage = Resources.Load<Sprite>($"Images/Items/{item.sourceImage}");
        GetComponent<Outline>().outlineMode = Outline.Mode.OutlineVisible;
    }

    public virtual void OnHover()
    {
        //Debug.Log("On Hover");
        if (_outLine == null)
            _outLine = GetComponent<Outline>();
        _outLine.enabled = true;

        ScreenManager.Instance.ShowInteractable();
    }

    public virtual void OnHoverExit()
    {
        Debug.Log("Hover Exit!!");

        if (this == null || gameObject == null)
        {
            return;
        }

        _outLine.enabled = false;
        ScreenManager.Instance.HideInteractable();
    }

    public virtual void OnInteract()
    {
        Debug.Log("ExampleItem: OnInteract");

        ScreenManager.Instance.EnablePopUp(_itemName, _itemDescription, _itemText, _popUpType, _sourceImage);

        if (_isVolatile)
            Destroy(gameObject.transform.parent.gameObject);

        DataManager.Instance.CollectItem(_collectionType);

        GetComponent<ObeliskBase>()?.ReadObelisk();
        GetComponent<LightSwitch>()?.TurnonLights();
        GetComponent<IsGemstoneAlive>()?.GetGemStone();

        if (_isKeyItem) DataManager.Instance.GetKeyItem(this);
    }

    void OnDestroy()
    {
        DataManager.Instance.LanguageChangedEvent -= OnLanguageChanged;
        ScreenManager.Instance.HideInteractable();
        if (gameObject == null) return;
        if (gameObject.transform.parent == null) return;
        if (gameObject.transform.parent.gameObject == null) return;
        Destroy(gameObject.transform.parent.gameObject);
    }
}