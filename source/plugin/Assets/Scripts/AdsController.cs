using System;

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

    public void Start()
    {
        string appId = "7e27fb38-5aff-473a-998f-437b89426f66";

        Adivery.Configure(appId);
        Adivery.SetLoggingEnabled(true);

        banner = new BannerAd("2f71ec44-f30a-4043-9cc1-f32347a07f8b", BannerAd.TYPE_BANNER, BannerAd.POSITION_TOP);
        banner.OnAdLoaded += OnBannerAdLoaded;
        banner.LoadAd();

        interstitial = new InterstitialAd("de5db046-765d-478f-bb2e-30dc2eaf3f51");
        interstitial.OnAdLoaded += OnInterstitialAdLoaded;
        interstitial.LoadAd();

        native = new NativeAd("103ea0d3-7b1d-458e-ac9d-a3165e7634d2");
        native.OnAdLoaded += OnNativeAdLoaded;
        native.LoadAd();
    }

    public void OnBannerAdLoaded(object caller, EventArgs args)
    {
        print("Banner ad loaded");
    }

    public void OnInterstitialAdLoaded(object caller, EventArgs args)
    {
        print("Interstitial ad loaded");
        interstitial.Show();
    }

    public void OnNativeAdLoaded(object caller, EventArgs args)
    {
        print("Native ad loaded: " + native.GetHeadline());
        nativeAdLoaded = true;
    }

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

    ///// <summary>
    ///// Creates a new GameObject with a TextMesh component to display an error message.
    ///// </summary>
    ///// <returns>The GameObject with a TextMesh component.</returns>
    ///// <param name="errorText">Error text to display.</param>
    ///// <param name="offset">Local position offset from parent GameObject</param>
    ///// <param name="parentObject">Parent GameObject for error text.</param>
    //private GameObject CreateErrorTextOnGameObject(
    //        string errorText,
    //        GameObject parentObject,
    //        Vector3 offset)
    //{
    //    GameObject errorTextObject = new GameObject("ErrorText1");

    //    errorTextObject.transform.parent = parentObject.transform;
    //    errorTextObject.transform.localPosition = offset;
    //    errorTextObject.AddComponent<TextMesh>();

    //    TextMesh textMeshComponent = errorTextObject.GetComponent<TextMesh>();
    //    MeshRenderer meshRendererComponent = errorTextObject.GetComponent<MeshRenderer>();

    //    textMeshComponent.text = errorText;
    //    textMeshComponent.fontSize = 8;
    //    textMeshComponent.anchor = TextAnchor.MiddleCenter;
    //    textMeshComponent.font = this.TextFont;
    //    meshRendererComponent.material = this.ErrorTextMaterial;

    //    return errorTextObject;
    //}

    ///// <summary>
    ///// Requests a CustomNativeTemplateAd.
    ///// </summary>
    //private void RequestNativeAd()
    //{
    //    AdLoader adLoader = new AdLoader.Builder(AdUnitId)
    //        .ForCustomNativeAd(TemplateId)
    //        .Build();
    //    adLoader.OnCustomNativeTemplateAdLoaded += this.HandleCustomNativeAdLoaded;
    //    adLoader.OnAdFailedToLoad += this.HandleNativeAdFailedToLoad;
    //    adLoader.LoadAd(new AdRequest.Builder().Build());
    //}

    ///// <summary>
    ///// Handles the ad event corresponding to a CustomNativeTemplateAd succesfully loading.
    ///// </summary>
    ///// <param name="sender">Sender.</param>
    ///// <param name="args">EventArgs wrapper for CustomNativeTemplateAd that loaded.</param>
    //private void HandleCustomNativeAdLoaded(object sender, CustomNativeEventArgs args)
    //{
    //    this.nativeAd = args.nativeAd;
    //    this.nativeAdLoaded = true;
    //}

    ///// <summary>
    ///// Handles the native ad failing to load.
    ///// </summary>
    ///// <param name="sender">Sender.</param>
    ///// <param name="args">Error information.</param>
    //private void HandleNativeAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    //{
    //    Vector3 errorTextOffset = new Vector3(0, 0, -0.25f);

    //    if (this.errorMessage1 == null)
    //    {
    //        this.errorMessage1 = this.CreateErrorTextOnGameObject(
    //                "Ad failed to load",
    //                GameObject.Find("Billboard1"),
    //                errorTextOffset);
    //    }

    //    if (this.errorMessage2 == null)
    //    {
    //        this.errorMessage2 = this.CreateErrorTextOnGameObject(
    //                "Ad failed to load",
    //                GameObject.Find("Billboard2"),
    //                errorTextOffset);
    //    }

    //    MonoBehaviour.print("Ad Loader fail event received with message: " + args.Message);
    //}
}
