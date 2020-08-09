using UnityEngine;

namespace adivery
{

    public class RewardedAdCallback : AndroidJavaProxy
    {
        public RewardedAdCallback() : base("com.adivery.unitylib.interfaces.AdiveryUnityRewardedCallback") { }

        public virtual void onAdLoaded(string adId)
        {
            
        }

        public virtual void onAdRewarded()
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