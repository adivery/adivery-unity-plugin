using UnityEngine;

namespace adivery
{

    public class InterstitialCallback : AndroidJavaProxy
    {
        public InterstitialCallback() : base("com.adivery.unitylib.interfaces.AdiveryUnityInterstitialCallback") { }

        public virtual void onAdLoaded(string adId)
        {
            
        }

        public virtual void onAdLoadFailed(int errorCode)
        {
            
        }

        public virtual void onAdShown()
        {
            
        }

        public virtual void onAdShowFailed(int errorCode)
        {
            
        }

        public virtual void onAdClicked()
        {
            
        }

        public virtual void onAdClosed()
        {
            
        }

    }

}