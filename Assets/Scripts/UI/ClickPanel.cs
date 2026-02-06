using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ClickPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Button Action")]
    [SerializeField] private UnityEvent<ItemBase> _ClickHandler;
    private Color _basicColor = new Color(1f, 1f, 1f, 1f);
    private Color _hoverColor = new Color(1f, 1f, 1f, 0.3f);
    private Color _textBasicColor = new Color(1f, 1f, 1f, 1f);
    private Color _textHoverColor = new Color(1f, 1f, 1f, 0.3f);
    private Image _objectImage;
    private TextMeshProUGUI _objectText;
    private ItemBase _itemInfo;
    private void Awake()
    {
        _objectImage = GetComponent<Image>();
        _objectText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        _itemInfo = null;
    }
    private Color SetHoverColor(Color co)
    {
        Color color = co;
        color.a = 0.3f;
        return color;
    }
    public void CallByButton()
    {
        Debug.Log("Butan");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_objectImage != null)
        {
            _objectImage.color = _hoverColor;
            _objectText.color = _textHoverColor;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_objectImage != null)
        {
            _objectImage.color = _basicColor;
            _objectText.color = _textBasicColor;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_objectImage != null)
        {
            _objectImage.color = _basicColor;
            _objectText.color = _textBasicColor;
            _ClickHandler.Invoke(_itemInfo);
        }
    }
    public void BindHandler(ItemBase itemBase, UnityAction<ItemBase> handler)
    {
        _itemInfo = itemBase;
        _ClickHandler.AddListener(handler);
    }
}