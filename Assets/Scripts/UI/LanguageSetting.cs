using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;
public class LanguageSetting : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown languageDropdown; // Reference to the dropdown UI element

    private void Start()
    {
        switch(DataManager.Instance.LanguageType)
        {
            case LanguageType.English:
                languageDropdown.value = 0;
                break;
            case LanguageType.Korean:
                languageDropdown.value = 1;
                break;
        }

        languageDropdown.onValueChanged.AddListener(OnLanguageChange); // Add listener to the dropdown
    }

    public void OnLanguageChange(int index)
    {
        LanguageType previous = DataManager.Instance.LanguageType;
        LanguageType requested = index == 0 ? LanguageType.English : LanguageType.Korean;
        DataManager.Instance.LanguageType = requested;

        //Json 데이터 교체에 실패하면 로컬라이징 패키지 언어도 바꾸지 않고 드롭다운을 되돌림 (언어 혼합 방지)
        if (DataManager.Instance.LanguageType != requested)
        {
            languageDropdown.SetValueWithoutNotify(previous == LanguageType.English ? 0 : 1);
            return;
        }
        //로컬라이징 패키지 언어 변경
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[requested.GetHashCode()];
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}