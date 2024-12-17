using RTLTMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System;

public class Payment : MonoBehaviour
{
    [SerializeField] private Toggle toggle;   
    [SerializeField] private TextMeshProUGUI TextCoin;
    [SerializeField] private RTLTextMeshPro currentName;
    [SerializeField] private ChatManager chatManager;
    [SerializeField] private GameObject panel_setName;
    [SerializeField] private GameObject btnbuy;
    [SerializeField] private GameObject profile;
    [SerializeField] private GameObject listprofile;
    [SerializeField] private GameObject prefab_profile;
    [SerializeField] private GameObject context;
    [SerializeField] private Image currentProfileImage;
    [SerializeField] private GameObject backbtn;
    private static readonly List<int> profileprices = new List<int> {100,200,300 ,400,500 , 100, 200, 300, 400, 500 , 100, 200, 300, 400, 500 , 100, 200, 300, 400, 500 };
    void Start()
    {
        if (IsDeviceRooted() || IsRunningOnEmulator())
        {
            Application.Quit();
        }
        if(Application.platform==RuntimePlatform.Android)
        {
            if (string.IsNullOrEmpty(SecurePlayerPrefs.GetString("playerName")))
            {
                panel_setName.SetActive(true);
                panel_setName.GetComponentInChildren<Button>().AddEventListener("", setName);
            }
            else
            {
                currentName.text = PhotonNetwork.LocalPlayer.NickName;
                currentName.text = SecurePlayerPrefs.GetString("playerName");
                PhotonNetwork.NickName = currentName.text;  
                chatManager.enabled = true;
            }
        }
        else
        {
            if (string.IsNullOrEmpty(PhotonNetwork.NickName))
            {
                panel_setName.SetActive(true);
                panel_setName.GetComponentInChildren<Button>().AddEventListener("", setName);
            }
            else
            {
                currentName.text = PhotonNetwork.LocalPlayer.NickName;
                chatManager.enabled = true;
            }
        }
        if (SecurePlayerPrefs.GetInt("give") == 0)
        {
            SecurePlayerPrefs.SetInt("give", 1);
            SecurePlayerPrefs.SetInt("coin", 500);
            SecurePlayerPrefs.SetString("profile", "c");
            SecurePlayerPrefs.SetString("c", "c");
        }
        profile.GetComponent<Button>().AddEventListener(0, openbuyprofile);
        backbtn.GetComponent<Button>().AddEventListener(0, openbuyprofile);
        // set profile 
        update_profile();
    }
    private void openbuyprofile(int i )
    {
        listprofile.SetActive(!listprofile.activeInHierarchy);
    }
    private void set_(int i)
    {
        btnbuy.GetComponentInChildren<RTLTextMeshPro>().text = i.ToString();
    }
    private void buyprofile(string s ,int i)
    {
        s = s[0].ToString();
        if(SecurePlayerPrefs.GetString(s)!="")
        {
            // just set 
            SecurePlayerPrefs.SetString("profile", s);
            update_profile();
            return;
        }
        if (SecurePlayerPrefs.GetInt("coin")>=i)
        {
            Toast._ShowAndroidToastMessage(getToastMessage.Translate("Purchased"));
            SecurePlayerPrefs.SetInt("coin", SecurePlayerPrefs.GetInt("coin")-i);
            SecurePlayerPrefs.SetString(s, s);



            SecurePlayerPrefs.SetString("profile", s);
            Sprite mySprite = Resources.Load<Sprite>("images/" + s);
            currentProfileImage.sprite = mySprite;
            profile.GetComponent<Image>().sprite = mySprite;
            Update_Coin();
            update_profile();
            return;
        }
        Toast._ShowAndroidToastMessage(getToastMessage.Translate("You don't have enough money."));
    }
    private void update_profile()
    {
        for (int z = 0; z < context.transform.childCount; z++)
        {
            Destroy(context.transform.GetChild(z).gameObject);
        }
        string currentProfile = SecurePlayerPrefs.GetString("profile");
        Sprite mySprite = Resources.Load<Sprite>("images/" + currentProfile);
        currentProfileImage.sprite = mySprite;
        profile.GetComponent<Image>().sprite = mySprite;
        Sprite[] mySprites = Resources.LoadAll<Sprite>("images/");
        int i = 0;
        foreach (Sprite sprite in mySprites)
        {
            GameObject g = Instantiate(prefab_profile, context.transform);
            g.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            RTLTextMeshPro text = g.transform.GetComponentInChildren<RTLTextMeshPro>();
            Button btn = g.transform.GetComponentInChildren<Button>();
            if (SecurePlayerPrefs.GetString(sprite.name[0].ToString()) != "")
            {
                if (currentProfile == sprite.name[0].ToString())
                {
                    text.text = getToastMessage.Translate("Selected");
                }
                else
                {
                    text.text = getToastMessage.Translate("Select");
                    btn.AddEventListener(sprite.name, profileprices[i], buyprofile);
                }
            }
            else
            {
                btn.AddEventListener(sprite.name, profileprices[i], buyprofile);
                text.text = profileprices[i].ToString();

            }
            i++;
        }
    }
    private void setName(string s)
    {
        string Name = panel_setName.transform.GetChild(1).GetComponent<TMP_InputField>().text;
        if (!string.IsNullOrEmpty(Name))
        {
            panel_setName.SetActive(false);
            SecurePlayerPrefs.SetString("playerName", Name);
            PhotonNetwork.NickName = Name;
            PhotonNetwork.LocalPlayer.NickName = Name;
            currentName.text = Name;
            chatManager.enabled = true;
            Language.Change_Language(toggle.isOn);
        }
    }
    bool IsDeviceRooted()
    {
        bool isRooted = false;
        string[] rootPaths = {
            "/system/app/Superuser.apk",
            "/sbin/su",
            "/system/bin/su",
            "/system/xbin/su",
            "/data/local/xbin/su",
            "/data/local/bin/su",
            "/system/sd/xbin/su",
            "/system/bin/failsafe/su",
            "/data/local/su"
        };

        foreach (string path in rootPaths)
        {
            if (System.IO.File.Exists(path))
            {
                isRooted = true;
                break;
            }
        }

        return isRooted;
    }
    bool IsRunningOnEmulator()
    {
        string buildBrand = SystemInfo.deviceModel.ToLower();
        string[] emulatorIndicators = {
            "generic", "google_sdk", "emulator", "android sdk built for x86"
        };

        foreach (string indicator in emulatorIndicators)
        {
            if (buildBrand.Contains(indicator))
            {
                return true;
            }
        }

        return false;
    }
    private void Update_Coin()
    {
        TextCoin.text = SecurePlayerPrefs.GetInt("coin").ToString();
    }

    public void Fix(GameObject g)
    {
        string currenttext = g.GetComponent<TMP_InputField>().text;
        g.GetComponentsInChildren<RTLTextMeshPro>()[1].text = currenttext;
    }
}
