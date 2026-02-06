using UnityEngine;

public class PlayerAreaInteraction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IEnterable enterableArea = other.GetComponent<IEnterable>();
        
        if (enterableArea != null)
        {
            Debug.Log($"Player entered an area of type: {enterableArea.AreaType}");
            enterableArea.OnPlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IEnterable enterableArea = other.GetComponent<IEnterable>();
        
        if (enterableArea != null)
        {
            Debug.Log($"Player exited an area of type: {enterableArea.AreaType}");
            enterableArea.OnPlayerExit();
        }
    }
}