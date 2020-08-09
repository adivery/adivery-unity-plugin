using UnityEngine;

public class BannerAdCallback : AndroidJavaProxy
{
    public BannerAdCallback() : base("com.adivery.unitylib.interfaces.AdiveryUnityBannerCallback") { }

    public virtual void onAdLoaded()
    {
        
    }

    public virtual void onAdLoadFailed(int errorCode)
    {
        
    }

    public virtual void onAdShowFailed(int errorCode)
    {
        
    }

    public virtual void onAdClicked()
    {
        
    }

}
