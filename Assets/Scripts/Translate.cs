using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Translate
{
    Dictionary<string, Dictionary<string,string>> dic = new Dictionary<string, Dictionary<string,string>>() { { "fa",new Dictionary<string, string> { { "","" } }  } };
    public Dictionary<string, string> getTranslate(string lang)
    {
        return dic[lang];
    }
}
