using UnityEngine;
using UnityEngine.InputSystem;
public class ToggleSetting : MonoBehaviour
{
    public GameObject settingCanvas;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettings();
        }
    }

    public void ToggleSettings()
    {
        if (!settingCanvas.activeSelf && DataManager.Instance._canGoSetting)
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerInput>().enabled = false;
            settingCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (settingCanvas.activeSelf)
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerInput>().enabled = true;
            settingCanvas.SetActive(false);
            if (!DataManager.Instance._canGoSetting)
                return;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}