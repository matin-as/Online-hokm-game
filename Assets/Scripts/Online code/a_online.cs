using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Linq;
using Photon.Realtime;
using RTLTMPro;
using UnityEngine.UI; 


public class a_online : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject mi;
    [SerializeField] private GameObject pa;


    [SerializeField] private GameObject end;
    private PhotonView photonview;
    int dast = 0;
    public int c = 0;
    public int z;
    public  List<GameObject> ls;
    int b = 0;
    GameObject barande;
    private GameObject maincard;
    bool l = true;
    // Start is called before the first frame update
    void Start()
    {
        photonview = GetComponent<PhotonView>();
        maincard = GameObject.Find("main cards ");
    }
    public void ff(string s)
    {
        ls.Clear();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Finish"))
        {  
            ls.Add(go);
        }
        int[] desiredOrder = { 4, 2, 1, 3 };
        // مرتب‌سازی لیست بر اساس ترتیب دلخواه
        ls = ls
            .OrderBy(go => System.Array.IndexOf(desiredOrder, int.Parse(go.name)))
            .ToList();
        ls = RotateList(ls, ls.IndexOf(GameObject.Find(s).transform.GetChild(0).GetChild(0).GetChild(0).gameObject)); 
    }
    public void add(int h, string s)
    {
        l = true;
        if (b == ls.Count)
        {
            b = 0;
        }
        if (c % 5 == 0)
        {
            photonview.RPC("RPCZ",RpcTarget.AllBuffered,h);
        }
        if(photonview.IsMine)
        {
            photonview.RPC("RPCC", RpcTarget.AllBuffered);
        }
        if (c == 4 && c != 0)
        {
            if(photonview.IsMine)
            {
                StartCoroutine(xc());
            }
        }
        if (l)
        {
            if (z != 6)
            {
                l = false;
                b++;
                StartCoroutine(cv());
            }
        }
    }
    static void Swap<T>(List<T> list, int index1, int index2)
    {
        T temp = list[index1];
        list[index1] = list[index2];
        list[index2] = temp;
    }
    private List<GameObject> RotateList(List<GameObject> list, int win)
    {
        int count = list.Count;
        List<GameObject> rotatedList = new List<GameObject>(count);

        for (int i = 0; i < count; i++)
        {
            int newIndex = (i + win) % count;
            rotatedList.Add(list[newIndex]);
        }

        return rotatedList;
    }
    IEnumerator cv()
    {
        yield return new WaitForSeconds(1.6f);// 2 
        if (b == -1)
        {
            b = 0;
        }
        if(PhotonNetwork.IsMasterClient)
        {
            // Check is player left 
            if (ls[b].GetComponent<PhotonView>().Owner==null)
            {
                // push to master client
                // find index master client
                foreach (sorta_online sort in FindObjectsOfType<sorta_online>())
                {
                    PhotonView view = sort.GetComponent<PhotonView>();
                    if (view.IsMine)
                    {
                        print("randomcall");
                        int index = (b + 1 < ls.Count) ? b + 1 : 0;
                        GameObject.Find(sort.name).GetComponent<PhotonView>().RPC("startTimer", RpcTarget.All, ls[index].name);
                        GameObject.Find(sort.name).GetComponent<PhotonView>().RPC("randommoveAdmin", view.Owner, ls[b].name);
                        break;
                    }
                }
            }
            else
            {
                photonview.RPC("e", RpcTarget.AllBuffered, ls[b].name);
            }
        }
    }
    [PunRPC]
    private void e(string s )
    {
        GameObject.Find(s).GetComponent<sorta_online>().m(ls);
    }
    [PunRPC]
    IEnumerator xc()
    {
        yield return new WaitForSeconds(1.6f);
        if(photonview.IsMine)
        {
            photonview.RPC("RPCClean", RpcTarget.AllBuffered);
        }
        b = -1;
        dast++;
        bool is_use_hokm = false;
        int hokm = maincard.GetComponent<manager_online>().h;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetChild(0).GetComponent<card_online>().Rol == hokm)
            {
                is_use_hokm = true;
                break;
            }
        }
        int up_point = 0;
        if (is_use_hokm)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetChild(0).GetComponent<card_online>().Rol == hokm)
                {
                    if (transform.GetChild(i).GetChild(0).GetComponent<card_online>().P > up_point)
                    {
                        up_point = transform.GetChild(i).GetChild(0).GetComponent<card_online>().P;
                        barande = transform.GetChild(i).GetChild(0).GetComponent<card_online>().myparent;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetChild(0).GetComponent<card_online>().Rol == z)
                {
                    if (transform.GetChild(i).GetChild(0).GetComponent<card_online>().P > up_point)
                    {
                        up_point = transform.GetChild(i).GetChild(0).GetComponent<card_online>().P;
                        barande = transform.GetChild(i).GetChild(0).GetComponent<card_online>().myparent;
                    }
                }
            }
        }
        int mm = (ls.IndexOf(barande) + 2) % ls.Count;
        object[] paraments = {barande.name,mm };
        photonview.RPC("EndMatch", RpcTarget.AllBuffered, paraments);
    }
    [PunRPC]
    private void EndMatch(string barandeName,int mm)
    {
        GameObject barandee = GameObject.Find(barandeName);
        barandee.GetComponent<sorta_online>().SetDast(barandee.GetComponent<sorta_online>().Dast()+1);
        ls[mm].GetComponent<sorta_online>().SetDast(ls[mm].GetComponent<sorta_online>().Dast() + 1);
        // Destroy All cards 
        if (barandee.GetComponent<sorta_online>().Dast() == 7)
        {
            z = 6;
            l = false;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).GetComponent<card_online>().myparent.GetComponent<sorta_online>().SetDast(0);
                // clear All Dast 

            }
            if (barandee.transform.childCount == 6)
            {
                // Check am i hackem and what? 
                if (maincard.GetComponent<manager_online>().hakem != barandee.transform.parent.parent.parent)
                {
                    barandee.GetComponent<sorta_online>().SetDastCol(barandee.GetComponent<sorta_online>().DastCol()+3);
                    ls[mm].GetComponent<sorta_online>().SetDastCol(ls[mm].GetComponent<sorta_online>().DastCol() + 3);
                    add(barandee, ls[mm],3);
                }
                else
                {
                    barandee.GetComponent<sorta_online>().SetDastCol(barandee.GetComponent<sorta_online>().DastCol() + 2);
                    ls[mm].GetComponent<sorta_online>().SetDastCol(ls[mm].GetComponent<sorta_online>().DastCol() + 2);
                    add(barandee, ls[mm], 2);
                }
            }
            else
            {
                barandee.GetComponent<sorta_online>().SetDastCol(barandee.GetComponent<sorta_online>().DastCol()+1);
                ls[mm].GetComponent<sorta_online>().SetDastCol(ls[mm].GetComponent<sorta_online>().DastCol()+1);
                add(barandee, ls[mm], 1);
            }
            maincard.GetComponent<manager_online>().clear_all();
            dast = 0;
        }
        GameObject ga;
        if ((barandee.GetComponent<PhotonView>().IsMine && barandee.GetComponent<PhotonView>().Owner!=null) || (ls[mm].GetComponent<PhotonView>().IsMine && ls[mm].GetComponent<PhotonView>().Owner!= null))
        {
            ga = Instantiate(prefab,mi.transform);
        }
        else
        {
           ga =  Instantiate(prefab,pa.transform);
        }
        ga.transform.GetComponentInChildren<RTLTextMeshPro>().text = barandee.GetComponent<sorta_online>().Dast().ToString();


        if (barandee.GetComponent<sorta_online>().DastCol() >= (int)PhotonNetwork.CurrentRoom.CustomProperties["dast"])
        {
            if ((barandee.GetComponent<PhotonView>().IsMine && barandee.GetComponent<PhotonView>().Owner != null) || (ls[mm].GetComponent<PhotonView>().IsMine && ls[mm].GetComponent<PhotonView>().Owner != null))
            {
                if (!(bool)PhotonNetwork.CurrentRoom.CustomProperties["iscustome"])
                {
                    if (SecurePlayerPrefs.GetString("x2") == "yes")
                    {
                        SecurePlayerPrefs.SetString("x2", "no");
                        SecurePlayerPrefs.SetInt("coin", SecurePlayerPrefs.GetInt("coin") + int.Parse(((int)PhotonNetwork.CurrentRoom.CustomProperties["coin"] * 2f).ToString()));
                        end.transform.GetChild(1).GetChild(0).GetComponent<RTLTextMeshPro>().text = "بردین ";
                        end.transform.GetChild(2).GetChild(0).GetComponent<RTLTextMeshPro>().text = "+" + int.Parse(((int)PhotonNetwork.CurrentRoom.CustomProperties["coin"] * 2f).ToString());
                    }
                    else
                    {
                        SecurePlayerPrefs.SetInt("coin", SecurePlayerPrefs.GetInt("coin") + int.Parse(((int)PhotonNetwork.CurrentRoom.CustomProperties["coin"] * 2f).ToString()));
                        end.transform.GetChild(1).GetChild(0).GetComponent<RTLTextMeshPro>().text = "بردین ";
                        end.transform.GetChild(2).GetChild(0).GetComponent<RTLTextMeshPro>().text = "+" + int.Parse(((int)PhotonNetwork.CurrentRoom.CustomProperties["coin"] * 2f).ToString());
                    }
                }
                else
                {
                    SecurePlayerPrefs.SetInt("coin", SecurePlayerPrefs.GetInt("coin") + int.Parse(((int)PhotonNetwork.CurrentRoom.CustomProperties["coin"] * 2.5f).ToString()));
                    end.transform.GetChild(1).GetChild(0).GetComponent<RTLTextMeshPro>().text = "بردین ";
                    end.transform.GetChild(2).GetChild(0).GetComponent<RTLTextMeshPro>().text = "+" + int.Parse(((int)PhotonNetwork.CurrentRoom.CustomProperties["coin"] * 2.5f).ToString());
                }
            }
            else
            {
                end.transform.GetChild(1).GetChild(0).GetComponent<RTLTextMeshPro>().text = "باختین";
                end.transform.GetChild(2).GetChild(0).GetComponent<RTLTextMeshPro>().text = "-"+PhotonNetwork.CurrentRoom.CustomProperties["coin"].ToString();
            }
            PhotonNetwork.LeaveRoom();
            end.transform.GetChild(3).GetComponent<Button>().AddEventListener(0,leaveRoom);
            end.SetActive(true);
        }
        ls = RotateList(ls, ls.IndexOf(barandee));
        for (int i = 0; i < transform.childCount; i++)
        {
            try
            {
                DestroyImmediate(transform.GetChild(i).GetChild(0).gameObject);
            }
            catch
            {

            }
        }
        if(barandee.GetComponent<sorta_online>().Dast()==0)
        {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("base"))
            {
                for (int i = 0; i < g.transform.childCount; i++)
                {
                    Destroy(g.transform.GetChild(i).gameObject);
                }
            }
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (sorta_online sort in FindObjectsOfType<sorta_online>())
                {
                    PhotonView view = sort.GetComponent<PhotonView>();
                    if (view.Owner != null)
                    {
                        view.RPC("stopTimer", view.Owner);
                    }
                }
                if(barandee.GetComponent<sorta_online>().DastCol()== (int)PhotonNetwork.CurrentRoom.CustomProperties["dast"])
                {
                    return;
                }
                GameObject current_hakem = maincard.GetComponent<manager_online>().hakem;
                if (current_hakem == ls[mm].transform.parent.parent.parent.gameObject || current_hakem == barandee.transform.parent.parent.parent.gameObject)
                {
                    StartCoroutine(maincard.GetComponent<manager_online>().main_pakhsh(current_hakem));
                }
                else
                {
                    StartCoroutine(maincard.GetComponent<manager_online>().main_pakhsh(ls[(ls.IndexOf(current_hakem.GetComponentInChildren<sorta_online>().gameObject) + 1) % ls.Count].transform.parent.parent.parent.gameObject));
                }
                b = 0;
            }
        }
    }
    private void add(GameObject a ,GameObject b , int i)
    {
        foreach (GameObject g in ls)
        {
            if(g!=a&&g!=b)
            {
                g.GetComponent<sorta_online>().SetDastHarif(g.GetComponent<sorta_online>().DastHarif() + i);
            }
        }
    }
    [PunRPC]
    private void RPCZ(int zz)
    {
        z = zz;
    }
    [PunRPC]
    private void RPCC()
    {
        c++;
    }
    [PunRPC]
    private void RPCClean()
    {
        c = 0;
    }
    private void leaveRoom(int i)
    {
        SceneManager.LoadScene(1);
    }
}
