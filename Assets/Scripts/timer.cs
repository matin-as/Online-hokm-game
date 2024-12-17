using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using RTLTMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class timer : MonoBehaviour
{
    private bool isMasterMoving = false;
    public Dictionary<int, string> pos = new Dictionary<int, string>()
{
    { 3, "q (2)" },
    { 2, "q (1)" },
    { 1, "q" },
    { 4, "q (3)" }
};
    Image img;
    private float totalTime = 15;//15
    private bool isRunning = false;
    public string a;
    private void OnEnable()
    {
        img = GetComponent<Image>();
        isRunning = false;
        img.fillAmount = 0;
        StopAllCoroutines();

        isRunning = true;
        StartCoroutine(TimerCoroutine());
    }

    private void OnDisable()
    {
        isRunning = false;
        img.fillAmount = 0;
        StopAllCoroutines();
    }

    private IEnumerator TimerCoroutine()
    {
        float timeRemaining = totalTime;
        while (timeRemaining > 0 && isRunning)
        {
            timeRemaining -= Time.deltaTime;

            float fillAmount = timeRemaining / totalTime;

            img.fillAmount = fillAmount;

            yield return null;
        }

        if (timeRemaining <= 0 && isRunning)
        {
            img.fillAmount = 0;
            if (PhotonNetwork.IsMasterClient)
            {
                bool masterMoved = false;
                GameObject target = null;
                int playerid = 0;

                foreach (sorta_online sort in FindObjectsOfType<sorta_online>())
                {
                    PhotonView view = sort.GetComponent<PhotonView>();
                    int.TryParse(transform.parent.parent.GetChild(2).GetComponent<RTLTextMeshPro>().text, out int playerId);

                    if (view.Owner != null && view.Owner.ActorNumber == playerId)
                    {
                        playerid = playerId;
                        target = sort.gameObject;
                        break;
                    }
                }

                try
                {
                    print("Target:" + target.name);
                    masterMoved = true; // فرض می‌کنیم MasterClient حرکتی انجام داده است
                }
                catch
                {
                    saound x = GameObject.Find("canvas").GetComponent<saound>();
                    int.TryParse(transform.parent.parent.GetChild(2).GetComponent<RTLTextMeshPro>().text, out int playerId);
                    if (x.leaves.Contains(playerId))
                    {
                        playerid = x.leaves[x.leaves.IndexOf(playerId)];
                        target = GameObject.Find(pos[playerid]).transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
                    }
                }

                // بررسی حرکت نکردن MasterClient و واگذاری حرکت به بازیکن با ActorNumber 2
                //if (!masterMoved && target == null)
                //{
                //    foreach (sorta_online sort in FindObjectsOfType<sorta_online>())
                //    {
                //        PhotonView view = sort.GetComponent<PhotonView>();

                //        if (view.Owner != null && view.Owner.ActorNumber == 2)
                //        {
                //            target = sort.gameObject;
                //            break;
                //        }
                //    }
                //}

                // ارسال فرمان حرکت برای بازیکنی که باید به جای MasterClient حرکت کند
                foreach (sorta_online sort in FindObjectsOfType<sorta_online>())
                {
                    PhotonView view = sort.GetComponent<PhotonView>();
                    if (view.Owner!=null &&view.IsMine)
                    {
                        yield return new WaitForSeconds(1);
                        view.RPC("randommoveAdmin", view.Owner, target.name);
                        break;
                    }
                }
            }
            GetComponent<timer>().enabled = false;
        }
    }
}
