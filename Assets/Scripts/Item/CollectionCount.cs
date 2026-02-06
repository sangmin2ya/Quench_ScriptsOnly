
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
    // Start is called before the first frame update
    void Start()
    {

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
        endingExpText.text = !DataManager.Instance._getGemstone ? JsonReader.Instance.IngameText("end_01") : JsonReader.Instance.IngameText("end_02");
        collectionCountText.text = JsonReader.Instance.IngameText("result_01") + $" [ {DataManager.Instance.GetCollectedCount().ToString()} / 8 ]";
        deathCountText.text = JsonReader.Instance.IngameText("result_02") + $" : {DataManager.Instance._deathCount}";
        Invoke("OnEndingScreen", 20f);
    }
}