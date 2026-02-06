using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [SerializeField] private Slider soundSlider;
    [SerializeField] private TextMeshProUGUI soundText;

    private void Start()
    {
        soundSlider = GetComponentInChildren<Slider>();
        soundText = transform.Find("SoundSettingTxt").GetComponent<TextMeshProUGUI>();
        soundSlider.value = DataManager.Instance.SoundVolume / 2f;
        UpdateSoundText(soundSlider.value);

        soundSlider.onValueChanged.AddListener(OnSoundChanged);
    }

    // 슬라이더 값 변경 시 호출되는 함수
    public void OnSoundChanged(float value)
    {
        DataManager.Instance.SoundVolume = value * 2f;
        UpdateSoundText(value);
    }

    // 텍스트를 업데이트하는 함수
    private void UpdateSoundText(float value)
    {
        soundText.text = $"{value:F2}";
    }
}
