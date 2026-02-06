using UnityEngine;

public class ObeliskBase : MonoBehaviour
{
    [SerializeField] private ObeliskType _obeliskType;
    public void ReadObelisk()
    {
        AreaVisitManager.Instance.VisitObelisk(_obeliskType);
    }
}
