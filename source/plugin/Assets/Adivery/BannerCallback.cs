using UnityEngine;

namespace AdiveryUnity
{
    internal class BannerCallback : AndroidJavaProxy
    {
        public BannerCallback() : base("com.adivery.unity.BannerCallback") { }

        public virtual void onAdLoaded() { }

        public virtual void onAdLoadFailed() { }

        public virtual void onAdShowFailed() { }

        public virtual void onAdClicked() { }
    }
}
