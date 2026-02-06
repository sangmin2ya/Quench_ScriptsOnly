using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManageList : MonoBehaviour
{
    [SerializeField] private List<GameObject> _buttons;

    private Dictionary<GameObject, Item> _buttonItem;
    private List<ItemBase> _items;

    public void AlignButtons(int count)
    {
        foreach (var button in _buttons)
        {
            button.SetActive(false);
        }

        float blockSize = 100f;
        float fullSize = 620f;

        float fullMargin = fullSize - blockSize * count;

        float margin = fullMargin / (float)(count + 1);

        float currentY = 130f;

        for (int i = 0; i < count; i++)
        {
            currentY -= margin;
            _buttons[i].SetActive(true);
            _buttons[i].GetComponent<RectTransform>().anchoredPosition =
                new Vector3(0, currentY, transform.position.z);
            _buttons[i].transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "???";
            currentY -= blockSize;
        }
    }

    public void BindItems(List<ItemBase> items)
    {
        _items = new List<ItemBase>();

        _items = items;

        AlignButtons(_items.Count);

        _buttonItem = new Dictionary<GameObject, Item>();

        int cnt = 0;

        foreach (ItemBase item in _items)
        {
            if (DataManager.Instance.IsKeyItemContained(item))
            {
                _buttonItem.Add(_buttons[cnt], item.ItemInfo);
                _buttons[cnt].transform.Find("Text").
                    GetComponent<TextMeshProUGUI>().text = item.ItemInfo.itemName;

                _buttons[cnt].GetComponent<ClickPanel>()
                    .BindHandler(item, ScreenManager.Instance.SwapToDetails);
            }

            cnt++;
        }
    }
}
