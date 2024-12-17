using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class getToastMessage
{
    public static Dictionary<string, Dictionary<string, string>> langs = 
        new Dictionary<string, Dictionary<string, string>>() 
        {
            { "fa", new Dictionary<string, string>() { {"Room Number :","شماره اتاق" } ,{ "entry","ورودی" }, { "Number of Round","تعداد دست ها " }, { "You don't have enough money.", "پول کافی ندارید" }, { "Room not found.", "اتاق پیدا نشد " },{ "Enter the coin amount.", "مقدار سکه را وارد کنید " } , { "Enter the room name.", "نام اتاق را وارد کنید " },{"Select", "انتخاب" } ,{"Selected", "انتخاب شده" },{ "Purchased", "خریداری شد" },{ "Purchase completed successfully.", "خرید با موفقیت انجام شد" },{ "Purchase canceled.", "خرید لغو شد " },{ "Connecting to Chat", "در حال اتصال به گفت و گو" } } }
        };

    public static string Translate(string key)
    {
        string language = SecurePlayerPrefs.GetString("lang");
        if (language == "en")
        {
            return key;
        }
        var languageDict = langs[language];
        return languageDict[key];
        //return langs;
    }
}
