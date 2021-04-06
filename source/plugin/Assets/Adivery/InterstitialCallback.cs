using UnityEngine;

namespace AdiveryUnity
{
    internal class InterstitialCallback : AndroidJavaProxy
    {
        public InterstitialCallback() : base("com.adivery.unity.InterstitialCallback") { }

        public virtual void onAdLoaded() { }

        public virtual void onAdLoadFailed() { }

        public virtual void onAdShown() { }

        public virtual void onAdShowFailed() { }

        public virtual void onAdClicked() { }

        public virtual void onAdClosed() { }
    }
}