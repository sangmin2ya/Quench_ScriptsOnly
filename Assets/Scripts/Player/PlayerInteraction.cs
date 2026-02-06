using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteraction : MonoBehaviour
{
    public float _interactionDistance = 2f;
    public LayerMask _interactableLayer;
    public KeyCode _interactKey = KeyCode.E;

    // Maze Shortcut Switch
    public float _interactionDistance_Switch;
    public LayerMask _interactableLayer_Switch;
    //

    private IInteractable _lastInteracted = null;

    void Update()
    {
        InteractItem();
        //InteractSwitch();
    }

    private void InteractItem()
    {
        if (ScreenManager.Instance.IsPaused) return;

        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, _interactionDistance, _interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (hit.collider.gameObject.GetComponent<SwitchBase>() != null) // if is not switch, get out from this state
            {
                if (interactable != null)
                {
                    if (_lastInteracted != null && _lastInteracted != interactable)
                    {
                        _lastInteracted.OnHoverExit();
                        _lastInteracted = null;
                    }
                    else
                        interactable.OnHover();

                    if (Input.GetKeyDown(_interactKey))
                    {
                        Debug.Log($"Interacting with {interactable.ItemType}");
                        if (SceneManager.GetActiveScene().name == "Main")
                        {
                            if (hit.collider.gameObject.name == "Village_HiddenEntrance")
                                DataManager.Instance.SetStartPoint(new Vector3(-360.16f, 79.5f, 57.97f)); // village hidden entrance
                            else
                                DataManager.Instance.SetStartPoint(new Vector3(510.63f, 71.17f, 349.67f)); // another hidden entrance
                        }
                        else // Underground
                        {
                            if (hit.collider.gameObject.name == "Maze_Ladder")
                            {
                                DataManager.Instance.SetStartPoint(new Vector3(-133.6f, 1.3f, 493.01f)); // village hidden entrance
                                DataManager.Instance._goUnderground = true;
                            }
                        }
                        interactable.OnInteract();
                    }

                    _lastInteracted = interactable;
                }
            }
            else
            {
                if (interactable != null)
                {
                    if (_lastInteracted != null && _lastInteracted != interactable)
                    {
                        _lastInteracted.OnHoverExit();
                        _lastInteracted = null;
                    }
                    else
                        interactable.OnHover();

                    if (Input.GetKeyDown(_interactKey))
                    {
                        Debug.Log($"Interacting with {interactable.ItemType}");
                        interactable.OnInteract();
                    }

                    _lastInteracted = interactable;
                }
            }
        }
        else
        {
            if (_lastInteracted != null)
            {
                _lastInteracted.OnHoverExit();
                _lastInteracted = null;
            }
        }
    }

    // private void InteractSwitch()
    // {
    //     Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
    //     Ray ray = Camera.main.ScreenPointToRay(screenCenter);
    //     RaycastHit hit;
    // 
    //     if (Physics.Raycast(ray, out hit, _interactionDistance_Switch, _interactableLayer_Switch))
    //     {
    //         IInteractable interactable = hit.collider.GetComponent<IInteractable>();
    // 
    //         if (interactable != null)
    //         {
    //             if (_lastInteracted != null && _lastInteracted != interactable)
    //             {
    //                 _lastInteracted.OnHoverExit();
    //                 _lastInteracted = null;
    //             }
    //             else
    //                 interactable.OnHover();
    // 
    //             if (Input.GetKeyDown(_interactKey))
    //             {
    //                 Debug.Log($"Interacting with {interactable.ItemType}");
    //                 interactable.OnInteract();
    //             }
    // 
    //             _lastInteracted = interactable;
    //         }
    //     }
    //     else
    //     {
    //         if (_lastInteracted != null)
    //         {
    //             _lastInteracted.OnHoverExit();
    //             _lastInteracted = null;
    //         }
    //     }
    // }
}