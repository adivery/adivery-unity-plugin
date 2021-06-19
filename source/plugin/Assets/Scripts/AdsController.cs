using UnityEngine;
using UnityEngine.UI;
using AdiveryUnity;
using System;


public class AdsController : MonoBehaviour
{
    string appID = "59c36ce3-7125-40a7-bd34-144e6906c796";
    string rewardedPlacement = "16414bae-368e-4904-b259-c5b89362206d";
    string interstitialPlacement = "38b301f2-5e0c-4776-b671-c6b04a612311";
    string bannerPlacement = "a355be22-970a-46b8-bc52-f0a59c4ded05";
    string largeBannerPlacement = "90717d2a-dad1-4ac1-8e71-c4d741a225bb";
    string mediumRectanglePlacement = "850d771f-dd47-4b2e-832c-ad1ad70d9266";
    string nativePlacement = "ff454979-efaa-4ab8-b084-7db19e995d9b";
    NativeAd native;
    BannerAd banner, largeBanner, mediumRectangle;


    // Use this for initialization
    void Start()
    {
        Debug.Log("start called");
        Adivery.SetLoggingEnabled(true);
        Adivery.Configure(appID);

        Adivery.PrepareInterstitialAd(interstitialPlacement);
        Adivery.PrepareRewardedAd(rewardedPlacement);

        Adivery.OnError += OnError;
        Adivery.OnRewardedAdClosed += OnReardedClosed;

        initRewarded();

        GameObject.Find("rewarded").GetComponent<Button>().onClick.AddListener(delegate () { ShowRewardedAd(); });
        GameObject.Find("interstitial").GetComponent<Button>().onClick.AddListener(delegate () { ShowInterstitial(); });
        GameObject.Find("banner").GetComponent<Button>().onClick.AddListener(delegate () { ShowBannerAd(); });
        GameObject.Find("largeBanner").GetComponent<Button>().onClick.AddListener(delegate () { ShowLargeBanner(); });
        GameObject.Find("mediumRectangle").GetComponent<Button>().onClick.AddListener(delegate () { ShowMediumRectangle(); });
        GameObject.Find("native").GetComponent<Button>().onClick.AddListener(delegate () { ShowNativeAd(); });
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
