using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using RTLTMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

public class player_online : MonoBehaviour,IPunOwnershipCallbacks
{
    private PhotonView photonViewa;
    [SerializeField] GameObject chose_hokm;
    [SerializeField] sorta_online s;
    public TextMeshProUGUI name_text;
    public bool is_but;
    public int nobat;
    private PhotonView remindphotonview;
    public Dictionary<string, string[]> pos = new Dictionary<string, string[]>() { { "q (2)", new string[] { "q (2)", "q (3)", "q (1)", "q" } }, { "q (1)", new string[] { "q (1)", "q (3)", "q (2)", "q"} }, { "q", new string[] { "q", "q (3)" , "q (1):-90", "q (2):90" } }, { "q (3)", new string[] { } } };
    public Dictionary<string, string[]> apos = new Dictionary<string, string[]>() { { "1", new string[] { "a1", "a4", "a2", "a3" } }, { "2", new string[] { "a2", "a4", "a1", "a4", "a4", "a3" } }, { "3", new string[] { "a4", "a3", "a1", "a2", "a4", "a1" } }, { "4", new string[] { } } };
    public List<GameObject> ls = null;
    public List<GameObject> ls2 = null;
    public List<GameObject> nm;
    RectTransform canvasRect;
    void Start()
    {
        Time.timeScale = 1.25f;
        photonViewa = GetComponent<PhotonView>();
        PhotonNetwork.AddCallbackTarget(this);
        Vector3 worldPositionOfUnparentedObject = transform.root.transform.GetChild(0).transform.position;
        Vector3 localPositionForParentedObject = transform.GetChild(1).transform.parent.InverseTransformPoint(worldPositionOfUnparentedObject);
        transform.GetChild(1).transform.localPosition = localPositionForParentedObject;
    }
    int c = 0;
    void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    [PunRPC]
    public void I_hakem()
    {
        if(remindphotonview==photonViewa)
        {
            chose_hokm.SetActive(!chose_hokm.activeInHierarchy);
        }
    }
    public void close()
    {
        chose_hokm.SetActive(false);
    }
    private void choseRandomHokm()
    {
        GameObject.Find("main cards ").GetComponent<manager_online>().chose_hokm(Random.Range(0, 3));
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        throw new System.NotImplementedException();
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView == photonViewa)
        {
            if (targetView.IsMine)
            {
                remindphotonview = targetView;

                int currentIndex = transform.GetSiblingIndex();
                transform.GetChild(0).localScale = new Vector3(1.8f, 1.8f, 1);
                string[] l2 = apos[transform.GetChild(0).GetChild(0).GetChild(0).name];
                string[] l = pos[gameObject.name];
                List<GameObject> ls = new List<GameObject>();

                foreach (string s in l)
                {
                    string objName = s.Split(':')[0].Trim(); // جدا کردن نام آبجکت از مقدار چرخش
                    GameObject obj = GameObject.Find(objName);

                    if (obj != null)
                    {
                        ls.Add(obj);
                    }
                }

                for (int i = 0; i < ls.Count - 1; i += 2)
                {
                    Vector3 tempPosition = ls[i].transform.position;
                    Quaternion tempRotation = ls[i].transform.GetChild(0).transform.rotation;

                    // چرخش سفارشی بررسی می‌شود
                    for (int j = 0; j < l.Length; j++)
                    {
                        string[] parts = l[j].Split(':');
                        string objName = parts[0].Trim();
                        float customRotation = 0;

                        // اگر مقدار چرخش در رشته موجود باشد
                        if (parts.Length > 1 && float.TryParse(parts[1], out customRotation))
                        {
                            if (ls[j].name == objName)
                            {
                                ls[j].transform.GetChild(0).transform.rotation = Quaternion.Euler(0, 0, customRotation);
                            }
                        }
                        else
                        {
                            // چرخش و جابجایی معمولی برای سایر آبجکت‌ها
                            ls[i].transform.GetChild(0).transform.position = ls[i + 1].transform.position;
                            ls[i + 1].transform.GetChild(0).transform.position = tempPosition;

                            ls[i].transform.GetChild(0).transform.rotation = ls[i + 1].transform.rotation;
                            ls[i + 1].transform.GetChild(0).transform.rotation = tempRotation;
                        }
                    }
                }
                if(PhotonNetwork.LocalPlayer.ActorNumber==2 || PhotonNetwork.LocalPlayer.ActorNumber == 3)
                {
                    GameObject target = GameObject.Find("1");
                    GameObject g = GameObject.Find("4");

                    // Swap positions
                    Vector3 tempPosition = g.transform.position;
                    g.transform.position = target.transform.position;
                    target.transform.position = tempPosition;

                    // Swap rotations
                    Quaternion tempRotation = g.transform.rotation;
                    g.transform.rotation = target.transform.rotation;
                    target.transform.rotation = tempRotation;
                }
                foreach (string c in l2)
                {
                    ls2.Add(GameObject.Find(c));
                }
                for (int i = 0; i < ls2.Count - 1; i += 2)
                {
                    Vector3 tempPosition = ls2[i].transform.position;
                    ls2[i].transform.position = ls2[i + 1].transform.position;
                    ls2[i + 1].transform.position = tempPosition;
                }
                transform.SetSiblingIndex(9);
                transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(1).gameObject.SetActive(false);
            }
            transform.GetChild(1).GetChild(3).GetComponent<Button>().AddEventListener(0, open_chat);
        }
        c++;   
        if(c==8)
        {
            List<GameObject> nm = GameObject.FindGameObjectsWithTag("Finish").ToList();
            nm.Remove(transform.GetChild(0).GetChild(0).GetChild(0).gameObject);
            GameObject parentname = transform.GetChild(1).GetChild(2).gameObject;
            MatchObjectsByPosition(nm, parentname);
        }
    }
    void SwapNamesBasedOnPermutation(int permutationIndex, Transform parentname)
    {
        var names = new string[3];
        var number = new string[3];
        var sprites = new Sprite[3];
        for (int i = 0; i < 3; i++)
        {
            names[i] = parentname.GetChild(i).GetChild(1).GetComponent<RTLTextMeshPro>().text;
            number[i] = parentname.GetChild(i).GetChild(2).GetComponent<RTLTextMeshPro>().text;
            sprites[i] = parentname.GetChild(i).GetChild(0).GetComponent<Image>().sprite;
        }
        var permutations = new[] { new[] { 0, 1, 2 }, new[] { 0, 2, 1 }, new[] { 1, 0, 2 },
                               new[] { 1, 2, 0 }, new[] { 2, 0, 1 }, new[] { 2, 1, 0 } };
        var permutation = permutations[permutationIndex - 1];
        for (int i = 0; i < 3; i++)
        {
            parentname.GetChild(i).GetChild(2).GetComponent<RTLTextMeshPro>().text = number[permutation[i]];
            parentname.GetChild(i).GetChild(1).GetComponent<RTLTextMeshPro>().text = names[permutation[i]];
            parentname.GetChild(i).GetChild(0).GetComponent<Image>().sprite = sprites[permutation[i]];
        }
    }
    string DeterminePosition(Vector3 position)
    {
        float canvasWidth = Screen.width;
        float canvasHeight = Screen.height;
        if (position.y > canvasHeight * 0.75f)
        {
            return "TopCenter";
        }
        else if (position.y < canvasHeight * 0.25f)
        {
            return "BottomCenter";
        }
        else if (position.x < canvasWidth * 0.25f)
        {
            return "LeftCenter";
        }
        else if (position.x > canvasWidth * 0.75f)
        {
            return "RightCenter";
        }
        else
        {
            return "Center";
        }
    }

    void MatchObjectsByPosition(List<GameObject> finishObjects, GameObject parentObject)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("a"))
        {
            string childPosition = DeterminePosition(obj.transform.position);
            foreach (GameObject finish in finishObjects)
            {
                string finishPosition = DeterminePosition(finish.transform.position);

                if (childPosition == finishPosition)
                {
                    GameObject child = obj.gameObject;
                    if (finish != finish.GetComponent<PhotonView>() && child != null)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (char c in (string)finish.GetComponent<PhotonView>().Owner.CustomProperties["myname"])
                        {
                            stringBuilder.Append(c.ToString());
                        }
                        child.transform.GetChild(1).GetComponent<RTLTextMeshPro>().text = stringBuilder.ToString();
                        child.transform.GetChild(2).GetComponent<RTLTextMeshPro>().text = finish.GetComponent<PhotonView>().Owner.ActorNumber.ToString();
                             Sprite mySprite = Resources.Load<Sprite>("images/" + finish.GetPhotonView().Owner.CustomProperties["profilePicture"]);
                             if (mySprite == null)
                             {
                                 mySprite = Resources.Load<Sprite>("images/c_0"); // آدرس عکس پیش‌فرض
                             }
                             child.transform.GetChild(0).GetComponent<Image>().sprite = mySprite;
                        break;
                    }
                }
            }
        }
    }
    GameObject FindNearestObject(GameObject target, List<GameObject> objects)
    {
        GameObject nearest = null;
        float minDistance = float.MaxValue;

        foreach (GameObject obj in objects)
        {
            float distance = Vector3.Distance(target.transform.position, obj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = obj;
            }
        }

        return nearest;
    }
    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
    }
    public void call_send_chat(string s)
    {
        object[] parametrs = new object[] { s, PhotonNetwork.LocalPlayer.ActorNumber.ToString() };
        photonViewa.RPC("send_message", RpcTarget.AllBuffered, parametrs);
    }
    [PunRPC]
    public void send_message(string s,string senderName)
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("a"))
        {
            RTLTextMeshPro te = g.transform.GetChild(1).GetComponent<RTLTextMeshPro>();
            RTLTextMeshPro actorNumber = g.transform.GetChild(2).GetComponent<RTLTextMeshPro>();
            if (actorNumber.text == senderName)
            {
                te.transform.parent.GetComponent<Animator>().SetTrigger("send");
                te.transform.parent.GetChild(3).GetChild(0).GetComponent<RTLTextMeshPro>().text = s;
            }
        }
    }
    public void open_chat(int i)
    {
        if(photonViewa.IsMine)
        {
            transform.GetChild(1).GetChild(8).gameObject.SetActive(!transform.GetChild(1).GetChild(8).gameObject.activeInHierarchy);
        }
    }
    public void change_model()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("mycard"))
        {
            g.GetComponent<Button>().enabled = !g.GetComponent<Button>().enabled;
        }
    }
    public void open_setting(GameObject g)
    {
        g.GetComponent<Animator>().SetBool("a", !g.GetComponent<Animator>().GetBool("a"));
    }
    public void exite()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(1);
    }
    public IEnumerator MoveAndScale(GameObject g, GameObject target)
    {
        float speed = 5.0f;
        Vector3 targetPosition = target.transform.position;
        Vector3 targetScale = new Vector3(0.55f, 0.55f, g.transform.localScale.z);

        while (Vector3.Distance(g.transform.position, targetPosition) > 0.01f ||
               Vector3.Distance(g.transform.localScale, targetScale) > 0.01f)
        {
            g.transform.position = Vector3.Lerp(g.transform.position, targetPosition, speed * Time.deltaTime);
            g.transform.rotation = Quaternion.Lerp(g.transform.rotation, target.transform.rotation, speed * Time.deltaTime);
            g.transform.localScale = Vector3.Lerp(g.transform.localScale, targetScale, speed * Time.deltaTime);

            yield return null;
        }

        // Swap the positions of the two GameObjects
        Vector3 tempPosition = g.transform.position;
        g.transform.position = target.transform.position;
        target.transform.position = tempPosition;

        // If you want to swap the scales too
        Vector3 tempScale = g.transform.localScale;
        g.transform.localScale = target.transform.localScale;
        target.transform.localScale = tempScale;
    }
    public static int GetNumberFromString(string input)
    {
        if (int.TryParse(input, out int number))
        {
            return number;
        }
        // اگر عدد موجود نبود، مقدار 0 برمی‌گردد
        return 0;
    }
}

