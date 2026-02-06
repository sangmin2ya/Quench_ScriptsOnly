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
        switch (index)
        {
            case 0:
                DataManager.Instance.LanguageType = LanguageType.English;
                //로컬라이징 패키지 언어 변경
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[LanguageType.English.GetHashCode()];
                break;
            case 1:
                DataManager.Instance.LanguageType = LanguageType.Korean;
                //로컬라이징 패키지 언어 변경
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[LanguageType.Korean.GetHashCode()];
                break;
        }
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}