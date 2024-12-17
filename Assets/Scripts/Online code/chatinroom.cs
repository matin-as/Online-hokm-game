using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class chatinroom : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void open(GameObject g)
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            GameObject ga = transform.GetChild(0).GetChild(i).gameObject;
            if (ga==g)
            {
               ga.SetActive(true);
            }
            else
            {
                ga.SetActive(false);
            }
        }
    }
    public void send_chat(string s)
    {
        //object[] parametrs = new object[] { s, PhotonNetwork.LocalPlayer.NickName };
        //transform.parent.parent.GetComponent<PhotonView>().RPC("send_message", RpcTarget.AllBuffered, parametrs);
        transform.parent.parent.GetComponent<player_online>().call_send_chat(s);
    }
}
