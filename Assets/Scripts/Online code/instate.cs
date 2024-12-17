using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class instate : MonoBehaviour
{
    public int i = 0;
    [SerializeField] GameObject player;
    [SerializeField] List<GameObject> PositionOfPlayers;
    private PhotonView photonView;
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        Player[] players = PhotonNetwork.PlayerList;
        //GameObject g = PhotonNetwork.Instantiate(player.name, PositionOfPlayers[0].transform.position, PositionOfPlayers[0].transform.rotation);
        PositionOfPlayers[0].GetComponent<PhotonView>().TransferOwnership(players[i]);
        photonView.RPC("getpos",RpcTarget.AllBuffered);
    }
    [PunRPC]
    public void getpos()
    {
        PositionOfPlayers[0].SetActive(true);
        PositionOfPlayers.Remove(PositionOfPlayers[0]);
        i++;
    }
}
