using UnityEngine;

namespace AdiveryUnity
{
    internal class BannerCallback : AndroidJavaProxy
    {
        public BannerCallback() : base("com.adivery.unity.BannerCallback") { }

        public virtual void onAdLoaded() { }

        public virtual void onAdLoadFailed(int errorCode) { }

        public virtual void onAdShowFailed(int errorCode) { }

        public virtual void onAdClicked() { }
    }
}
