using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AreaPrinter : MonoBehaviour
{
    private Dictionary<string, GameObject> _areaPrefabs = new Dictionary<string, GameObject>();

    private void Start()
    {
        _areaPrefabs.Add(AreaType.Cave.ToString(), transform.Find("Cave").gameObject);
        _areaPrefabs.Add(AreaType.Oasis.ToString(), transform.Find("Oasis").gameObject);
        _areaPrefabs.Add(AreaType.Village.ToString(), transform.Find("Village").gameObject);
        _areaPrefabs.Add(AreaType.TwinRocks.ToString(), transform.Find("TwinRock").gameObject);
        _areaPrefabs.Add(AreaType.UnderworldEntrance.ToString(), transform.Find("Underworld").gameObject);
        _areaPrefabs.Add(AreaType.Stonehenge.ToString(), transform.Find("Stonehenge").gameObject);
        _areaPrefabs.Add(AreaType.Minipyramid.ToString(), transform.Find("Minipyramid").gameObject);
        _areaPrefabs.Add(AreaType.WreckedShip.ToString(), transform.Find("WreckedShip").gameObject);
        _areaPrefabs.Add(AreaType.LionStatue.ToString(), transform.Find("LionStatue").gameObject);
        _areaPrefabs.Add(AreaType.BigSkull.ToString(), transform.Find("BigSkull").gameObject);

        _areaPrefabs.Add(ObeliskType.Obelisk0.ToString(), transform.Find("Obelisk0").gameObject);
        _areaPrefabs.Add(ObeliskType.Obelisk1.ToString(), transform.Find("Obelisk1").gameObject);
        _areaPrefabs.Add(ObeliskType.Obelisk2.ToString(), transform.Find("Obelisk2").gameObject);
        _areaPrefabs.Add(ObeliskType.Obelisk3.ToString(), transform.Find("Obelisk3").gameObject);
        _areaPrefabs.Add(ObeliskType.Obelisk4.ToString(), transform.Find("Obelisk4").gameObject);
        _areaPrefabs.Add(ObeliskType.Obelisk5.ToString(), transform.Find("Obelisk5").gameObject);
        _areaPrefabs.Add(ObeliskType.Obelisk6.ToString(), transform.Find("Obelisk6").gameObject);
        _areaPrefabs.Add(ObeliskType.Obelisk7.ToString(), transform.Find("Obelisk7").gameObject);

        foreach (var area in _areaPrefabs)
        {
            area.Value.SetActive(false);
        }

        _areaPrefabs[AreaType.Cave.ToString()].SetActive(true);
    }
    private void Update()
    {
        foreach (var area in AreaVisitManager.Instance.GetAreaList())
        {
            PrintArea(area.ToString());
        }
        foreach (var obelisk in AreaVisitManager.Instance.GetObeliskList())
        {
            PrintArea(obelisk.ToString());
        }
    }
    public void PrintArea(string areaType)
    {
        _areaPrefabs[areaType].SetActive(true);
    }
}
