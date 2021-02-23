using System;
using UnityEngine;

namespace AdiveryUnity
{

    public class InterstitialAd
    {
        private readonly AndroidJavaObject adObject;

        public event EventHandler OnAdLoaded;
        public event EventHandler<int> OnAdLoadFailed;
        public event EventHandler OnAdShown;
        public event EventHandler<int> OnAdShowFailed;
        public event EventHandler OnAdClicked;
        public event EventHandler OnAdClosed;

        public InterstitialAd(string placementId)
        {
            if (!Adivery.IsAdiverySupported())
            {
                return;
            }

            adObject = new AndroidJavaObject("com.adivery.unity.Interstitial",
                Adivery.GetAndroidActivity(),
                placementId,
                new InterstitialCallbackImpl(this));
        }

        public void LoadAd()
        {
            adObject?.Call("loadAd");
        }

        public bool IsLoaded()
        {
            return adObject?.Call<bool>("isLoaded") ?? false;
        }

        public void Show()
        {
            adObject?.Call("show");
        }

        public void Destroy()
        {
            adObject?.Call("destroy");
        }

        internal class InterstitialCallbackImpl : InterstitialCallback
        {
            readonly InterstitialAd ad;

            public InterstitialCallbackImpl(InterstitialAd ad)
            {
                this.ad = ad;
            }
            public override void onAdLoaded()
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    ad.OnAdLoaded?.Invoke(this, null);
                });
            }

            public override void onAdLoadFailed(int errorCode)
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    ad.OnAdLoadFailed?.Invoke(this, errorCode);
                });
            }

            public override void onAdShown()
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    ad.OnAdShown?.Invoke(this, null);
                });
            }

            public override void onAdShowFailed(int errorCode)
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    ad.OnAdShowFailed?.Invoke(this, errorCode);
                });
            }

            public override void onAdClicked()
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    ad.OnAdClicked?.Invoke(this, null);
                });
            }

            public override void onAdClosed()
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    ad.OnAdClosed?.Invoke(this, null);
                });
            }
        }
    }
}
