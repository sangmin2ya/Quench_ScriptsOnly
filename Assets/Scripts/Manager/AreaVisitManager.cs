using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaVisitManager : MonoBehaviour
{
    private List<AreaType> _visitedAreas = new List<AreaType>();
    private List<Tuple<AreaType, int>> _checkedInfo = new List<Tuple<AreaType, int>>();
    private List<ObeliskType> _visitedObelisks = new List<ObeliskType>();
    private static AreaVisitManager _instance;

    public static AreaVisitManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject areaVisitManagerObject = new GameObject("AreaVisitManager");
                _instance = areaVisitManagerObject.AddComponent<AreaVisitManager>();

                DontDestroyOnLoad(areaVisitManagerObject);
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            DontDestroyOnLoad(_instance);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void VisitArea(AreaType areaType)
    {
        if (!_visitedAreas.Contains(areaType))
        {
            _visitedAreas.Add(areaType);
        }
    }

    public void CheckInfo(AreaType areaType, int info)
    {
        if (!_checkedInfo.Contains(new Tuple<AreaType, int>(areaType, info)))
        {
            _checkedInfo.Add(new Tuple<AreaType, int>(areaType, info));
        }
    }

    public void VisitObelisk(ObeliskType obeliskType)
    {
        if (!_visitedObelisks.Contains(obeliskType))
        {
            _visitedObelisks.Add(obeliskType);
        }
    }

    public bool IsVisited(AreaType areaType)
    {
        return _visitedAreas.Contains(areaType);
    }

    public bool IsVisited(ObeliskType obeliskType)
    {
        return _visitedObelisks.Contains(obeliskType);
    }

    public void Reset()
    {
        _visitedAreas.Clear();
        _visitedObelisks.Clear();
    }

    public List<AreaType> GetAreaList()
    {
        return _visitedAreas;
    }

    public List<ObeliskType> GetObeliskList()
    {
        return _visitedObelisks;
    }
}
