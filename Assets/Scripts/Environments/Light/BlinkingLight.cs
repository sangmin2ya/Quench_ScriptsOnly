using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingLight : MonoBehaviour
{
    bool TurnOff = true;
    Light Light;
    // Start is called before the first frame update
    void Start()
    {
        Light = transform.GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(TurnOff)
        {
            Light.intensity -= Time.unscaledDeltaTime;
            if(Light.intensity <= 0.02f )
            {
                Light.intensity = 0f;
                TurnOff = false;
            }
        }
        else
        {
            Light.intensity += Time.unscaledDeltaTime;
            if (Light.intensity >= 2.5f)
            {
                Light.intensity = 2.5f;
                TurnOff = true;
            }
        }
    }
}
