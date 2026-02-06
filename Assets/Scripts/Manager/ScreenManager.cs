using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;


public class ScreenManager : MonoBehaviour
{
    #region Global Interfaces

    private static ScreenManager _instance;

    public static ScreenManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject screenManagerObject = new GameObject("ScreenManager");
                _instance = screenManagerObject.AddComponent<ScreenManager>();

                DontDestroyOnLoad(screenManagerObject);

                _instance.Initialize();
            }

            return _instance;
        }
    }

    public bool IsPaused { get { return _isPaused; } }

    #endregion

    #region Local variables

    private PostProcessVolume _postProc;
    private GameObject _popUpMenu;
    private GameObject _listedPopUpMenu;
    private GameObject _crossHair;

    private bool _isTransition = false;
    private bool _isPaused = false;

    private PopUpType _popUpType;

    // Normal PopUp Miscs
    private Sprite _sourceImage;
    private Sprite _listSourceImage;

    // Listed PopUp Miscs
    private List<ItemBase> _items;

    // interactable
    private GameObject _interactGuide;
    private GameObject _enterableGuide;

    private string _viewMode;

    #endregion

    #region Initialize

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            DontDestroyOnLoad(_instance);

            Initialize();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        // �ʱ�ȭ
        _postProc = Camera.main.GetComponent<PostProcessVolume>();
        _popUpMenu = GameObject.Find("Canvas")?.transform.Find("ItemPopUp")?.gameObject;
        _listedPopUpMenu = GameObject.Find("Canvas")?.transform.Find("ListedPopUp")?.gameObject;
        if (_interactGuide == null)
        {
            _interactGuide = GameObject.Find("InteractGuide").gameObject;
            _interactGuide.SetActive(false);
        }
        if (_enterableGuide == null)
        {
            _enterableGuide = GameObject.Find("EnterableGuide").gameObject;
            _enterableGuide.SetActive(false);
        }
        _popUpType = PopUpType.None;
        _sourceImage = null;
        _listSourceImage = null;
        _viewMode = "Normal";

        if (_popUpMenu == null)
        {
            Debug.LogError("[ScreenManager.cs/Initialize()] Unable to Load PopUpPanel");
        }


        _crossHair = GameObject.Find("Canvas").transform.Find("CrossHair").gameObject;
    }

    #endregion

    private void Update()
    {
        if (_postProc == null)
        {
            Initialize();
        }

        if (_isTransition) return;

        GetInput();
    }

    #region Popup Management

    /// <summary>
    /// Popup Item Description
    /// </summary>
    /// <param name="name">name of Item</param>
    /// <param name="description">description of Name</param>
    /// <param name="mainText">mainText of </param>
    public void EnablePopUp(string name, string description, string mainText, PopUpType pType, Sprite source)
    {
        if (_isTransition) return;

        _viewMode = "Normal";
        _popUpMenu.SetActive(true);

        // Set Types
        _popUpType = pType;
        _sourceImage = source;

        // Bind Values
        SetMessage(name, description, mainText);
        LocateImage(pType);

        // Status Processing
        StartCoroutine(EnablingAnimation());
        _crossHair.gameObject.SetActive(false);
        PauseGame();
    }

    public void DisablePopUp()
    {
        if (_isTransition) return;

        StartCoroutine(DisablingAnimation());
        _crossHair.gameObject.SetActive(true);
        ResumeGame();
    }

    public void SetMessage(string name, string description, string mainText)
    {
        Transform popUp = _popUpMenu.transform.Find(_popUpType.ToString());

        switch (_popUpType)
        {
            case PopUpType.None:

                break;

            case PopUpType.Normal:
                popUp.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = name;
                popUp.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = description;
                popUp.transform.Find("MainText").GetComponent<TextMeshProUGUI>().text = mainText;
                break;

            case PopUpType.Book:
                popUp.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = name;
                popUp.transform.Find("MainText").GetComponent<TextMeshProUGUI>().text = mainText;
                break;

            case PopUpType.List:

                break;
        }
    }

    #endregion

    #region Listed PopUp Management

    public void EnableListedPopUp(string areaName, string areaDescription, Sprite source,
    List<ItemBase> areaItems)
    {
        if (_isTransition) return;

        _listedPopUpMenu.SetActive(true);
        SwapToList();
        _viewMode = "Listed";

        // Set Types
        _sourceImage = source;

        // Bind Values
        SetListedPopUp(areaName, areaDescription, source, areaItems);

        // Status Processing
        StartCoroutine(EnablingAnimation());
        _crossHair.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PauseGame();
    }

    public void SetListedPopUp(string areaName, string areaDescription, Sprite icon,
        List<ItemBase> areaItems)
    {
        Transform holder = _listedPopUpMenu.transform.Find("List");
        holder.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = areaName;

        holder.Find("Icon").GetComponent<Image>().sprite = icon;

        // Description Initialize Not done
        _items = areaItems;

        holder.GetComponent<ManageList>().BindItems(_items);
    }

    public void DisableListedPopUp()
    {
        if (_isTransition) return;

        StartCoroutine(DisablingAnimation());
        _crossHair.gameObject.SetActive(true);
        ResumeGame();
    }

    public void SwapToDetails(ItemBase item)
    {
        Transform detailTransform = _listedPopUpMenu.transform.Find("Detail").transform;

        detailTransform.gameObject.SetActive(true);
        PopUpType pType = (PopUpType)System.Enum.Parse(typeof(PopUpType), item.ItemInfo.popUpType);

        Transform normal = detailTransform.Find("Normal");
        Transform book = detailTransform.Find("Book");

        if (pType == PopUpType.Normal)
        {
            normal.Find("Name").GetComponent<TextMeshProUGUI>().text = item.ItemInfo.itemName;
            normal.Find("Description").GetComponent<TextMeshProUGUI>().text = item.ItemInfo.itemDescription;
            normal.Find("MainText").GetComponent<TextMeshProUGUI>().text = item.ItemInfo.itemText;
            _listSourceImage = Resources.Load<Sprite>($"Images/Items/{item.ItemInfo.sourceImage}");

            normal.gameObject.SetActive(true);
            book.gameObject.SetActive(false);
        }
        else if (pType == PopUpType.Book)
        {
            book.Find("Name").GetComponent<TextMeshProUGUI>().text = item.ItemInfo.itemName;
            book.Find("MainText").GetComponent<TextMeshProUGUI>().text = item.ItemInfo.itemText;
            _listSourceImage = Resources.Load<Sprite>($"Images/Items/{item.ItemInfo.sourceImage}");

            book.gameObject.SetActive(true);
            normal.gameObject.SetActive(false);
        }

        LocateListedImage(pType);

        _listedPopUpMenu.transform.Find("List").gameObject.SetActive(false);
    }

    public void SwapToList()
    {
        _listedPopUpMenu.transform.Find("Detail").gameObject.SetActive(false);
        _listedPopUpMenu.transform.Find("List").gameObject.SetActive(true);
    }

    #endregion

    #region UI Utilities
    private void DispatchAlpha(GameObject go, float alpha)
    {
        Transform popUp = _viewMode switch
        {
            "Normal" => go.transform.Find(_popUpType.ToString()),
            "Listed" => go.transform,
            _ => null
        };

        Color goColor = go.GetComponent<Image>().color;
        goColor.a = alpha * 0.6f;
        go.GetComponent<Image>().color = goColor;

        Dispatch(popUp, alpha);
    }

    private void Dispatch(Transform parent, float alpha)
    {
        foreach (Transform t in parent)
        {
            TextMeshProUGUI tmp = t.GetComponent<TextMeshProUGUI>();
            Image img = t.GetComponent<Image>();

            Color color;

            if (tmp != null)
            {
                color = tmp.color;
                color.a = alpha;
                tmp.color = color;
            }
            else if (img != null)
            {
                color = img.color;
                color.a = alpha;
                img.color = color;
            }

            Dispatch(t, alpha);
        }
    }

    private void LocateImage(PopUpType pType)
    {
        Image itemImage = null;
        Sprite sourceImage = null;

        Vector3 position = Vector3.zero;
        Vector2 maxSize = Vector2.zero;

        switch (pType)
        {
            case PopUpType.Normal:
                itemImage = _popUpMenu.transform.Find("Normal").transform.Find("Image_Item").GetComponent<Image>();
                position.x = -550f;
                position.y = -18f;
                maxSize = new Vector2(580f, 1000f);
                break;

            case PopUpType.Book:
                itemImage = _popUpMenu.transform.Find("Book").transform.Find("Image_Item").GetComponent<Image>();
                position.x = 25f;
                position.y = 216f;
                maxSize = new Vector2(940f, 630f);
                break;

            case PopUpType.None:

                break;
        }

        itemImage.sprite = _sourceImage;
        sourceImage = itemImage.sprite;

        float sourceWidth = sourceImage.rect.width;
        float sourceHeight = sourceImage.rect.height;

        float ratioX = maxSize.x / sourceWidth;
        float ratioY = maxSize.y / sourceHeight;

        float ratio = ratioX < ratioY ? ratioX : ratioY;

        itemImage.rectTransform.sizeDelta = new Vector2(sourceWidth * ratio, sourceHeight * ratio);
        itemImage.rectTransform.anchoredPosition = position;
    }

    private void LocateListedImage(PopUpType pType)
    {
        Image itemImage = null;
        Sprite sourceImage = null;

        Vector3 position = Vector3.zero;
        Vector2 maxSize = Vector2.zero;

        GameObject detailObject = _listedPopUpMenu.transform.Find("Detail").gameObject;

        switch (pType)
        {
            case PopUpType.Normal:
                itemImage = detailObject.transform.Find("Normal").transform.Find("Image_Item").GetComponent<Image>();
                position.x = -550f;
                position.y = -18f;
                maxSize = new Vector2(580f, 1000f);
                break;

            case PopUpType.Book:
                itemImage = detailObject.transform.Find("Book").transform.Find("Image_Item").GetComponent<Image>();
                position.x = 25f;
                position.y = 216f;
                maxSize = new Vector2(940f, 630f);
                break;

            case PopUpType.List:

                break;

            case PopUpType.None:

                break;
        }

        itemImage.sprite = _listSourceImage;
        sourceImage = itemImage.sprite;

        float sourceWidth = sourceImage.rect.width;
        float sourceHeight = sourceImage.rect.height;

        float ratioX = maxSize.x / sourceWidth;
        float ratioY = maxSize.y / sourceHeight;

        float ratio = ratioX < ratioY ? ratioX : ratioY;

        itemImage.rectTransform.sizeDelta = new Vector2(sourceWidth * ratio, sourceHeight * ratio);
        itemImage.rectTransform.anchoredPosition = position;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        _isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        _isPaused = false;


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private IEnumerator EnablingAnimation()
    {
        _isTransition = true;

        float currentTime = 0f;
        float duration = 0.5f;

        while (currentTime < duration)
        {
            yield return null;
            currentTime += Time.unscaledDeltaTime;

            float ratio = currentTime / duration;

            switch (_viewMode)
            {
                case "Normal":
                    DispatchAlpha(_popUpMenu, ratio);
                    break;

                case "Listed":
                    DispatchAlpha(_listedPopUpMenu, ratio);
                    break;
            }

            _postProc.weight = ratio;
        }

        _isTransition = false;
    }

    private IEnumerator DisablingAnimation()
    {
        _isTransition = true;

        float currentTime = 0f;
        float duration = 0.5f;

        while (currentTime < duration)
        {
            yield return null;
            currentTime += Time.unscaledDeltaTime;

            float ratio = 1 - currentTime / duration;

            switch (_viewMode)
            {
                case "Normal":
                    DispatchAlpha(_popUpMenu, ratio);
                    break;

                case "Listed":
                    DispatchAlpha(_listedPopUpMenu, ratio);
                    break;
            }

            _postProc.weight = ratio;
        }


        switch (_viewMode)
        {
            case "Normal":
                _popUpMenu.SetActive(false);
                break;

            case "Listed":
                _listedPopUpMenu.SetActive(false);

                break;
        }

        _isTransition = false;
    }

    private void GetInput()
    {
        //if (_viewMode == "Normal")
        //{
        //    if (Input.GetKeyDown(KeyCode.E) && _isPaused)
        //    {
        //        Debug.Log("End PopUp");

        //        DisablePopUp();
        //    }
        //}
        //else if (_viewMode == "Listed")
        //{
        //    if (Input.GetKeyDown(KeyCode.E) && _isPaused)
        //    {
        //        Debug.Log("End PopUp");

        //        DisablePopUp();
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.E) && _isPaused)
        {
            switch (_viewMode)
            {
                case "Normal":
                    DisablePopUp();
                    break;

                case "Listed":
                    DisableListedPopUp();
                    break;
            }
        }
        if (_viewMode == "Listed" && _isPaused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
    }

    public void ShowInteractable()
    {
        if (_interactGuide == null) return;

        _interactGuide.SetActive(true);
    }
    public void HideInteractable()
    {
        if (_interactGuide == null) return;

        _interactGuide.SetActive(false);
    }
    public void ShowEnterable()
    {
        if (_enterableGuide == null) return;

        _enterableGuide.SetActive(true);
    }
    public void HideEnterable()
    {
        if (_enterableGuide == null) return;

        _enterableGuide.SetActive(false);
    }

    #endregion
}
