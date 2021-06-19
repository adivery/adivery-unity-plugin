using UnityEngine;

namespace AdiveryUnity
{
    internal class BannerCallback : AndroidJavaProxy
    {
        public BannerCallback() : base("com.adivery.unity.BannerCallback") { }

        public virtual void onAdLoaded() { }

        public virtual void onError(string reason) { }

        public virtual void onAdClicked() { }
    }
}
