using UnityEngine;
using System;

namespace AdiveryUnity
{
    public class AdiveryError : EventArgs
    {
        public string PlacementId;
        public string Reason;
    }

    public class AdiveryReward : EventArgs
    {
        public string PlacementId;
        public bool IsRewarded;
    }

    public class Adivery
    {
        public static event EventHandler<AdiveryError> OnError;
        public static event EventHandler<string> OnInterstitialAdLoaded;
        public static event EventHandler<string> OnInterstitialAdClicked;
        public static event EventHandler<string> OnInterstitialAdShown;
        public static event EventHandler<string> OnInterstitialAdClosed;
        public static event EventHandler<string> OnRewardedAdLoaded;
        public static event EventHandler<string> OnRewardedAdShown;
        public static event EventHandler<string> OnRewardedAdClicked;
        public static event EventHandler<AdiveryReward> OnRewardedAdClosed;

        internal static AdiveryListener listener;
        internal static AndroidJavaObject adiveryListenreObject = new AndroidJavaObject("com.adivery.unity.FullScreenAd");


        internal static bool IsAdiverySupported()
        {
            return Application.platform == RuntimePlatform.Android;
        }

        internal static AndroidJavaObject GetAndroidActivity()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        internal static AndroidJavaObject GetAndroidApplication()
        {
            AndroidJavaObject activity = GetAndroidActivity();
            return activity.Call<AndroidJavaObject>("getApplication");
        }

        internal static AndroidJavaObject GetAdiveryClass()
        {
            return new AndroidJavaClass("com.adivery.sdk.Adivery");
        }

        public static void Configure(string appId)
        {
            if (!IsAdiverySupported())
            {
                return;
            }

            AdiveryEventExecutor.Initialize();
            GetAdiveryClass().CallStatic("configure", GetAndroidApplication(), appId);
            AddListener();
        }

        public static void SetLoggingEnabled(bool enabled)
        {
            if (!IsAdiverySupported())
            {
                return;
            }

            GetAdiveryClass().CallStatic("setLoggingEnabled", enabled);
        }

        public static void PrepareInterstitialAd(string placementId)
        {
            if (!IsAdiverySupported())
            {
                return;
            }
            GetAdiveryClass().CallStatic("prepareInterstitialAd", GetAndroidActivity(), placementId);
        }

        public static void PrepareRewardedAd(string placementId)
        {
            if (!IsAdiverySupported())
            {
                return;
            }
            GetAdiveryClass().CallStatic("prepareRewardedAd", GetAndroidActivity(), placementId);
        }

        internal static void AddListener()
        {
            if (!IsAdiverySupported())
            {
                return;
            }
            listener = new AdiveryListenerImpl();


            adiveryListenreObject.Call("setListener", listener);
        }

        public static void Show(string placement)
        {
            if (!IsAdiverySupported())
            {
                return;
            }
            GetAdiveryClass().CallStatic("showAd", placement);
        }

        public static Boolean IsLoaded(string placementId)
        {
            if (!IsAdiverySupported())
            {
                return false;
            }
            return GetAdiveryClass().CallStatic<bool>("isLoaded", placementId);
        }


        public class AdiveryListenerImpl : AdiveryListener
        {
            public virtual void onError(string placementId, string reason)
            {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    AdiveryError error = new AdiveryError();
                    error.PlacementId = placementId;
                    error.Reason = reason;
                    OnError?.Invoke(this, error);
                });
                
            }

            public virtual void onInterstitialAdLoaded(string placementId) {
                AdiveryEventExecutor.ExecuteInUpdate(() => 
                {
                    OnInterstitialAdLoaded?.Invoke(this,placementId);
                });
            }

            public virtual void onInterstitialAdShown(string placementId) {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    OnInterstitialAdShown?.Invoke(this, placementId);
                });
            }

            public virtual void onInterstitialAdClicked(string placementId) {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    OnInterstitialAdClicked?.Invoke(this, placementId);
                });
            }

            public virtual void onInterstitialAdClosed(string placementId) {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    OnInterstitialAdClosed?.Invoke(this, placementId);
                });
            }

            public virtual void onRewardedAdLoaded(string placementId) {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    OnRewardedAdLoaded?.Invoke(this, placementId);
                });
            }

            public virtual void onRewardedAdShown(string placementId) {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    OnRewardedAdShown?.Invoke(this, placementId);
                });
            }

            public virtual void onRewardedAdClicked(string placementId) {
                AdiveryEventExecutor.ExecuteInUpdate(() =>
                {
                    OnRewardedAdClicked?.Invoke(this, placementId);
                });
            }

            public virtual void onRewardedAdClosed(string placementId, bool isRewarded) {
                AdiveryEventExecutor.ExecuteInUpdate(()=> {
                    AdiveryReward reward = new AdiveryReward();
                    reward.PlacementId = placementId;
                    reward.IsRewarded = isRewarded;
                    OnRewardedAdClosed?.Invoke(this, reward);
                });
            }
        }
    }
}
