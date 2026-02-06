using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsGemstoneAlive : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (DataManager.Instance._getGemstone)
            Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void GetGemStone()
    {
        DataManager.Instance._getGemstone = true;
    }
}
