using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using random = UnityEngine.Random;
using TMPro;
using RTLTMPro;

public class manager_online : MonoBehaviourPunCallbacks
{
    public Dictionary<string, int> pos = new Dictionary<string, int>()
{
    { "q (2)", 3},
    { "q (1)", 2 },
    { "q", 1 },
    { "q (3)", 4 }
};
    public Dictionary<int, string> pla = new Dictionary<int, string>();


    public List<string> leaves_name;
    [SerializeField] private GameObject waitforstart;
    private PhotonView photonViewa;
    [SerializeField] Image image;
    [SerializeField] Sprite[] sprites;
    int index = 52;
    public GameObject hakem;
    public int h = 6;
    public bool j;
    public bool p;
    [SerializeField] Transform[] positions;
    void Start()
    {
        photonViewa = GetComponent<PhotonView>();    
        if (PhotonNetwork.IsMasterClient)
        {
            Player[] players = PhotonNetwork.PlayerList;
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("player");
            for (int i = 0; i < gameObjects.Length; i++)
            {
                gameObjects[i].GetComponent<PhotonView>().TransferOwnership(players[i]);
                gameObjects[i].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<PhotonView>().TransferOwnership(players[i]);
            }

            StartCoroutine(main_pakhsh(gameObjects[random.Range(0, gameObjects.Length)]));
        }
    }
    void Update()
    {
        if (j)
        {
            j = false;
            clear_all();
        }
        if (p)
        {
            p = false;
            StartCoroutine(pakhsh(7));
        }
    }
    public void chose_hokm(int i)
    {
        object[] parametr = {i,hakem.name};
        photonView.RPC("calla", RpcTarget.All,parametr);
        photonView.RPC("pakhsh", RpcTarget.All, 8);
        if(hakem.GetComponent<PhotonView>().Owner==null)
        {
            // Random Move
            foreach (sorta_online sort in FindObjectsOfType<sorta_online>())
            {
                PhotonView view = sort.GetComponent<PhotonView>();
                if (view.IsMine)
                {
                    GameObject.Find(sort.name).GetComponent<PhotonView>().RPC("randommoveAdmin", view.Owner, hakem.transform.GetChild(0).GetChild(0).GetChild(0).name);
                    break;
                }
            }
        }
        else
        {
            photonView.RPC("vb", RpcTarget.All, hakem.name);
        }
    }
    [PunRPC]
    public void calla(int g ,string ha)
    {
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("f"))
        {
            item.GetComponent<Image>().sprite = sprites[g];
            item.GetComponent<Image>().color = Color.white;
        }
        h = g;
        GameObject.Find("a").GetComponent<a_online>().ff(ha);
    }
    [PunRPC]
    IEnumerator vb(string  s)
    {
        yield return new WaitForSeconds(2);
        hakem = GameObject.Find(s);
        hakem.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<sorta_online>().m();
    }
    [PunRPC]
    void SwapChildTransforms(Transform parent, int indexA, int indexB)
    {

        // گرفتن فرزندان
        Transform childA = parent.GetChild(indexA);
        Transform childB = parent.GetChild(indexB);

        // گرفتن اندیس‌های اصلی
        int childAIndex = childA.GetSiblingIndex();
        int childBIndex = childB.GetSiblingIndex();

        // جابه‌جایی اندیس‌ها
        childA.SetSiblingIndex(childBIndex);

    }
    [PunRPC]
    IEnumerator pakhsh(int cardnumber)
    {
        GameObject g;
        GameObject[] ls_player = GameObject.FindGameObjectsWithTag("player");
        for (int i = 0; i < cardnumber; i++)
        {
            for (int a = 0; a < ls_player.Length; a++)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                g = Instantiate(transform.GetChild(0).gameObject, ls_player[a].transform.GetChild(0).GetChild(0).GetChild(0).transform);
                g.GetComponent<card_online>().enabled = true;
                g.GetComponent<Image>().enabled = true;
                g.GetComponent<card_online>().myparent = g.transform.parent.gameObject;
                g.GetComponent <card_online>().s = g.transform.parent.name;
                if (g.transform.parent.GetComponent<PhotonView>().Owner != null && g.transform.parent.GetComponent<PhotonView>().IsMine)
                {
                    g.GetComponent<card_online>().FlipCard();
                }
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
        yield return new WaitForSeconds(2);
    }
    public void Move(GameObject g, int r)
    {
        Destroy(g);
    }
    [PunRPC]
    public  IEnumerator main_pakhsh(GameObject Hakem)
    {
        photonView.RPC("waittostart", RpcTarget.All);
        yield return new WaitForSeconds(4);
        hakem = Hakem;
        int childCount = 52;
        List<int> randomIndices = new List<int>();
        for (int i = 0; i < childCount; i++)
        {
            randomIndices.Add(i);
        }
        for (int i = 0; i < randomIndices.Count; i++)
        {
            int temp = randomIndices[i];
            int randomIndex = random.Range(i, randomIndices.Count);
            randomIndices[i] = randomIndices[randomIndex];
            randomIndices[randomIndex] = temp;
        }
        photonView.RPC("ShuffleChildrenRPC", RpcTarget.All, randomIndices.ToArray());
        photonView.RPC("pakhsh", RpcTarget.All, 5);
        if (Hakem.GetComponent<PhotonView>().Owner == null)
        {
            print("Owner is null");
            chose_hokm(random.Range(0, 4));
        }
        photonView.RPC("set_haken", RpcTarget.All, hakem.name);
        StartCoroutine(waitforchose());
    }
    IEnumerator waitforchose()
    {
        yield return new WaitForSeconds(7);
        // close chose hakem 
        photonView.RPC("close", RpcTarget.All);
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("f"))
        {
            if(item.GetComponent<Image>().color == new Color(0, 0, 0, 0))
            {
                chose_hokm(random.Range(0, 4));
                break;
            }
        }
    }
    [PunRPC]
    private void close()
    {
        if (hakem.GetComponent<PhotonView>().Owner != null)
        {
            hakem.GetComponent<player_online>().close();
        }
    }
    [PunRPC]
    private void waittostart()
    {
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("f"))
        {
            item.GetComponent<Image>().color = new Color(0,0,0,0);
        }
        waitforstart.SetActive(true);
    }
    [PunRPC]
    public void set_haken(string s )
    {
        waitforstart.SetActive(false);
        hakem = GameObject.Find(s);
        if (hakem.GetComponent<PhotonView>().Owner == null)
        {
            int playerId = pos[hakem.name];
            string playerName = pla[playerId];
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("a"))
            {
                RTLTextMeshPro te = g.transform.GetChild(1).GetComponent<RTLTextMeshPro>();
                RTLTextMeshPro ActorNumber = g.transform.GetChild(2).GetComponent<RTLTextMeshPro>();
                if (ActorNumber.text == playerId.ToString())
                {
                    g.transform.GetChild(0).GetChild(1).GetComponent<Image>().enabled = true;
                }
                else
                {
                    g.transform.GetChild(0).GetChild(1).GetComponent<Image>().enabled = false;
                }
            }
            // get name of hakem .. 
            return;
        }
        if (hakem.GetComponent<PhotonView>().Owner != null)
        {
            hakem.GetComponent<player_online>().I_hakem();
        }
        else
        {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("a"))
            {
                RTLTextMeshPro te = g.transform.GetChild(1).GetComponent<RTLTextMeshPro>();
                RTLTextMeshPro ActorNumber = g.transform.GetChild(2).GetComponent<RTLTextMeshPro>();
                if (ActorNumber.text == hakem.GetComponent<PhotonView>().Owner.ActorNumber.ToString())
                {
                    g.transform.GetChild(0).GetChild(1).GetComponent<Image>().enabled = true;
                }
                else
                {
                    g.transform.GetChild(0).GetChild(1).GetComponent<Image>().enabled = false;
                }
            }
        }
    }
    [PunRPC]
    void ShuffleChildrenRPC(int[] randomIndices)
    {
        List<Transform> shuffledChildren = new List<Transform>();
        for (int i = 0; i < randomIndices.Length; i++)
        {
            shuffledChildren.Add(transform.GetChild(randomIndices[i]));
        }
        for (int i = 0; i < shuffledChildren.Count; i++)
        {
            shuffledChildren[i].SetSiblingIndex(i);
        }
    }
        public void clear_all()
    {
        jam();
        jam();
        jam();
        jam();
        jam();
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void jam()
    {
        GameObject g;
        GameObject[] ls_player = GameObject.FindGameObjectsWithTag("player");
        for (int a = 0; a < ls_player.Length; a++)
        {
            g = ls_player[a].transform.GetChild(0).GetChild(0).GetChild(0).gameObject; // sorta 
            for (int i = 0; i < g.transform.childCount; i++)
            {
                print(i);
                try
                {
                    DestroyImmediate(g.transform.GetChild(i).gameObject);
                }
                catch
                {

                }
            }
        }
    }


    [PunRPC]
    public void set_name(PhotonView pw, TextMeshProUGUI name_text)
    {
        if (pw.IsMine)
        {
            name_text.text = PhotonNetwork.NickName;
        }
        else
        {
            name_text.enabled = false;
        }
    }
    [PunRPC]
    public void set_hakem(GameObject g)
    {
        hakem = g;

    }
    [PunRPC]
    public void call_sort()
    {
        GameObject[] ls_player = GameObject.FindGameObjectsWithTag("player");
        for (int i = 0; i < ls_player.Length; i++)
        {
            ls_player[i].GetComponentInChildren<sorta_online>().SortCard();
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        pla.Add(otherPlayer.ActorNumber, (string)otherPlayer.CustomProperties["myname"]);
        leaves_name.Add(otherPlayer.NickName);
        GameObject.Find("canvas").GetComponent<saound>().leaves.Add(otherPlayer.ActorNumber);
        Toast._ShowAndroidToastMessage("بازی را ترک کرد " + otherPlayer.NickName);
        base.OnPlayerLeftRoom(otherPlayer);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }
}
