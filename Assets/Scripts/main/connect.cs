using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using RTLTMPro;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using randome = UnityEngine.Random;
using System.Collections;
using System.IO;
using System;
public class connect : MonoBehaviourPunCallbacks
{

    [SerializeField] private Slider Slider;
    [SerializeField] private TextMeshProUGUI version;
    [SerializeField] private RTLTextMeshPro suggest;
    [SerializeField] private RTLTextMeshPro errorMessage;

    public List<string> loadingMessages = new List<string>
        {
            "یه لحظه صبر کنی، دارم وصل می‌شم...",
            "تو همین لحظه‌ها وصل می‌شیم، نگران نباش!",
            "داری می‌ری تو بازی... نَفَس نگه‌دار!",
            "در حال ردیف کردن کارت‌ها... یه کم دیگه!",
            "یه ذره صبر کنی، درست می‌شه!",
            "الان وصل می‌شیم، یه چایی بریز تا بازی شروع شه!",
            "همه چیز آماده‌ست... فقط یه لحظه دیگه...",
            "دستت رو گرم کن، بازی داره میاد بالا!",
            "یه لحظه طاقت بیار، داریم وصل می‌شیم!"
        };
    void Start()
    {
        version.text = Application.version;
        if (SecurePlayerPrefs.GetString("p") == "")
        {
            SecurePlayerPrefs.SetString("p", "User_" + GetCharacterRandomString(8));
            SecurePlayerPrefs.SetString("lang", "fa");
        }
        if (SecurePlayerPrefs.GetString("lang")=="fa")
        {
            int randomIndex = randome.Range(0, loadingMessages.Count);
            suggest.text = loadingMessages[randomIndex];
        }
        else
        {
            suggest.text = "Please Wait ...";
        }
        conn();
    }
    private void conn()
    {
        AppSettings settings = new AppSettings();
        settings.AppIdRealtime = "cc659d1d-3d9d-4411-b800-c4b785c2594b";
        settings.UseNameServer = true;
        settings.AppIdChat = "28036b6d-e8c8-4507-81e8-169bf3dca934";
        PhotonNetwork.ConnectUsingSettings(settings);
    }

    public override void OnConnectedToMaster()
    {
        print(PhotonNetwork.ServerAddress);
        PhotonNetwork.JoinLobby();
        base.OnConnectedToMaster();
    }
    public override void OnJoinedLobby()
    {
        StartCoroutine(LoadSceneInBackground());
        base.OnJoinedLobby();
    }
    IEnumerator LoadSceneInBackground()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            Slider.value = asyncLoad.progress * 100;
            if (asyncLoad.progress >= 0.9f)
            {
                print("Scene is loaded but not activated.");
                Slider.value = 100;
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
    }
    private void AddLog(string logString)
    {
        string LogString = logString + "\n";
        errorMessage.text += LogString;
    }
    private string GetCharacterRandomString(int len)
    {
        string path = Path.GetRandomFileName();
        path = path.Replace(".", "");
        return path.Substring(0, len);
    }
}
