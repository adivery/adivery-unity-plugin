using UnityEngine;

namespace AdiveryUnity
{
    internal class RewardedCallback : AndroidJavaProxy
    {
        public RewardedCallback() : base("com.adivery.unity.RewardedCallback") { }

        public virtual void onAdLoaded() { }

        public virtual void onAdRewarded() { }

        public virtual void onAdLoadFailed() { }

        public virtual void onAdShown() { }

        public virtual void onAdShowFailed() { }

        public virtual void onAdClicked() { }

        public virtual void onAdClosed() { }
    }
}