using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using TMPro;
using RTLTMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.Android;
using System.Collections;
using Photon.Realtime;
using System.Text;

public class ChatManager : MonoBehaviourPunCallbacks, IChatClientListener
{
    [SerializeField] private RTLTextMeshPro timerText;  // Text UI element for displaying timer
    private float countdownTime = 10f;  // Total countdown time in seconds

    private float y;
    private Texture2D selectedTexture;
    [SerializeField] private GameObject imagebtn;
    [SerializeField] private RTLTextMeshPro current_room;
    [SerializeField] private TMP_InputField newchanelname;
    [SerializeField] private GameObject grupprefab;
    [SerializeField] private GameObject content_groups;
    [SerializeField] private GameObject chatfab;
    [SerializeField] private GameObject imageprefab;
    [SerializeField] private GameObject content_chats;
    [SerializeField] private TMP_InputField chatInputField;
    [SerializeField] private GameObject chatInputField_game;
    private string playerName = "Player";

    private ChatClient chatClient;
    public struct chat
    {
        public string senderId;
        public string sender;
        public string message;
        public string chanel;
        public bool isprivate;
        public string reseverId;
        public ChatType chatType;
    }
    public enum ChatType
    {
        Image,
        text
    }
    private string currentImageRecipient;
    private List<chat> chatlist = new List<chat>();
    private static readonly string AppIdChat = "a441e1a9-b958-4573-8947-ba75aaaa4f8e";
    void Start()
    {
        y = chatInputField.GetComponent<RectTransform>().anchoredPosition.y;
        if(SecurePlayerPrefs.GetString("p")=="")
        {
            SecurePlayerPrefs.SetString("p", "User_"+GetCharacterRandomString(8));
        }
        playerName = SecurePlayerPrefs.GetString("p");
        PhotonNetwork.AddCallbackTarget(this); // اضافه کردن این خط
        chatClient = new ChatClient(this);
        chatClient.Connect(AppIdChat, "1.0", new Photon.Chat.AuthenticationValues(playerName));
        chatClient.SetOnlineStatus(0);
        AddChannelToPrefs("LobbyChat");
        update_chanels();
        StartCoroutine(run());
    }
    IEnumerator run()
    {
        chatInputField_game.SetActive(false);
        // Countdown loop
        while (countdownTime > 0)
        {
            timerText.text = getToastMessage.Translate("Connecting to Chat") + countdownTime.ToString("0");
            yield return new WaitForSeconds(1f);  // Wait for 1 second
            countdownTime--;  // Decrease countdown time by 1 second
        }

        chatInputField_game.SetActive(true);
        timerText.text = "";
        chatClient.Subscribe(new string[] { "LobbyChat" });
    }
    public override void OnJoinedLobby()
    {
        if (chatClient != null)
        {
            chatClient.Subscribe(new string[] { "LobbyChat" });
        }
    }

    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }
    }

    public void SendChatMessage()
    {
        if (!string.IsNullOrEmpty(chatInputField.text) && chatClient != null)
        {
            string message = chatInputField.text;
            if(message.Contains(";") )
            {
                Toast._ShowAndroidToastMessage("از کلمات ممنوعه استفاده نکنید ");
                return;
            }
            if (current_room.text.StartsWith("user:"))
            {
                message = "user;" + playerName + ";" + PlayerPrefs.GetString("playerName") + ";" + "text" + ";" + message + ";" + "100;" + SecurePlayerPrefs.GetString(current_room.text.Split(":")[1]);
                chatClient.SendPrivateMessage(SecurePlayerPrefs.GetString(current_room.text.Split(":")[1]), message);
                chatInputField.text = "";
                return;
            }
            message = "group;" + playerName + ";" + SecurePlayerPrefs.GetString("playerName") + ";" + "text" + ";" + message + ";" + "100";
            if (chatClient.PublicChannels.ContainsKey("LobbyChat"))
            {
                chatClient.PublishMessage(current_room.text, message);
                chatInputField.text = "";
            }
            else
            {
                AddChannelToPrefs("LobbyChat");
                chatClient.Subscribe(GetChannelListFromPrefs().ToArray());
                chatClient.PublishMessage("LobbyChat", message);
                chatInputField.text = "";
            }
        }
    }

    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    {
    }

    public void OnChatStateChange(ChatState state)
    {
    }

    public void OnDisconnected()
    {
    }

    public void OnConnected()
    {
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            add_chat(messages[i].ToString(), senders[i], senders[i], channelName, false, "manypeople", getChattype(messages[i].ToString()));
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        string[] messageParts = message.ToString().Split(';');
        string playerNameInMessage = messageParts[1];
        string currentName = messageParts[2];
        string targetName = messageParts[6];
        if (sender == playerName)
        {
            if (SecurePlayerPrefs.GetString(targetName) == "")
            {
                AddChannelToPrefs(channelName.Split(':')[1]);
                SecurePlayerPrefs.SetString(channelName.Split(':')[1], targetName);
                SecurePlayerPrefs.SetString(targetName, channelName.Split(':')[1]);
            }
            add_chat(message.ToString(), sender, sender, channelName, true, channelName.Split(":")[1],getChattype(message.ToString()), false);
        }
        else
        {
            if (SecurePlayerPrefs.GetString(currentName) == "")
            {
                AddChannelToPrefs(sender);
                SecurePlayerPrefs.SetString(sender, currentName);
                SecurePlayerPrefs.SetString(currentName, sender);
            }
            add_chat(message.ToString(), channelName.Split(':')[1], channelName.Split(':')[1], channelName, true, channelName.Split(":")[0],getChattype(message.ToString()));
        }
        update_chanels();
    }


    public void OnSubscribed(string[] channels, bool[] results)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            AddChannelToPrefs(channels[i]);
        }
        update_chanels();
    }
    private void update_chanels()
    {
        for (int i = 1; i < content_groups.transform.childCount; i++)
        {
            Destroy(content_groups.transform.GetChild(i).gameObject);
        }
        foreach (string s in GetChannelListFromPrefs())
        {
            print(s);
            if (s.StartsWith("User_"))
            {
                add_("user:" + SecurePlayerPrefs.GetString(s), false);
            }
            else
            {
                add_(s, true);
            }
        }
    }
    public void OnUnsubscribed(string[] channels)
    {
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        print(user + " To Status : " + status);
    }

    public void OnUserSubscribed(string channel, string user) 
    {
    }

    public void OnUserUnsubscribed(string channel, string user) { }
    private void OnApplicationQuit()
    {
        //chatClient.SetOnlineStatus(1);
    }
    private void add_(string s, bool a)
    {
        if (s != "")
        {
            if (a)
            {
                AddChannelToPrefs(s);
            }
            GameObject g = Instantiate(grupprefab, content_groups.transform);
            g.GetComponent<Button>().AddEventListener(s, click);
            g.GetComponentInChildren<RTLTextMeshPro>().text = s;
        }
    }
    public void create_chanel(string chanelname = null)
    {
        ChannelCreationOptions channelCreationOptions = new ChannelCreationOptions()
        {
            MaxSubscribers = 10,
            PublishSubscribers = true
        };
        if (chanelname == "")
        {
            AddChannelToPrefs(newchanelname.text);
            chatClient.Subscribe(newchanelname.text, 0, -1, channelCreationOptions);
             string  message = "group;" + playerName + ";" + PhotonNetwork.LocalPlayer.NickName + ";" + "text" + ";" + "GROUP created" + ";" + "100;" +newchanelname.text;
            chatClient.PublishMessage(newchanelname.text, message);
        }
        else
        {
            if (chanelname.Length != 0)
            {
                AddChannelToPrefs(chanelname);
                chatClient.Subscribe(chanelname, 0, -1, channelCreationOptions);
                string message = "group;" + playerName + ";" + PhotonNetwork.LocalPlayer.NickName + ";" + "text" + ";" + "GROUP created" + ";" + "100;" + chanelname;
                chatClient.PublishMessage(chanelname, "GROUP created");
            }
        }
    }
    private void click(string roomName)
    {
        current_room.text = roomName;
        if (roomName.Contains("user:"))
        {
            roomName = roomName.Split(":")[1];
            imagebtn.SetActive(true);
        }
        else
        {
            imagebtn.SetActive(false);
        }
        chatClient.Subscribe(GetChannelListFromPrefs().ToArray(), 0);
        for (int i = 0; i < content_chats.transform.childCount; i++)
        {
            content_chats.transform.GetChild(i).gameObject.SetActive(false);
        }
        foreach (chat ch in chatlist)
        {
            if (ch.isprivate)
            {
                if (ch.senderId == SecurePlayerPrefs.GetString(roomName) || ch.reseverId == SecurePlayerPrefs.GetString(roomName))
                {
                    add_chat(ch.message, ch.sender, ch.senderId, ch.chanel, ch.isprivate, ch.reseverId,ch.chatType, true);
                }
            }
            else
            {
                if (ch.chanel == roomName)
                {
                    add_chat(ch.message, ch.sender, ch.senderId, ch.chanel, ch.isprivate, ch.reseverId,ch.chatType);
                }
            }
        }
    }
    private void add_chat(string Message, string SenderName, string SenderId, string chanele, bool isprivatee, string ReseverId, ChatType type, bool dontckeck = false)
    {
        GameObject g = null;
        if (getChattype(Message) == ChatType.text)
        {
            if(string.IsNullOrEmpty(Message.Split(';')[4]))
            {
                return;
            }
            g = Instantiate(chatfab, content_chats.transform);
            g.transform.GetChild(0).GetComponent<RTLTextMeshPro>().text = Message.Split(';')[4];

            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in Message.Split(';')[2])
            {
                stringBuilder.Append(c.ToString());
            }

            g.transform.GetChild(1).GetComponent<RTLTextMeshPro>().text = stringBuilder.ToString();



            g.transform.GetChild(3).GetComponent<RTLTextMeshPro>().text = Message.Split(';')[1];
        }
        else
        {
            if(Message.Split(';')[4]== "endimage")
            {
                g = Instantiate(imageprefab, content_chats.transform);
                g.transform.GetChild(0).GetComponent<RawImage>().texture = Base64ToTexture(currentImageRecipient);
                g.transform.GetChild(1).GetComponent<RTLTextMeshPro>().text = Message.Split(';')[2];
                g.transform.GetChild(3).GetComponent<RTLTextMeshPro>().text = Message.Split(';')[1];
                currentImageRecipient = null;
            }
            else
            {
                currentImageRecipient += Message.Split(';')[4];
                return;
            }
        }




        if (!chatlist.Any(c => c.senderId == SenderId && c.message == Message))
        {
            chatlist.Add(new chat() { sender = Message.Split(';')[2], senderId = Message.Split(';')[1], message = Message, chanel = chanele, isprivate = isprivatee,chatType = type, reseverId = ReseverId });
        }
        if (playerName == Message.Split(';')[1] || isprivatee)
        {
            g.transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            g.transform.GetChild(2).GetComponent<Button>().AddEventListener(PhotonNetwork.LocalPlayer.NickName, SenderName, Message.Split(';')[2], addfreand);
        }
        if (isprivatee && current_room.text.StartsWith("user:"))
        {
            if (dontckeck)
            {
                g.SetActive(true);
                return;
            }
            if (Message.Split(';')[1] == SecurePlayerPrefs.GetString(current_room.text.Split(":")[1]) || ReseverId == SecurePlayerPrefs.GetString(current_room.text.Split(":")[1]))
            {
                g.SetActive(true);
            }
            else
            {
                g.SetActive(false);
            }

        }
        else
        {
            if (current_room.text != chanele)
            {
                g.SetActive(false);
            }
        }
    }
    private void addfreand(string sender, string target, string targetName)
    {
        string message = "user;" + playerName + ";" + PhotonNetwork.LocalPlayer.NickName + ";" + "text" + ";" + "درخواست دوستی ارسال کرده است " + ";" + "100;" + targetName;
        chatClient.SendPrivateMessage(target, message);
    }
    private void AddChannelToPrefs(string channelName)
    {
        string key = "ChannelList";

        string savedChannels = SecurePlayerPrefs.GetString(key);

        List<string> channelList = new List<string>(savedChannels.Split(';'));

        if (!channelList.Contains(channelName))
        {
            channelList.Add(channelName);
        }
        string updatedChannels = string.Join(";", channelList.ToArray());
        SecurePlayerPrefs.SetString(key, updatedChannels);
    }
    public List<string> GetChannelListFromPrefs()
    {
        string key = "ChannelList";
        string savedChannels = SecurePlayerPrefs.GetString(key);
        List<string> channelList = new List<string>(savedChannels.Split(';'));
        return channelList;
    }
    private string GetCharacterRandomString(int len)
    {
        string path = Path.GetRandomFileName();
        path = path.Replace(".", "");
        return path.Substring(0, len);
    }
    public void open_galary()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite) || !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
            return;
        }
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                long fileSize = new FileInfo(path).Length;
                if (fileSize > 5 * 1024 * 1024)
                {
                    Debug.LogWarning("File size is too large. Reducing quality...");
                    selectedTexture = LoadTextureFromFile(path);
                }
                else
                {
                    Toast._ShowAndroidToastMessage("حجم فایل باید کمتر از 5 مگابایت باشد ");
                    return;
                }
                string base64String = TextureToBase64(selectedTexture, 50);
                SendLargeMessageInChunks(base64String, SecurePlayerPrefs.GetString(current_room.text.Split(":")[1]));
            }
        }, "Select an image", "image/*");
    }
    public string TextureToBase64(Texture2D texture, int  quality = 50)
    {
        byte[] imageBytes = texture.EncodeToJPG(quality);
        return Convert.ToBase64String(imageBytes);
    }
    private void SendLargeMessageInChunks(string base64String, string recipient, int chunkSize = 30000)
    {
        int totalLength = base64String.Length;
        for (int i = 0; i < totalLength; i += chunkSize)
        {
            // گرفتن یک بخش از رشته base64
            string chunk = base64String.Substring(i, Mathf.Min(chunkSize, totalLength - i));
            string message = "user;" + playerName + ";" + PhotonNetwork.LocalPlayer.NickName + ";" + "image" + ";" + chunk + ";" + "100;" + SecurePlayerPrefs.GetString(current_room.text.Split(":")[1]);

            // ارسال بخش
            chatClient.SendPrivateMessage(recipient, message);
            print("sended Message ");
        }
        string  messag = "user;" + playerName + ";" + PhotonNetwork.LocalPlayer.NickName + ";" + "image" + ";" + "endimage" + ";" + "100;" + SecurePlayerPrefs.GetString(current_room.text.Split(":")[1]);
        chatClient.SendPrivateMessage(recipient, messag);
    }

    private Texture2D LoadTextureFromFile(string path)
    {
        byte[] imageBytes = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);
        return texture;
    }
    private Texture2D Base64ToTexture(string base64String)
    {
        byte[] imageBytes = Convert.FromBase64String(base64String);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);
        return texture;
    }
    private ChatType getChattype(string message)
    {
        return message.Contains("text") ? ChatType.text : ChatType.Image;
    }

    public void On_keyboard()
    {
        StartCoroutine("OnKeyboardOpen");
    }
    IEnumerator OnKeyboardOpen()
    {
        yield return new WaitForSeconds(0.3f);
        chatInputField.GetComponent<RectTransform>().anchoredPosition = new Vector2(chatInputField.GetComponent<RectTransform>().anchoredPosition.x, y + GetKeyboardSize()+130);
        StopCoroutine("OnKeyboardOpen");
    }

    public void OnKeyboardClose()
    {
        chatInputField.GetComponent<RectTransform>().anchoredPosition = new Vector2(chatInputField.GetComponent<RectTransform>().anchoredPosition.x, y);
        chatInputField.transform.position = new Vector3(chatInputField.transform.position.x, y, chatInputField.transform.position.z);

    }
    private int GetKeyboardSize()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return 0;
        }
        using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");

            using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
            {
                View.Call("getWindowVisibleDisplayFrame", Rct);

                return Screen.height - Rct.Call<int>("height");
            }
        }
    }
}