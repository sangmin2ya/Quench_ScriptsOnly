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

            Initialize(DataManager.Instance.LanguageType);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    public LanguageType languageType;
    public ItemDB itemDB;
    public IngameTextDB ingameTextDB;
    public Dictionary<string, string> ingameTextDictionary = new Dictionary<string, string>();

    // Start is called before the first frame update
    public void Initialize(LanguageType languageType)
    {
        // ItemDB 초기화
        TextAsset itemJson = Resources.Load<TextAsset>($"Json/{languageType.ToString()}_Item");
        print(itemJson.text);
        itemDB = JsonUtility.FromJson<ItemDB>(itemJson.text);
        
        foreach (var item in itemDB.items)
        {
            print(item.itemName);
        }
        // IngameTextDB 초기화
        TextAsset ingameTextJson = Resources.Load<TextAsset>($"Json/{languageType.ToString()}_IngameText");
        print(ingameTextJson.text);
        ingameTextDB = JsonUtility.FromJson<IngameTextDB>(ingameTextJson.text);
        ingameTextDictionary.Clear();
        foreach (var ingameText in ingameTextDB.ingameTexts)
        {
            print(ingameText.textID + " : " + ingameText.ingameText);
            ingameTextDictionary.Add(ingameText.textID, ingameText.ingameText);
        }
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