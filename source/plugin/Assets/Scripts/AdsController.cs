using UnityEngine;
using UnityEngine.UI;
using AdiveryUnity;
using System;


public class AdsController : MonoBehaviour
{
    [Header("Adivery Configuration")]
    [SerializeField] private string appID = "738c5e66-2e4f-4ff0-8118-ef6c8235a5e9";
    [SerializeField] private string rewardedPlacement = "29f509a9-3826-4ada-8404-4735aa14b510";
    [SerializeField] private string interstitialPlacement = "5788d25b-b7d6-442c-a562-b4bef582c18c";
    [SerializeField] private string offerWallPlacement = "9cad440a-8271-49d2-8f81-0819268a5ce4";
    [SerializeField] private string bannerPlacement = "06f0c520-3cc8-45c0-bbf1-f866f7eac525";
    [SerializeField] private string nativePlacement = "9bc87b9f-3aef-4988-b1d2-2f4e5b67b2c5";

    [Header("UI - Buttons")]
    [SerializeField] private Button rewardedButton;
    [SerializeField] private Button interstitialButton;
    [SerializeField] private Button offerWallButton;
    [SerializeField] private Button bannerButton;
    [SerializeField] private Button largeBannerButton;
    [SerializeField] private Button mediumRectangleButton;
    [SerializeField] private Button loadNativeAdButton;

    [Header("UI - Labels")]
    [SerializeField] private Text rewardText;
    [SerializeField] private Text skipText;
    [SerializeField] private Text impressionText;

    [Header("UI - Native Ad Template")]
    [SerializeField] private RawImage nativeIcon;
    [SerializeField] private RawImage nativeImage;
    [SerializeField] private Text nativeHeadline;
    [SerializeField] private Text nativeAdvertiser;
    [SerializeField] private Text nativeCtaText;
    [SerializeField] private Button nativeCtaButton;

    NativeAd native;
    BannerAd banner, largeBanner, mediumRectangle;
    AdiveryListener listener;
    AdiveryListener rewardedListener;
    int score = 0;
    int skips = 0;
    int impressions = 0;

    private void OnDestroy()
    {
        Adivery.RemoveListener(listener);
        Adivery.RemovePlacementListener(rewardedPlacement, rewardedListener);

        banner?.Destroy();
        largeBanner?.Destroy();
        mediumRectangle?.Destroy();
        native?.Destroy();
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("start called");
#if DEBUG
        Adivery.SetLoggingEnabled(true);
#endif
        Adivery.Configure(appID);

        listener = new AdiveryListener();

        listener.OnError += OnError;
        listener.OnInterstitialAdLoaded += OnInterstitialLoaded;
        listener.OnRewardedAdClicked += OnRewardedClicked;
        listener.OnOfferWallAdLoaded += OnOfferWallLoaded;
        listener.OnOfferWallAdShown += OnOfferWallShown;
        listener.OnOfferWallAdClicked += OnOfferWallClicked;
        listener.OnOfferWallAdClosed += OnOfferWallClosed;
        listener.OnOfferWallMissionRewarded += OnOfferWallMissionRewarded;

        rewardedListener = new AdiveryListener();
        rewardedListener.OnError += OnError;
        rewardedListener.OnRewardedAdClosed += OnRewardedClosed;

        Adivery.AddPlacementListener(rewardedPlacement, rewardedListener);

        Adivery.AddListener(listener);

        Adivery.PrepareInterstitialAd(interstitialPlacement);
        Adivery.PrepareRewardedAd(rewardedPlacement);

        initRewarded();

        rewardedButton.onClick.AddListener(ShowRewardedAd);
        interstitialButton.onClick.AddListener(ShowInterstitial);
        offerWallButton.onClick.AddListener(LoadOfferWallAd);
        bannerButton.onClick.AddListener(ShowBannerAd);
        largeBannerButton.onClick.AddListener(ShowLargeBanner);
        mediumRectangleButton.onClick.AddListener(ShowMediumRectangle);
        loadNativeAdButton.onClick.AddListener(LoadNativeAd);
    }

    public void OnRewardedClicked(object caller, string placement)
    {
        Debug.Log("on ad clicked " + placement);
    }

    private void OnInterstitialLoaded(object caller, string placement)
    {
        Debug.Log("Interstitial loaded");
    }

    public void OnRewardedClosed(object caller, AdiveryReward reward)
    {
        Debug.Log("Adivery reward: " + reward.IsRewarded);
        if (reward.IsRewarded)
        {
            score++;
            rewardText.text = "Reward: " + score;
        } else {
            skips ++;
            skipText.text = "Skip: " + skips;
        }
    }

    public void OnError(object caller, AdiveryError error)
    {
        Debug.Log("placement: " + error.PlacementId + " error: " + error.Reason);
    }

    private void OnOfferWallLoaded(object caller, string placement)
    {
        Debug.Log("OfferWall loaded");
        Adivery.Show(placement);
    }

    private void OnOfferWallShown(object caller, string placement)
    {
        Debug.Log("OfferWall shown");
    }

    private void OnOfferWallClicked(object caller, string placement)
    {
        Debug.Log("OfferWall clicked");
    }

    private void OnOfferWallClosed(object caller, string placement)
    {
        Debug.Log("OfferWall closed");
    }

    public void OnOfferWallMissionRewarded(object caller, AdiveryOfferWallReward reward)
    {
        Debug.Log("OfferWall mission rewarded: " + reward.RewardAmount + " " + reward.Currency);
        score += reward.RewardAmount;
        rewardText.text = "Reward: " + score;
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

        largeBanner = new BannerAd(bannerPlacement, BannerAd.TYPE_LARGE_BANNER, BannerAd.POSITION_BOTTOM);
        largeBanner.OnAdLoaded += OnLargeBannerLoaded;
        largeBanner.LoadAd();

        mediumRectangle = new BannerAd(bannerPlacement, BannerAd.TYPE_MEDIUM_RECTANGLE, BannerAd.POSITION_BOTTOM);
        mediumRectangle.OnAdLoaded += OnMediumRectangleAdLoaded;
        mediumRectangle.LoadAd();
    }

    public void LoadNativeAd()
    {
        native = new NativeAd(nativePlacement);
        native.OnAdLoaded += ShowNativeAd;
        native.LoadAd();
    }

    public void ShowNativeAd(object caller, EventArgs args)
    {
        nativeIcon.texture = native.GetIconTexture2D();
        nativeImage.texture = native.GetImageTexture2D();
        nativeCtaButton.onClick.AddListener(native.RecordClick);
        nativeHeadline.text = native.GetHeadline();
        nativeAdvertiser.text = native.GetAdvertiser();
        nativeCtaText.text = native.GetCallToAction();
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
            impressions++;
            impressionText.text = "impressions: " + impressions;
        }
    }

    public void LoadOfferWallAd()
    {
        // Shown from OnOfferWallLoaded once it's ready.
        Adivery.PrepareOfferWallAd(offerWallPlacement);
    }
}
