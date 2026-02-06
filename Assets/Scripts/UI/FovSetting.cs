using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cinemachine;

public class FovSetting : MonoBehaviour
{
    [SerializeField] private Slider fovSlider;
    [SerializeField] private TextMeshProUGUI fovText;
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    [SerializeField] private CinemachineVirtualCamera dyingCamera;
    [SerializeField] private float minFov = 30f; // 최소 FOV 값
    [SerializeField] private float maxFov = 120f; // 최대 FOV 값
    [SerializeField] private float currentFov = 60f; // 기본 FOV 값

    private void Start()
    {
        fovSlider = GetComponentInChildren<Slider>();
        fovText = transform.Find("FovSettingTxt").GetComponent<TextMeshProUGUI>();
        playerCamera = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        dyingCamera = GameObject.Find("DeathCollapse Camera").GetComponent<CinemachineVirtualCamera>();
        currentFov = DataManager.Instance.FieldOfView; // DataManager에서 FOV 값 가져오기
        fovSlider.value = currentFov;
        UpdateFovText(fovSlider.value);

        fovSlider.onValueChanged.AddListener(OnFovChanged);
    }

    // 슬라이더 값 변경 시 호출되는 함수
    public void OnFovChanged(float value)
    {
        currentFov = fovSlider.value;
        DataManager.Instance.FieldOfView = currentFov; // DataManager에 FOV 값 저장
        playerCamera.m_Lens.FieldOfView = currentFov;
        dyingCamera.m_Lens.FieldOfView = currentFov;
        UpdateFovText(value);
    }

    // 텍스트를 업데이트하는 함수
    private void UpdateFovText(float value)
    {
        fovText.text = $"{value:F0}"; // 소수점 없이 정수로 표시
    }
}
