using UnityEngine;
using UnityEngine.UI;
using AdiveryUnity;
using System;


public class AdsController : MonoBehaviour
{
    string appID = "7e27fb38-5aff-473a-998f-437b89426f66";
    string rewardedPlacement = "2efedcaa-fcc0-4610-a025-109ff17594af";
    string interstitialPlacement = "0045a4aa-1498-4790-9eed-6e33ac870e5f";
    string bannerPlacement = "2f71ec44-f30a-4043-9cc1-f32347a07f8b";
    string largeBannerPlacement = "2f71ec44-f30a-4043-9cc1-f32347a07f8b";
    string mediumRectanglePlacement = "2f71ec44-f30a-4043-9cc1-f32347a07f8b";
    string nativePlacement = "25928bf1-d4f7-432c-aaf7-1780602796c3";
    NativeAd native;
    BannerAd banner, largeBanner, mediumRectangle;
    AdiveryListener listener;


    private void OnDestroy()
    {
        Adivery.RemoveListener(listener);
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("start called");
        Adivery.SetLoggingEnabled(true);
        Adivery.Configure(appID);

        Adivery.PrepareInterstitialAd(interstitialPlacement);
        Adivery.PrepareRewardedAd(rewardedPlacement);

        listener = new AdiveryListener();

        listener.OnError += OnError;
        listener.OnRewardedAdClosed += OnReardedClosed;
        listener.OnInterstitialAdLoaded += OnInterstitialLoaded;

        Adivery.AddListener(listener);

        initRewarded();

        GameObject.Find("rewarded").GetComponent<Button>().onClick.AddListener(delegate () { ShowRewardedAd(); });
        GameObject.Find("interstitial").GetComponent<Button>().onClick.AddListener(delegate () { ShowInterstitial(); });
        GameObject.Find("banner").GetComponent<Button>().onClick.AddListener(delegate () { ShowBannerAd(); });
        GameObject.Find("largeBanner").GetComponent<Button>().onClick.AddListener(delegate () { ShowLargeBanner(); });
        GameObject.Find("mediumRectangle").GetComponent<Button>().onClick.AddListener(delegate () { ShowMediumRectangle(); });
        GameObject.Find("native").GetComponent<Button>().onClick.AddListener(delegate () { ShowNativeAd(); });
    }

    private void OnInterstitialLoaded(object caller, string placement)
    {
        Debug.Log("Interstitial loaded");
    }

    public void OnReardedClosed(object caller, AdiveryReward reward)
    {
        Debug.Log("Adivery reward: " + reward.IsRewarded);
    }

    public void OnError(object caller, AdiveryError error)
    {
        Debug.Log("placement: " + error.PlacementId + " error: " + error.Reason);
    }

    public void OnRewardedAdLoadFailed(object caller, string args)
    {
        Debug.Log("ad load failed: " + args);
    }

    public void initRewarded()
    {
        banner = new BannerAd(bannerPlacement, BannerAd.TYPE_BANNER, BannerAd.POSITION_BOTTOM);
        banner.OnAdLoaded += OnBannerAdLoaded;
        banner.LoadAd();

        largeBanner = new BannerAd(largeBannerPlacement, BannerAd.TYPE_LARGE_BANNER, BannerAd.POSITION_BOTTOM);
        banner.OnAdLoaded += OnLargeBannerLoaded;
        largeBanner.LoadAd();

        mediumRectangle = new BannerAd(mediumRectanglePlacement, BannerAd.TYPE_MEDIUM_RECTANGLE, BannerAd.POSITION_BOTTOM);
        banner.OnAdLoaded += OnMediumRectangleAdLoaded;
        mediumRectangle.LoadAd();

        native = new NativeAd(nativePlacement);
        native.LoadAd();

    }

    public void ShowNativeAd()
    {
        RawImage icon = GameObject.Find("icon").GetComponent<RawImage>();
        Text headline = GameObject.Find("headline").GetComponent<Text>();
        Button cta = GameObject.Find("nativeButton").GetComponent<Button>();
        RawImage image = GameObject.Find("image").GetComponent<RawImage>();
        Text advertiser = GameObject.Find("advertiser").GetComponent<Text>();
        Text ctaText = GameObject.Find("ctaText").GetComponent<Text>();

        icon.texture = native.GetIconTexture2D();
        image.texture = native.GetImageTexture2D();
        cta.onClick.AddListener(delegate () { native.RecordClick(); });
        headline.text = native.GetHeadline();
        advertiser.text = native.GetAdvertiser();
        ctaText.text = native.GetCallToAction();

        native.RecordImpression();
    }

    public void ShowMediumRectangle()
    {
        if (mediumRectangle.IsLoaded())
        {
            Debug.Log("show medium rectangle");
            banner.Hide();
            largeBanner.Hide();
            mediumRectangle.Show();
        }
    }

    public void ShowLargeBanner()
    {
        if (largeBanner.IsLoaded())
        {
            banner.Hide();
            mediumRectangle.Hide();
            largeBanner.Show();
        }
    }

    public void ShowBannerAd()
    {
        if (banner.IsLoaded())
        {
            largeBanner.Hide();
            mediumRectangle.Hide();
            banner.Show();
        }
    }

    public void ShowInterstitial()
    {
        if (Adivery.IsLoaded(interstitialPlacement))
        {
            Adivery.Show(interstitialPlacement);
        }
    }

    public void OnMediumRectangleAdLoaded(object caller, EventArgs args)
    {
        mediumRectangle.Hide();
    }

    public void OnLargeBannerLoaded(object caller, EventArgs args)
    {
        largeBanner.Hide();
    }

    public void OnBannerAdLoaded(object caller, EventArgs args)
    {
        banner.Hide();
    }

    public void ShowRewardedAd()
    {
        if (Adivery.IsLoaded(rewardedPlacement))
        {
            Adivery.Show(rewardedPlacement);
        }
    }
}
