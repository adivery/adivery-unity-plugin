using UnityEngine;

namespace adivery
{

    public class InterstitialCallback : AndroidJavaProxy
    {
        public InterstitialCallback() : base("com.adivery.unitylib.interfaces.AdiveryUnityInterstitialCallback") { }

        public void onAdLoaded(string adId)
        {
            // show ad imediately after it has loaded
            Adivery.showAd(adId);
            Debug.Log("on interstitial ad loaded");
        }

        public void onAdLoadFailed(int errorCode)
        {
            Debug.Log("on interstitial ad load failed, errorcode: " + errorCode);
        }

        public void onAdShown()
        {
            Debug.Log("on interstitial ad shown");
        }

        public void onAdShowFailed(int errorCode)
        {
            Debug.Log("on interstitial ad failed to show, errorcode: " + errorCode);
        }

        public void onAdClicked()
        {
            Debug.Log("on interstitial ad clicked");
        }

        public void onAdClosed()
        {
            Debug.Log("on interstitial ad closed");
        }

    }

}