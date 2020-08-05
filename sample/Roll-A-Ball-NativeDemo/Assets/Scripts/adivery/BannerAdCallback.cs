using UnityEngine;

public class BannerAdCallback : AndroidJavaProxy
{
    public BannerAdCallback() : base("com.adivery.unitylib.interfaces.AdiveryUnityBannerCallback") { }

    public void onAdLoaded()
    {
        Debug.Log("on banner ad loaded");
    }

    public void onAdLoadFailed(int errorCode)
    {
        Debug.Log("on banner ad failed to load errorcode:" + errorCode);
    }

    public void onAdShowFailed(int errorCode)
    {
        Debug.Log("on banner ad failed to show errorcode: " + errorCode);
    }

    public void onAdClicked()
    {
        Debug.Log("on banner ad clicked");
    }

}
