using System;
using UnityEngine;

namespace AdiveryUnity
{
    public class NativeAd
    {
        private readonly AndroidJavaObject adObject;

        public event EventHandler OnAdShowFailed;
        public event EventHandler OnAdLoadFailed;
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
            if (adObject != null) {
                adObject.Call("loadAd");
            }
        }

        public bool IsLoaded()
        {
            return adObject != null ? adObject.Call<bool>("isLoaded") : false;
        }

        public void Destroy()
        {
            if (adObject != null) {
                adObject.Call("destroy");
            }
        }

        public string GetHeadline()
        {
            return adObject != null ? adObject.Call<string>("getHeadline") : null;
        }

        public string GetDescription()
        {
            return adObject != null ? adObject.Call<string>("getDescription") : null;
        }

        public string GetAdvertiser()
        {
            return adObject != null ? adObject.Call<string>("getAdvertiser") : null;
        }

        public string GetCallToAction()
        {
            return adObject != null ? adObject.Call<string>("getCallToAction") : null;
        }

        public Texture2D GetImageTexture2D()
        {
            return GetTexture2DFromByteArray(adObject != null ? adObject.Call<byte[]>("getImage") : null);
        }

        public Texture2D GetIconTexture2D()
        {
            return GetTexture2DFromByteArray(adObject != null ? adObject.Call<byte[]>("getIcon") : null);
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
            if (adObject != null) {
                adObject.Call("recordImpression");
            }
        }

        public void RecordClick()
        {
            if (adObject != null) {
                adObject.Call("recordClick");
            }
        }

        internal class NativeCallbackImpl : NativeCallback
        {
            readonly NativeAd ad;

            public NativeCallbackImpl(NativeAd ad)
            {
                this.ad = ad;
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

            public override void onAdLoadFailed()
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    if (ad.OnAdLoadFailed != null) {
                        ad.OnAdLoadFailed.Invoke(this, null);
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

            public override void onAdShown()
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    if (ad.OnAdShown != null) {
                        ad.OnAdShown.Invoke(this, null);
                    }
                });
            }

            public override void onAdLoaded()
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    ad.RecordImpression();
                    if (ad.OnAdLoaded != null) {
                        ad.OnAdLoaded.Invoke(this, null);
                    }
                });
            }
        }
    }
}
