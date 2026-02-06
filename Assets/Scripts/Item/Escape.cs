using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class Escape : MonoBehaviour, IInteractable
{
    /// <summary>
    /// Item Type of this item
    /// </summary>
    /// 
    [Header("Item Type")]
    [SerializeField] private ItemType _itemType;
    [SerializeField] private PopUpType _popUpType;
    [SerializeField] private GameObject _escapeCanvas;

    public ItemType ItemType => _itemType;
    public GameObject _dollCam;
    private Outline _outLine;

    private void Awake()
    {
        _outLine = GetComponent<Outline>();
    }

    public virtual void OnHover()
    {
        //Debug.Log("On Hover");
        _outLine.enabled = true;
    }

    public virtual void OnHoverExit()
    {
        Debug.Log("Hover Exit!!");

        if (this == null || gameObject == null)
        {
            return;
        }

        _outLine.enabled = false;
    }

    public virtual void OnInteract()
    {
        transform.Find("Fire").gameObject.SetActive(true);
        GameObject.Find("PlayerCapsule").GetComponent<FirstPersonController>().enabled = false;
        GameObject.Find("PlayerCapsule").GetComponent<PlayerThirstController>().enabled = false;
        _escapeCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        DataManager.Instance._canGoSetting = false;
    }
    public void StayGame()
    {
        Debug.Log("Stay Desert");
        _escapeCanvas.SetActive(false);
        GameObject.Find("PlayerCapsule").GetComponent<FirstPersonController>().enabled = true;
        GameObject.Find("PlayerCapsule").GetComponent<PlayerThirstController>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //SceneManager.LoadScene("Desert");
        DataManager.Instance._canGoSetting = true;
    }
    public void EscapeGame()
    {
        GameObject.Find("Canvas/Ending/Button").GetComponent<Button>().interactable = true;
        GameObject.Find("Canvas/Ending/Button1").GetComponent<Button>().interactable = true;
        _escapeCanvas.SetActive(false);
        Debug.Log("Escape Desert");
        _dollCam.SetActive(true);
        DataManager.Instance._canGoSetting = false;
        //SceneManager.LoadScene("Main");
    }
}