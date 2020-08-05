using ad;
using UnityEngine;

namespace adivery
{

    public class RewardedAdCallback : AndroidJavaProxy
    {
        public RewardedAdCallback() : base("com.adivery.unitylib.interfaces.AdiveryUnityRewardedCallback") { }

        public void onAdLoaded(string adId)
        {
            // show ad imediately after it has loaded
            Adivery.showAd(adId);
        }

        public void onAdRewarded()
        {
            // give reward to the player.
            AdInfo.isAdRequestSend = false;
            Debug.Log("on ad rewarded");
        }

        public void onAdLoadFailed(int errorCode)
        {
            Debug.Log("on rewarded ad load failed, errorcode: " + errorCode);
        }

        public void onAdShown()
        {
            Debug.Log("on rewarded ad shown");
        }

        public void onAdShowFailed(int errorCode)
        {
            Debug.Log("on rewarded ad failed to show, errorcode: " + errorCode);
        }

        public void onAdClicked()
        {
            AdInfo.isAdRequestSend = false;
            Debug.Log("on rewarded ad clicked");
        }

        public void onAdClosed()
        {
            AdInfo.isAdRequestSend = false;
            Debug.Log("on rewarded ad closed");
        }
    }

}