using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Button Action")]
    [SerializeField] private UnityEvent<ItemBase> _ClickHandler;


    private Color _basicColor;
    private Color _hoverColor;
    private Color _textBasicColor;
    private Color _textHoverColor;

    private Image _objectImage;
    private TextMeshProUGUI _objectText;

    private void Awake()
    {
        _objectImage = GetComponent<Image>();
        _objectText = transform.Find("Text").GetComponent<TextMeshProUGUI>();

        _basicColor = _objectImage.color;
        _textBasicColor = _objectText.color;

        _hoverColor = SetHoverColor(_basicColor);
        _textHoverColor = SetHoverColor(_textBasicColor);
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
            ScreenManager.Instance.SwapToList();
        }
    }
}
