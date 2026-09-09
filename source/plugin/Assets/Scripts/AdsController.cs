using UnityEngine;
using UnityEngine.UI;
using AdiveryUnity;
using System;


public class AdsController : MonoBehaviour
{
    [Header("Adivery Configuration")]
    [SerializeField] private string appID = "7e27fb38-5aff-473a-998f-437b89426f66";
    [SerializeField] private string rewardedPlacement = "2efedcaa-fcc0-4610-a025-109ff17594af";
    [SerializeField] private string interstitialPlacement = "de5db046-765d-478f-bb2e-30dc2eaf3f51";
    [SerializeField] private string bannerPlacement = "5f2c4c86-a6ec-4735-9a44-f881fe40789f";
    [SerializeField] private string nativePlacement = "25928bf1-d4f7-432c-aaf7-1780602796c3";

    [Header("UI - Buttons")]
    [SerializeField] private Button rewardedButton;
    [SerializeField] private Button interstitialButton;
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

        Adivery.PrepareInterstitialAd(interstitialPlacement);
        Adivery.PrepareRewardedAd(rewardedPlacement);

        listener = new AdiveryListener();

        listener.OnError += OnError;
        listener.OnInterstitialAdLoaded += OnInterstitialLoaded;
        listener.OnRewardedAdClicked += OnRewardedClicked;

        rewardedListener = new AdiveryListener();
        rewardedListener.OnError += OnError;
        rewardedListener.OnRewardedAdClosed += OnRewardedClosed;

        Adivery.AddPlacementListener(rewardedPlacement, rewardedListener);

        Adivery.AddListener(listener);

        initRewarded();

        rewardedButton.onClick.AddListener(ShowRewardedAd);
        interstitialButton.onClick.AddListener(ShowInterstitial);
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
}
