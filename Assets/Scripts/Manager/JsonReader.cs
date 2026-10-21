using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonReader : MonoBehaviour
{
    private static JsonReader _instance;
    public static JsonReader Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject JsonReaderObject = new GameObject("JsonReader");
                _instance = JsonReaderObject.AddComponent<JsonReader>();

                DontDestroyOnLoad(JsonReaderObject);
            }
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            DontDestroyOnLoad(_instance);

            LanguageType startLanguage = DataManager.Instance.LanguageType;
            // 시스템 언어 파일을 못 읽으면 영어로 대체
            if (!Initialize(startLanguage) && startLanguage != LanguageType.English && Initialize(LanguageType.English))
            {
                Debug.LogWarning($"{startLanguage} 데이터를 읽지 못해 English 데이터를 사용합니다.");
            }
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    public LanguageType languageType;
    private bool _isLoaded = false;
    // 해당 언어 데이터가 로드되어 있는지 (로드 실패 후 같은 언어를 다시 선택하면 재시도할 수 있도록)
    public bool IsLoaded(LanguageType type) => _isLoaded && languageType == type;
    public ItemDB itemDB;
    public IngameTextDB ingameTextDB;
    public Dictionary<string, string> ingameTextDictionary = new Dictionary<string, string>();

    /// <summary>
    /// 언어별 아이템/인게임 텍스트 Json을 읽음. 두 파일을 모두 읽은 경우에만 교체하고 true 반환
    /// (하나만 바뀌어 언어가 섞이지 않도록 실패하면 기존 데이터 유지)
    /// </summary>
    public bool Initialize(LanguageType languageType)
    {
        TextAsset itemJson = LoadJson($"Json/{languageType}_Item");
        TextAsset ingameTextJson = LoadJson($"Json/{languageType}_IngameText");
        if (itemJson == null || ingameTextJson == null)
        {
            return false;
        }

        ItemDB newItemDB;
        IngameTextDB newIngameTextDB;
        try
        {
            newItemDB = JsonUtility.FromJson<ItemDB>(itemJson.text);
            newIngameTextDB = JsonUtility.FromJson<IngameTextDB>(ingameTextJson.text);
        }
        catch (ArgumentException e)
        {
            Debug.LogError($"{languageType} Json 형식이 올바르지 않습니다: {e.Message}");
            return false;
        }
        if (newItemDB?.items == null || newIngameTextDB?.ingameTexts == null)
        {
            Debug.LogError($"{languageType} Json에 items 또는 ingameTexts가 없습니다.");
            return false;
        }

        Dictionary<string, string> newDictionary = new Dictionary<string, string>();
        foreach (var ingameText in newIngameTextDB.ingameTexts)
        {
            // 중복 ID는 처음 값을 유지하고 경고만 남김 (예외로 나머지 텍스트 로드가 중단되지 않도록)
            if (newDictionary.ContainsKey(ingameText.textID))
            {
                Debug.LogWarning($"중복된 Ingame text ID '{ingameText.textID}'는 무시합니다. ({languageType})");
                continue;
            }
            newDictionary.Add(ingameText.textID, ingameText.ingameText);
        }

        itemDB = newItemDB;
        ingameTextDB = newIngameTextDB;
        ingameTextDictionary = newDictionary;
        this.languageType = languageType;
        _isLoaded = true;
        return true;
    }

    // 파일이 없으면 에러를 남기고 기존 데이터를 유지
    private TextAsset LoadJson(string path)
    {
        TextAsset json = Resources.Load<TextAsset>(path);
        if (json == null)
        {
            Debug.LogError($"Resources/{path} 파일을 찾을 수 없습니다.");
        }
        return json;
    }

    public string IngameText(string textID)
    {
        if (ingameTextDictionary.TryGetValue(textID, out string ingameText))
        {
            return ingameText;
        }
        else
        {
            Debug.LogWarning($"Ingame text with ID '{textID}' not found.");
            return string.Empty;
        }
    }
}

[Serializable]
public class ItemDB
{
    public Item[] items;
}

[Serializable]
public class IngameTextDB
{
    public IngameText[] ingameTexts;
}