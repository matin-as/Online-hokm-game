using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Language : MonoBehaviour
{
    public enum Languages
    {
        Persian,
        English
    }
    public Dictionary<string, Dictionary<string, string>> langs;
    void Start()
    {
        Change_Language(is_Persian());
    }

    void Update()
    {
        
    }
    public static bool is_Persian()
    {
        return SecurePlayerPrefs.GetString("lang")=="fa"? true:false;
    }
    public static void Change_Language(bool isfa)
    {
        SecurePlayerPrefs.SetString("lang",isfa ? "fa":"en");
        string lang = SecurePlayerPrefs.GetString("lang");
        main m_main = GameObject.Find("main").GetComponent<main>();
        GameObject parent = m_main.customRoom;
        m_main.custome_coin = FindInactiveObject(parent.transform,ConvertLanguagePrefix(m_main.custome_coin.name, lang)).GetComponent<TMP_InputField>();
        m_main.inputField = FindInactiveObject(parent.transform, ConvertLanguagePrefix(m_main.inputField.name, lang)).GetComponent<TMP_InputField>();
        m_main.PrefabRoom = Resources.Load<GameObject>(ConvertLanguagePrefix(m_main.PrefabRoom.name, lang));
        m_main.rooms = FindInactiveObject(parent.transform, ConvertLanguagePrefix(m_main.rooms.name, lang));
        foreach (var g in Object.FindObjectsOfType<getlang>(true))
        {
            bool defult = g.gameObject.activeInHierarchy;
            g.gameObject.SetActive(!defult);
            g.gameObject.SetActive(defult);
        }
    }
    private static string ConvertLanguagePrefix(string input, string newPrefix)
    {
        var parts = input.Split('-');
        if (parts.Length > 1)
        {
            parts[0] = newPrefix;
            return string.Join("-", parts);
        }

        return input;
    }
    static GameObject  FindInactiveObject(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child.gameObject;

            GameObject result = FindInactiveObject(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}
