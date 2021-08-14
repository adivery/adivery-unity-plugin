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
        public event EventHandler<string> OnError;
        public event EventHandler OnAdClicked;

        public BannerAd(string placementId, int bannerType, int bannerPosition)
        {
            if (!Adivery.IsAdiverySupported())
            {
                return;
            }

            adObject = new AndroidJavaObject("com.adivery.sdk.plugins.unity.Banner",
                Adivery.GetAndroidActivity(),
                placementId,
                bannerType,
                bannerPosition,
                new BannerCallbackImpl(this));
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
            adObject.Call("show");
        }

        public void Hide()
        {
            adObject?.Call("hide");
        }

        public void Destroy()
        {
            adObject?.Call("destroy");
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
                    ad.OnAdLoaded?.Invoke(this, null);
                });
            }

            public override void onError(string reason)
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    ad.OnError?.Invoke(this, reason);
                });
            }

            public override void onAdClicked()
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    ad.OnAdClicked?.Invoke(this, null);
                });
            }
        }
    }
}

