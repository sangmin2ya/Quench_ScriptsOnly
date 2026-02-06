using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [SerializeField] private List<GameObject> _lights = new List<GameObject>();
    private bool _isOn = false;
    private bool _turnOnOnce = true;
    // Start is called before the first frame update
    void Start()
    {
        _lights.ForEach(light => light.SetActive(false));
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void TurnonLights()
    {
        if (_isOn && _turnOnOnce)
        {
            GetComponent<AudioSource>().Play();
            _turnOnOnce = false;
        }
        _lights.ForEach(light => light.SetActive(true));
        _isOn = true;
    }
}
