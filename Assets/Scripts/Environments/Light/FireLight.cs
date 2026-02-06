using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLight : MonoBehaviour
{
    private Light _spotlight;
    private float _minIntensity = 2.5f;
    private float _maxIntensity = 3f;
    [SerializeField] private float _changeSpeed; // 밝기 변화 속도
    private float _targetIntensity;

    void Start()
    {
        _spotlight = GetComponent<Light>();

        if (_spotlight == null || _spotlight.type != LightType.Point)
        {
            return;
        }

        StartCoroutine(SmoothChangeLightIntensity());
    }

    IEnumerator SmoothChangeLightIntensity()
    {
        while (true)
        {
            _targetIntensity = Random.Range(_minIntensity, _maxIntensity);

            while (Mathf.Abs(_spotlight.intensity - _targetIntensity) > 0.01f)
            {
                _spotlight.intensity = Mathf.Lerp(_spotlight.intensity, _targetIntensity, _changeSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
