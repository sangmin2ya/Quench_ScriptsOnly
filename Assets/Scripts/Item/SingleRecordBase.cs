
using UnityEngine;

public class SingleRecordBase : MonoBehaviour, IInteractable
{
    /// <summary>
    /// Item Type of this item
    /// </summary>
    /// 
    [Header("Item Type")]
    [SerializeField] private ItemType _itemType;
    [SerializeField] private PopUpType _popUpType;

    [Space]

    [Header("Image")]
    [SerializeField] private Sprite _source;

    [Space]

    [Header("Name")]
    [TextArea(5, 5)]
    [SerializeField] private string _name;

    [Space]

    [Header("Description")]
    [TextArea(5, 5)]
    [SerializeField] private string _description;


    [Space]

    [Header("Description")]
    [TextArea(5, 10)]
    [SerializeField] private string _mainText;

    [Space]
    [Header("Obelisk")]
    [SerializeField] private ItemBase _obeliskObject;

    public ItemType ItemType => _itemType;

    public PopUpType PopUpType => _popUpType;

    private void Awake()
    {
        transform.Find("Check").gameObject.SetActive(false);
    }

    public virtual void OnHover()
    {
        transform.Find("Check").gameObject.SetActive(true);
        ScreenManager.Instance.ShowInteractable(); 
    }

    public virtual void OnHoverExit()
    {

        transform.Find("Check").gameObject.SetActive(false);
        ScreenManager.Instance.HideInteractable();
    }

    public virtual void OnInteract()
    {
        ScreenManager.Instance.
            EnablePopUp(
            _obeliskObject.ItemInfo.itemName,
            _obeliskObject.ItemInfo.itemDescription,
            _obeliskObject.ItemInfo.itemText,
            (PopUpType)System.Enum.Parse(typeof(PopUpType), _obeliskObject.ItemInfo.popUpType),
            _source
            );
    }
}