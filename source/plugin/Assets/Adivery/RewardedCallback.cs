using UnityEngine;

namespace AdiveryUnity
{
    internal class RewardedCallback : AndroidJavaProxy
    {
        public RewardedCallback() : base("com.adivery.unity.RewardedCallback") { }

        public virtual void onAdLoaded() { }

        public virtual void onAdRewarded() { }

        public virtual void onAdLoadFailed(int errorCode) { }

        public virtual void onAdShown() { }

        public virtual void onAdShowFailed(int errorCode) { }

        public virtual void onAdClicked() { }

        public virtual void onAdClosed() { }
    }
}