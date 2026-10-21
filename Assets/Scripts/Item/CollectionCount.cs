
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CollectionCount : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI endingExpText;
    [SerializeField]
    private TextMeshProUGUI collectionCountText;
    [SerializeField]
    private TextMeshProUGUI deathCountText;
    private bool _isOnEndingScreen = false;
    private bool _hasData = false; // 결과 텍스트가 표시된 상태인지
    void Awake()
    {
        //결과 화면이 떠 있는 동안 언어가 바뀌면 텍스트를 다시 적용
        DataManager.Instance.LanguageChangedEvent += OnLanguageChanged;
    }

    void OnDestroy()
    {
        if (DataManager.HasInstance)
        {
            DataManager.Instance.LanguageChangedEvent -= OnLanguageChanged;
        }
    }

    private void OnLanguageChanged(LanguageType newLanguage)
    {
        if (_hasData)
        {
            RefreshTexts();
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (_isOnEndingScreen)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void OnEndingScreen()
    {
        if (_isOnEndingScreen)
        {
            return;
        }
        _isOnEndingScreen = true;
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void StayGame()
    {
        DataManager.Instance.setPlayerThirst(100f);

        DataManager.Instance._thirstyOnce1 = true;
        DataManager.Instance._thirstyOnce2 = true;
        DataManager.Instance._thirstyOnce3 = true;
        DataManager.Instance._reviveInfo = true;
        DataManager.Instance._getGemstone = false;
        DataManager.Instance.SetStartPoint(new Vector3(-20.3f, -1.5f, 28));
        DataManager.Instance._canGoSetting = true;
        SceneManager.LoadScene("Main");//Change it to Main Scene later
        _isOnEndingScreen = false;
    }
    public void SetData()
    {
        RefreshTexts();
        _hasData = true;
        Invoke("OnEndingScreen", 20f);
    }

    // 현재 언어로 결과 텍스트 설정
    private void RefreshTexts()
    {
        endingExpText.text = !DataManager.Instance._getGemstone ? JsonReader.Instance.IngameText("end_01") : JsonReader.Instance.IngameText("end_02");
        collectionCountText.text = JsonReader.Instance.IngameText("result_01") + $" [ {DataManager.Instance.GetCollectedCount().ToString()} / 8 ]";
        deathCountText.text = JsonReader.Instance.IngameText("result_02") + $" : {DataManager.Instance._deathCount}";
    }
}