using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToUnderWorld : MonoBehaviour, IInteractable
{
    private ItemType itemType;
    public ItemType ItemType => itemType;

    private Outline _outLine;

    private void Awake()
    {
        _outLine = GetComponent<Outline>();
    }
    public void OnHover()
    {
        _outLine.enabled = true;
        ScreenManager.Instance.ShowEnterable();
    }
    public void OnHoverExit()
    {
        _outLine.enabled = false;
        ScreenManager.Instance.HideEnterable();
    }

    public void OnInteract()
    {
        Debug.Log("To UnderWorld");

        DataManager.Instance.SetStartPoint(new Vector3(111.3f, 67.33f, 236.64f));

        SceneManager.LoadScene("UnderGround");
    }
}
