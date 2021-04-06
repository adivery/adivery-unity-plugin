using UnityEngine;

namespace AdiveryUnity
{
    internal class NativeCallback : AndroidJavaProxy
    {
        public NativeCallback() : base("com.adivery.unity.NativeCallback") { }

        public virtual void onAdShowFailed() { }

        public virtual void onAdLoadFailed() { }

        public virtual void onAdClicked() { }

        public virtual void onAdShown() { }

        public virtual void onAdLoaded() { }
    }
}
