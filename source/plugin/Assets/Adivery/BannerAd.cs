using System;
using UnityEngine;

namespace AdiveryUnity
{
    public class BannerAd
    {
        public const int POSITION_TOP = 0;
        public const int POSITION_BOTTOM = 1;

        public const int TYPE_BANNER = 0;
        public const int TYPE_LARGE_BANNER = 1;
        public const int TYPE_MEDIUM_RECTANGLE = 2;

        private readonly AndroidJavaObject adObject;

        public event EventHandler OnAdLoaded;
        public event EventHandler OnAdLoadFailed;
        public event EventHandler OnAdShowFailed;
        public event EventHandler OnAdClicked;

        public BannerAd(string placementId, int bannerType, int bannerPosition)
        {
            if (!Adivery.IsAdiverySupported())
            {
                return;
            }

            adObject = new AndroidJavaObject("com.adivery.unity.Banner",
                Adivery.GetAndroidActivity(),
                placementId,
                bannerType,
                bannerPosition,
                new BannerCallbackImpl(this));
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

        public void Hide()
        {
            if (adObject != null) {
                adObject.Call("hide");
            }
        }

        public void Destroy()
        {
            if (adObject != null) {
                adObject.Call("destroy");
            }
        }

        internal class BannerCallbackImpl : BannerCallback
        {
            readonly BannerAd ad;

            public BannerCallbackImpl(BannerAd ad)
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

            public override void onAdLoadFailed()
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    if (ad.OnAdLoadFailed != null) {
                        ad.OnAdLoadFailed.Invoke(this, null);
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
        }
    }
}

