using System;
using UnityEngine;

namespace AdiveryUnity
{
    public class NativeAd
    {
        private readonly AndroidJavaObject adObject;

        public event EventHandler<int> OnAdShowFailed;
        public event EventHandler<int> OnAdLoadFailed;
        public event EventHandler OnAdClicked;
        public event EventHandler OnAdShown;
        public event EventHandler OnAdLoaded;

        public NativeAd(string placementId)
        {
            if (!Adivery.IsAdiverySupported())
            {
                return;
            }

            adObject = new AndroidJavaObject("com.adivery.unity.Native",
                Adivery.GetAndroidActivity(),
                placementId,
                new NativeCallbackImpl(this));
        }

        public void LoadAd()
        {
            adObject?.Call("loadAd");
        }

        public bool IsLoaded()
        {
            return adObject?.Call<bool>("isLoaded") ?? false;
        }

        public void Destroy()
        {
            adObject?.Call("destroy");
        }

        public string GetHeadline()
        {
            return adObject?.Call<string>("getHeadline");
        }

        public string GetDescription()
        {
            return adObject?.Call<string>("getDescription");
        }

        public string GetAdvertiser()
        {
            return adObject?.Call<string>("getAdvertiser");
        }

        public string GetCallToAction()
        {
            return adObject?.Call<string>("getCallToAction");
        }

        public Texture2D GetImageTexture2D()
        {
            return GetTexture2DFromByteArray(adObject?.Call<byte[]>("getImage"));
        }

        public Texture2D GetIconTexture2D()
        {
            return GetTexture2DFromByteArray(adObject?.Call<byte[]>("getIcon"));
        }

        private static Texture2D GetTexture2DFromByteArray(byte[] img)
        {
            if (img == null)
            {
                return null;
            }

            Texture2D nativeAdTexture = new Texture2D(1, 1);
            return nativeAdTexture.LoadImage(img) ? nativeAdTexture : null;
        }

        internal void RecordImpression()
        {
            adObject?.Call("recordImpression");
        }

        public void RecordClick()
        {
            adObject?.Call("recordClick");
        }

        internal class NativeCallbackImpl : NativeCallback
        {
            readonly NativeAd ad;

            public NativeCallbackImpl(NativeAd ad)
            {
                this.ad = ad;
            }

            public override void onAdShowFailed(int errorCode)
            {
                ad.OnAdShowFailed?.Invoke(this, errorCode);
            }

            public override void onAdLoadFailed(int errorCode)
            {
                ad.OnAdLoadFailed?.Invoke(this, errorCode);
            }

            public override void onAdClicked()
            {
                ad.OnAdClicked?.Invoke(this, null);
            }

            public override void onAdShown()
            {
                ad.OnAdShown?.Invoke(this, null);
            }

            public override void onAdLoaded()
            {
                ad.RecordImpression();
                ad.OnAdLoaded?.Invoke(this, null);
            }
        }
    }
}
