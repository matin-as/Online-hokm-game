using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using random = UnityEngine.Random;
using TMPro;
using RTLTMPro;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using System.Text;

public class main : MonoBehaviourPunCallbacks
{
    public GameObject rooms;
    public GameObject customRoom;
    public   TMP_InputField custome_coin;
    public   TMP_InputField inputField;
    public GameObject PrefabRoom;
    //
    [SerializeField] private GameObject cant_tuch;
    [SerializeField] private GameObject disconnect;
    [SerializeField] private GameObject JoinRandome;
    [SerializeField] private GameObject Home;
    [SerializeField] private GameObject Joined_to_room;
    [SerializeField] private RTLTextMeshPro ff;
    [SerializeField] private TextMeshProUGUI te;
    [SerializeField] private GameObject joinRandome;
    [SerializeField] private GameObject panel_createdRoom;
    [SerializeField] private TextMeshProUGUI TextCoin;
    [SerializeField] private RTLTextMeshPro text;
    [SerializeField] private RTLTextMeshPro[] texts;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private List<GameObject> ls;
    VideoPlayer lastvideo;
    bool a = false;
    VideoClip tempclip;
    private int d;
    private int dast;
    bool played = false;
    bool aded = false;
    void Start()
    {
        Update_Coin();
    }
    public void Offline_play()
    {
        SceneManager.LoadScene(2);
    }
    public void online_play(GameObject g )
    {
        g.SetActive(!g.activeInHierarchy);
    }
    public void online_play_with(int i)
    {
        d = i;
        dast = 7;
        if (i==100)
        {
            dast = 3;
        }
        int coin = SecurePlayerPrefs.GetInt("coin");
        if(coin>=d)
        {
            Hashtable expectedCustomRoomProperties = new Hashtable() { { "coin", d },{"dast",dast }, { "iscustome", false } };
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
            cant_tuch.SetActive(true);
        }
        else
        {
            _ShowAndroidToastMessage(getToastMessage.Translate("You don't have enough money."));
        }
        //PhotonNetwork.JoinRandomRoom(,);
    }
    public override void OnJoinedRoom()
    {
        cant_tuch.SetActive(false);
        if (bool.Parse(PhotonNetwork.CurrentRoom.CustomProperties["iscustome"].ToString()))
        {
            if (int.Parse(PhotonNetwork.CurrentRoom.CustomProperties["coin"].ToString()) > SecurePlayerPrefs.GetInt("coin"))
            {
                Toast._ShowAndroidToastMessage(getToastMessage.Translate("You don't have enough money."));
                PhotonNetwork.LeaveRoom();
                return;
            }
            SecurePlayerPrefs.SetInt("coin", SecurePlayerPrefs.GetInt("coin") - int.Parse(PhotonNetwork.CurrentRoom.CustomProperties["coin"].ToString()));
        }
        else
        {
            SecurePlayerPrefs.SetInt("coin", SecurePlayerPrefs.GetInt("coin") - int.Parse(PhotonNetwork.CurrentRoom.CustomProperties["coin"].ToString()));
        }
        Joined_to_room.SetActive(false);
        JoinRandome.SetActive(false);
        Hashtable playerProperties = new Hashtable();
        playerProperties["myname"] = SecurePlayerPrefs.GetString("playerName");
        playerProperties["profilePicture"] = SecurePlayerPrefs.GetString("profile");
        playerProperties["mycoin"] = SecurePlayerPrefs.GetInt("coin");
        playerProperties["payed"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        // Translate 
        panel_createdRoom.transform.GetChild(5).GetComponent<RTLTextMeshPro>().text = getToastMessage.Translate("Number of Round");
        panel_createdRoom.transform.GetChild(6).GetComponent<RTLTextMeshPro>().text = getToastMessage.Translate("entry");
        panel_createdRoom.transform.GetChild(9).GetChild(0).GetComponent<RTLTextMeshPro>().text = getToastMessage.Translate("Room Number :");
        //
        panel_createdRoom.transform.GetChild(1).GetComponent<RTLTextMeshPro>().text = PhotonNetwork.CurrentRoom.CustomProperties["coin"].ToString();
        panel_createdRoom.transform.GetChild(2).GetComponent<RTLTextMeshPro>().text = PhotonNetwork.CurrentRoom.CustomProperties["dast"].ToString();
        panel_createdRoom.transform.GetChild(9).GetComponent<RTLTextMeshPro>().text = PhotonNetwork.CurrentRoom.Name;
        Update_Coin();
        update_player();
        base.OnJoinedRoom();
    }
    public override void OnPlayerPropertiesUpdate(Player player, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(bool.Parse(changedProps["payed"].ToString()))
        {
            update_player();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Create_Roome(d,dast,false);
        base.OnJoinRandomFailed(returnCode, message);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        cant_tuch.SetActive(false);
        Toast._ShowAndroidToastMessage(getToastMessage.Translate("Room not found."));
        base.OnJoinRoomFailed(returnCode, message);
    }
    public void CreateCustomeRoom()
    {
        if(custome_coin.text=="")
        {
            Toast._ShowAndroidToastMessage(getToastMessage.Translate("Enter the coin amount."));
            return;
        }
        Create_Roome(int.Parse(custome_coin.text),int.Parse(te.text), true);
    }
    private void Create_Roome(int coin,int int_dast,bool isCustomeRoom,string RoomName = null)
    {
        if(coin >  SecurePlayerPrefs.GetInt("coin"))
        {
            Toast._ShowAndroidToastMessage(getToastMessage.Translate("You don't have enough money."));
            return;
        }
        cant_tuch.SetActive(true);
        RoomOptions roomOptions = new RoomOptions()
        {
            MaxPlayers = 4,
            CustomRoomProperties = new Hashtable() { { "coin", coin  }, { "dast", int_dast } , { "iscustome", isCustomeRoom } },
            CustomRoomPropertiesForLobby = new string[] { "coin", "dast" , "iscustome" } // این خط باعث می‌شود مقدار پول در لابی هم نمایش داده شود
        };
        PhotonNetwork.JoinOrCreateRoom(Get8CharacterRandomString(), roomOptions, TypedLobby.Default);
    }
    void update_player()
    {
        if (PhotonNetwork.CurrentRoom.IsVisible)// its a Custome Roome 
        {
            panel_createdRoom.SetActive(true);
        }
        else
        {
            joinRandome.SetActive(true);
            joinRandome.transform.GetChild(0).GetComponent<RTLTextMeshPro>().text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        }
        Player[] players = PhotonNetwork.PlayerList;
        text.text = "";
        StringBuilder stringBuilder = new StringBuilder();
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            stringBuilder.AppendLine((string)player.CustomProperties["myname"]);
        }

        text.text = stringBuilder.ToString();


        if (!aded)
        {
            GameObject.Find("Leave Room").GetComponent<Button>().AddEventListener(0, leaveRoom);
            aded = true;
        }
        if (PhotonNetwork.CurrentRoom.PlayerCount == 4)
        {
            panel_createdRoom.SetActive(false);
            PhotonNetwork.LoadLevel(3);
        }
    }
    private void leaveRoom(int i )
    {
        Home.SetActive(true );
        aded = false;
        GameObject.Find("Leave Room").GetComponent<Button>().onClick.RemoveAllListeners();
        panel_createdRoom.SetActive(false);
        SecurePlayerPrefs.SetInt("coin", SecurePlayerPrefs.GetInt("coin") + int.Parse(PhotonNetwork.CurrentRoom.CustomProperties["coin"].ToString()));
        Update_Coin();
        PhotonNetwork.LeaveRoom();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        update_player();
        base.OnPlayerLeftRoom(otherPlayer);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < rooms.transform.childCount; i++)
        {
            Destroy(rooms.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if(!roomList[0].RemovedFromList && bool.Parse(roomList[i].CustomProperties["iscustome"].ToString()))
            {
                GameObject g = Instantiate(PrefabRoom, rooms.transform);
                g.transform.GetChild(0).GetComponent<Button>().AddEventListener(roomList[i], connectTORoom);
                g.transform.GetChild(4).GetComponent<RTLTextMeshPro>().text = roomList[i].CustomProperties["coin"].ToString();
                g.transform.GetChild(5).GetComponent<RTLTextMeshPro>().text = roomList[i].CustomProperties["dast"].ToString();
                g.transform.GetChild(6).GetComponent<RTLTextMeshPro>().text = roomList[i].PlayerCount.ToString();
            }

        }
        base.OnRoomListUpdate(roomList);
    }
    void connectTORoom(RoomInfo roominfo)
    {
        Update_Coin();
        PhotonNetwork.JoinRoom(roominfo.Name);
        cant_tuch.SetActive(true);
    }
    public string Get8CharacterRandomString()
    {
        return random.Range(10000, 100000).ToString();
    }
    private void _ShowAndroidToastMessage(string message)
    {
        if(Application.platform==RuntimePlatform.Android)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                    toastObject.Call("show");
                }));
            }
        }
        else
        {
            Debug.Log(message);
        }
    }
    private void Update_Coin()
    {
        TextCoin.text = SecurePlayerPrefs.GetInt("coin").ToString();
    }
    public void Change_Languge()
    {
        Toggle toggle = toggleGroup.ActiveToggles().FirstOrDefault();
        Translate translate = new Translate();
        Dictionary<string, string> k = translate.getTranslate(toggle.name);
        foreach (RTLTextMeshPro text in texts)
        {
            text.text = k[text.name];
        }
    }
    public void openchat(GameObject g )
    {
        g.GetComponent<Animator>().SetBool("a",!g.GetComponent<Animator>().GetBool("a"));
    }
    public void OpenURL(string s)
    {
        Application.OpenURL(s);
    }
    public void joinRoomWithName()
    {
        if(string.IsNullOrEmpty(inputField.text))
        {
            Toast._ShowAndroidToastMessage(getToastMessage.Translate("Enter the room name."));
            return;
        }
        cant_tuch.SetActive(true);
        PhotonNetwork.JoinRoom(inputField.text);
    }
    public void add()
    {
        if(int.Parse(te.text)==7)
        {
            return;
        }
        te.text = (int.Parse(te.text) + 1).ToString();
    }
    public void increas()
    {
        if(int.Parse(te.text) == 1)
        {
            return;
        }
        te.text = (int.Parse(te.text) - 1).ToString();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        print(cause);
        disconnect.SetActive(true);
        PhotonNetwork.Reconnect();
        base.OnDisconnected(cause);
    }
    public override void OnConnectedToMaster()
    {
        disconnect.SetActive(false);
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        base.OnConnectedToMaster();
    }
    public void anime(GameObject g)
    {
        g.GetComponentInParent<Animator>().SetTrigger("a");
    }

}
