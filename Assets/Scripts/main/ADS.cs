using UnityEngine;
using AdiveryUnity;
using TMPro;

public class ADS : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cointext;
    private static readonly string APP_ID = "APP_ID";
    private static readonly string rewardedPlacement = "rewardedPlacement";
    private static readonly string rewardedPlacement_moremoney = "rewardedPlacement_moremoney";
    AdiveryListener listener;
    void Start()
    {
        Adivery.Configure(APP_ID);

        Adivery.PrepareRewardedAd(rewardedPlacement);
        Adivery.PrepareRewardedAd(rewardedPlacement_moremoney);
        listener = new AdiveryListener();

        listener.OnError += OnError;
        listener.OnRewardedAdLoaded += OnRewardedLoaded;
        listener.OnRewardedAdClosed += OnRewardedClosed;

        Adivery.AddListener(listener);
    }
    private void OnRewardedLoaded(object caller, string placementId)
    {
        // Rewarded ad loaded
    }

    private void OnRewardedClosed(object caller, AdiveryReward reward)
    {
        k(reward);
    }

    private void OnError(object caller, AdiveryError args)
    {

    }
    private void k(AdiveryReward reward)
    {
        // Check if User should receive the reward
        if(reward.PlacementId== rewardedPlacement)
        {
            if (reward.IsRewarded)
            {
                SecurePlayerPrefs.SetString("x2", "yes");
                Toast._ShowAndroidToastMessage("جوایز بیشتر بر روی بازی اعمال شد ");
                //getRewardAmount(reward.PlacementId); // Implrement getRewardAmount yourself
            }
            else
            {
                Toast._ShowAndroidToastMessage("تا پایان تبلیعات صبر کنید ");
            }
        }
        else
        {
            if (reward.IsRewarded)
            {
                SecurePlayerPrefs.SetInt("coin", SecurePlayerPrefs.GetInt("coin")+25);
                cointext.text = SecurePlayerPrefs.GetInt("coin").ToString();
                Toast._ShowAndroidToastMessage("پول اضافه شد ");
                //getRewardAmount(reward.PlacementId); // Implrement getRewardAmount yourself
            }
            else
            {
                Toast._ShowAndroidToastMessage("تا پایان تبلیعات صبر کنید ");
            }
        }
    }
    public void showAd()
    {
        if (SecurePlayerPrefs.GetString("x2") == "yes")
        {
            Toast._ShowAndroidToastMessage("تغیرات اعمال شده است ");
            return;
        }
        if(Adivery.IsLoaded(rewardedPlacement))
        {
            Adivery.Show(rewardedPlacement);
            return;
        }
        Toast._ShowAndroidToastMessage("تا بارگزاری تبلیغات صبر کنید");
    }
    public void showAd_more()
    {
        if (Adivery.IsLoaded(rewardedPlacement_moremoney))
        {
            Adivery.Show(rewardedPlacement_moremoney);
            return;
        }
        Toast._ShowAndroidToastMessage("تا بارگزاری تبلیغات صبر کنید");
    }
}
