using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TimeController : MonoBehaviour
{
    private Transform _directionalLight;
    [SerializeField] private float _rotationDuration = 60f; // 5 minutes in seconds
    [SerializeField] private Material _skyboxMaterial;
    private Quaternion _initialRotation;
    private float _magnitude = 0;
    private void Start()
    {
        if (DataManager.Instance.GetStartPoint() == new Vector3(-20.3f, -1.5f, 28))
        {
            _skyboxMaterial = RenderSettings.skybox;
            _directionalLight = GameObject.Find("Directional Light").transform;
            _initialRotation = _directionalLight.rotation;
            _skyboxMaterial.SetFloat("_AtmosphereThickness", 1.7f);
            _skyboxMaterial.SetFloat("_Exposure", 0.9f);
            ResetTime();
            TimeRun();
        }
        else
        {
            _skyboxMaterial = RenderSettings.skybox;
            _directionalLight = GameObject.Find("Directional Light").transform;
            _directionalLight.GetComponent<Light>().intensity = 0;
            RenderSettings.fogColor = Color.black;
            Debug.Log("TimeController: Start() - Not at start point, setting night mode.");
            _skyboxMaterial.SetFloat("_AtmosphereThickness", 0.8f);
            _skyboxMaterial.SetFloat("_Exposure", 0.1f);
        }
    }
    public void TimeStop()
    {
        _magnitude = 0;
    }
    public void TimeRun()
    {
        _magnitude = 1f;
    }
    public void ResetTime()
    {
        StopAllCoroutines();
        _skyboxMaterial.SetFloat("_AtmosphereThickness", 1.7f);
        _skyboxMaterial.SetFloat("_Exposure", 0.9f);
        _directionalLight.rotation = _initialRotation;
        StartCoroutine("RotateSun");
    }
    private IEnumerator RotateSun()
    {
        Debug.Log("RotateSun");
        float _elapsedTime = 0f;
        while (_elapsedTime < _rotationDuration)
        {
            _elapsedTime += Time.deltaTime * _magnitude;
            // Calculate the current x and y angles based on elapsed time
            float _xAngle = Mathf.Lerp(35f, 0f, Mathf.Pow(_elapsedTime / _rotationDuration, 3));
            float _yAngle = Mathf.Lerp(80f, 125f, _elapsedTime / _rotationDuration); // Y-axis from 80 to 125 degrees
                                                                                     // Apply the rotation
            _directionalLight.rotation = Quaternion.Euler(_xAngle, _yAngle, 0f);
            yield return null;
        }
        Color stratColor = RenderSettings.fogColor;
        while (_directionalLight.GetComponent<Light>().intensity > 0)
        {

            _directionalLight.GetComponent<Light>().intensity -= Time.deltaTime / 30;
            RenderSettings.fogColor = Color.Lerp(Color.black, stratColor, _directionalLight.GetComponent<Light>().intensity);
            _skyboxMaterial.SetFloat("_AtmosphereThickness", Mathf.Lerp(0.8f, 1.7f, _directionalLight.GetComponent<Light>().intensity));
            _skyboxMaterial.SetFloat("_Exposure", Mathf.Lerp(0.1f, 0.9f, _directionalLight.GetComponent<Light>().intensity));
            yield return null;

        }
    }
}