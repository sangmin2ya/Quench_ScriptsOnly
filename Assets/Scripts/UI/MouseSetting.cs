using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseSetting : MonoBehaviour
{
    [SerializeField] private Slider sensitivitySlider;  // 마우스 감도 슬라이더
    [SerializeField] private TextMeshProUGUI sensitivityText;  // 현재 감도를 보여줄 텍스트

    private void Start()
    {
        sensitivitySlider = GetComponentInChildren<Slider>();
        sensitivityText = transform.Find("MouseSettingTxt").GetComponent<TextMeshProUGUI>();
        sensitivitySlider.value = DataManager.Instance.RotationSpeed;
        UpdateSensitivityText(sensitivitySlider.value);

        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    // 슬라이더 값 변경 시 호출되는 함수
    public void OnSensitivityChanged(float value)
    {
        DataManager.Instance.RotationSpeed = value;
        UpdateSensitivityText(value);
    }

    // 텍스트를 업데이트하는 함수
    private void UpdateSensitivityText(float value)
    {
        sensitivityText.text = $"{value:F2}";
    }
}
