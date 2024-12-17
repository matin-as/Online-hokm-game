using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using RTLTMPro;
using TMPro;
using System;
using Photon.Pun;
using UnityEngine.U2D;
using Unity.Notifications.Android;

public class FortuneWheelManager : MonoBehaviour
{
    [SerializeField] private FortuneWheel fortuneWheel;
    [SerializeField] private RTLTextMeshPro resultLabel;
    [SerializeField] private TextMeshProUGUI coin;
    Button spain;
    private void Start()
    {
        spain = transform.GetChild(1).GetComponent<Button>();
        if (PlayerPrefs.GetString("time")=="")
        {
            PlayerPrefs.SetString("time", GetPhotonServerTime().ToString());
            spain = transform.GetChild(1).GetComponent<Button>();
            spain.AddEventListener(0, Spin);
        }
        else
        {
            string lastPlayTimeString = PlayerPrefs.GetString("time");
            DateTime lastPlayTime = DateTime.Parse(lastPlayTimeString);
            DateTime currentTime = GetPhotonServerTime();
            // مدت زمان گذشته را حساب می‌کنیم
            TimeSpan timePassed = currentTime - lastPlayTime;
            if (timePassed.TotalHours >= 24)
            {
                spain.AddEventListener(0, Spin);
            }
            else
            {
                double remainingHours = 24 - timePassed.TotalHours;
                if (remainingHours < 1)
                {
                    double remainingMinutes = (1 - timePassed.TotalMinutes % 60); // یا استفاده از 24 * 60 - TotalMinutes
                    spain.GetComponentInChildren<RTLTextMeshPro>().text = $"{Math.Round(remainingMinutes)} دقیقه دیگر";
                }
                else
                {
                    spain.GetComponentInChildren<RTLTextMeshPro>().text = $"{Math.Round(remainingHours)} ساعت دیگر";
                }
            }
        }
    }
    private void Spin(int i)
    {
        spain.interactable = false;
        StartCoroutine(SpinCoroutine());
    }
    IEnumerator SpinCoroutine()
    {
        yield return StartCoroutine(fortuneWheel.StartFortune());
        spain.onClick.RemoveAllListeners();
        spain.interactable = true;
        PlayerPrefs.SetString("time", GetPhotonServerTime().ToString());
        if (resultLabel == null) yield break;
        resultLabel.text = fortuneWheel.GetLatestResult();
        if(fortuneWheel.GetLatestResult().Contains("+"))
        {
            SecurePlayerPrefs.SetInt("coin", SecurePlayerPrefs.GetInt("coin")+int.Parse(fortuneWheel.GetLatestResult().Substring(1)));
        }
        spain.GetComponentInChildren<RTLTextMeshPro>().text = "24 ساعت دیگر ";
        coin.text = SecurePlayerPrefs.GetInt("coin").ToString();
        yield return new WaitForSeconds(2f);
        resultLabel.text = "";


        // Create Chanel
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        // Create Notification
        var notification = new AndroidNotification();
        notification.Title = "چرخونه شانس";
        notification.Text = "به بازی برگرد و چرخونه شانست رو امتحان کن ";
        notification.FireTime = DateTime.Now.AddDays(1);
        // Send Notificatin
        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }
    DateTime GetPhotonServerTime()
    {
        // زمان جاری سرور Photon بر حسب میلی‌ثانیه
        int serverTimestamp = PhotonNetwork.ServerTimestamp;
        // محاسبه زمان بر اساس میلی‌ثانیه (تاریخ 1970-01-01 شروع Unix)
        DateTime serverTime = new DateTime(1970, 1, 1).AddMilliseconds(serverTimestamp);

        return serverTime;
    }
}
