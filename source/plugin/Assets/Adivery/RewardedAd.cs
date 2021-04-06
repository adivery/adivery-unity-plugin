using System;
using UnityEngine;

namespace AdiveryUnity
{
    public class RewardedAd
    {
        private readonly AndroidJavaObject adObject;

        public event EventHandler OnAdLoaded;
        public event EventHandler OnAdRewarded;
        public event EventHandler OnAdLoadFailed;
        public event EventHandler OnAdShowFailed;
        public event EventHandler OnAdShown;
        public event EventHandler OnAdClicked;
        public event EventHandler OnAdClosed;

        public RewardedAd(string placementId)
        {
            if (!Adivery.IsAdiverySupported())
            {
                return;
            }

            adObject = new AndroidJavaObject("com.adivery.unity.Rewarded",
                Adivery.GetAndroidActivity(),
                placementId,
                new RewardedCallbackImpl(this));
        }

        public void LoadAd()
        {
            if (adObject != null) {
                adObject.Call("loadAd");
            }
        }

        public bool IsLoaded()
        {
            return adObject != null ? adObject.Call<bool>("isLoaded") : false;
        }

        public void Show()
        {
            if (adObject != null) {
                adObject.Call("show");
            }
        }

        internal class RewardedCallbackImpl : RewardedCallback
        {
            readonly RewardedAd ad;

            public RewardedCallbackImpl(RewardedAd ad)
            {
                this.ad = ad;
            }

            public override void onAdLoaded()
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    if (ad.OnAdLoaded != null) {
                        ad.OnAdLoaded.Invoke(this, null);
                    }
                });
            }

            public override void onAdRewarded()
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    if (ad.OnAdRewarded != null) {
                        ad.OnAdRewarded.Invoke(this, null);
                    }
                });
            }

            public override void onAdLoadFailed()
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    if (ad.OnAdLoadFailed != null) {
                        ad.OnAdLoadFailed.Invoke(this, null);
                    }
                });
            }

            public override void onAdShown()
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    if (ad.OnAdShown != null) {
                        ad.OnAdShown.Invoke(this, null);
                    }
                });
            }

            public override void onAdShowFailed()
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    if (ad.OnAdShowFailed != null) {
                        ad.OnAdShowFailed.Invoke(this, null);
                    }
                });
            }

            public override void onAdClicked()
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    if (ad.OnAdClicked != null) {
                        ad.OnAdClicked.Invoke(this, null);
                    }
                    
                });
            }

            public override void onAdClosed()
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    if (ad.OnAdClosed != null) {
                        ad.OnAdClosed.Invoke(this, null);                        
                    }
                });
            }
        }
    }
}
