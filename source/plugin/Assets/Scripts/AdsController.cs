using System;
using System.Threading;
using AdiveryUnity;
using UnityEngine;

public class AdsController : MonoBehaviour
{
    public Material GroundTextMaterial;
    public Font TextFont;

    private BannerAd banner;
    private InterstitialAd interstitial;
    private NativeAd native;
    private bool nativeAdLoaded;
    private Thread mainThread;

    public void Start()
    {
        mainThread = System.Threading.Thread.CurrentThread;

        string appId = "7e27fb38-5aff-473a-998f-437b89426f66";

        Adivery.Configure(appId);
        Adivery.SetLoggingEnabled(true);

        banner = new BannerAd("2f71ec44-f30a-4043-9cc1-f32347a07f8b", BannerAd.TYPE_BANNER, BannerAd.POSITION_TOP);
        banner.OnAdLoaded += OnBannerAdLoaded;
        banner.LoadAd();

        interstitial = new InterstitialAd("de5db046-765d-478f-bb2e-30dc2eaf3f51");
        interstitial.OnAdLoaded += OnInterstitialAdLoaded;
        interstitial.OnAdLoadFailed += OnInterstitialAdLoadFailed;
        interstitial.OnAdShown += OnInterstitialAdShown;
        interstitial.OnAdShowFailed += OnInterstitialAdShowFailed;
        interstitial.OnAdClicked += OnInterstitialAdClicked;
        interstitial.OnAdClosed += OnInterstitialAdClosed;
        interstitial.LoadAd();

        native = new NativeAd("103ea0d3-7b1d-458e-ac9d-a3165e7634d2");
        native.OnAdLoaded += OnNativeAdLoaded;
        native.LoadAd();
    }

    bool IsMainThread()
    {
        return mainThread.Equals(Thread.CurrentThread);
    }

    private void LogEvent(string evt)
    {
        print("Adivery Ad Event: " + evt + " Main Thread: " + IsMainThread());
    }

    // Interstitial Events

    public void OnInterstitialAdLoaded(object caller, EventArgs args)
    {
        LogEvent("OnInterstitialAdLoaded");
        interstitial.Show();
    }

    private void OnInterstitialAdLoadFailed(object caller, EventArgs args)
    {
        LogEvent("OnInterstitialAdLoadFailed");
    }

    private void OnInterstitialAdShowFailed(object caller, EventArgs args)
    {
        LogEvent("OnInterstitialAdShowFailed");
    }

    private void OnInterstitialAdShown(object sender, EventArgs e)
    {
        LogEvent("OnInterstitialAdShown");
    }

    private void OnInterstitialAdClicked(object sender, EventArgs e)
    {
        LogEvent("OnInterstitialAdClicked");
    }

    private void OnInterstitialAdClosed(object sender, EventArgs e)
    {
        LogEvent("OnInterstitialAdClosed");
    }

    // Banner Events

    public void OnBannerAdLoaded(object caller, EventArgs args)
    {
        LogEvent("OnBannerAdLoaded");
    }

    // Native Events

    public void OnNativeAdLoaded(object caller, EventArgs args)
    {
        LogEvent("OnNativeAdLoaded");
        nativeAdLoaded = true;
    }

    // Show Native Ad

    public void Update()
    {
        if (nativeAdLoaded)
        {
            Texture2D billboardTexture1 = native.GetIconTexture2D();
            Texture2D billboardTexture2 = native.GetImageTexture2D();

            for (int i = 1; i <= 6; i++)
            {
                if (i % 2 == 0)
                {
                    GameObject.Find("Billboard" + i.ToString())
                            .GetComponent<Renderer>()
                            .material
                            .mainTexture = billboardTexture1;
                }
                else
                {
                    GameObject.Find("Billboard" + i.ToString())
                            .GetComponent<Renderer>()
                            .material
                            .mainTexture = billboardTexture2;
                }
            }

            GameObject textObject = new GameObject("GroundText");
            GameObject ground = GameObject.Find("Ground");
            textObject.transform.parent = ground.transform;
            textObject.transform.position = new Vector3(0, 0.1f, 0);
            textObject.AddComponent<TextMesh>();

            TextMesh textMeshComponent = textObject.GetComponent<TextMesh>();
            MeshRenderer meshRendererComponent = textObject.GetComponent<MeshRenderer>();

            string adText = native.GetHeadline();

            textMeshComponent.text = adText;
            textMeshComponent.fontSize = 8;
            textMeshComponent.anchor = TextAnchor.MiddleCenter;
            textMeshComponent.transform.Rotate(new Vector3(90, 0, 0));
            textMeshComponent.font = TextFont;
            meshRendererComponent.material = GroundTextMaterial;

            nativeAdLoaded = false;
        }
    }

}
