#if UNITY_ANDROID
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace adivery
{
    public class Adivery
    {
        public static int GRAVITY_TOP = 0;
        public static int GRAVITY_BOTTOM = 1;

        public static int BANNER = 0;
        public static int LARGE_BANNER = 1;
        public static int MEDIUM_RECTANGLE = 2;

        public static void configure(String appId){
            if (Application.isEditor)
            {
                return;
            }
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaClass adivery = new AndroidJavaClass("com.adivery.unitylib.AdiveryUnity");
            adivery.CallStatic("configure", activity,appId);

        }

        public static void showAd(String adId)
        {
            if (Application.isEditor)
            {
                return;
            }
            AndroidJavaClass adivery = new AndroidJavaClass("com.adivery.unitylib.AdiveryUnity");
            adivery.CallStatic("showAd", adId);
        }

        public static void setLoggingEnabled(bool enabled)
        {
            if (Application.isEditor)
            {
                return;
            }
            AndroidJavaClass adivery = new AndroidJavaClass("com.adivery.unitylib.AdiveryUnity");
            adivery.CallStatic("setLoggingEnabled", enabled);
        }

        public static void requestInterstitalAd(String placementId, InterstitialAdCallback callback)
        {
            if (Application.isEditor)
            {
                return;
            }
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaClass adivery = new AndroidJavaClass("com.adivery.unitylib.AdiveryUnity");
            adivery.CallStatic("requestInterstitialAd", activity, placementId, callback);
        }

        public static void requestRewardedAd(String placementId, RewardedAdCallback callback)
        {
            if (Application.isEditor)
            {
                return;
            }
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaClass adivery = new AndroidJavaClass("com.adivery.unitylib.AdiveryUnity");
            adivery.CallStatic("requestRewardedAd", activity, placementId, callback);
        }

        public static void requestBannerAd(String placementId,int bannerType, int gravity, BannerAdCallback callback)
        {
            if (Application.isEditor)
            {
                return;
            }
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaClass adivery = new AndroidJavaClass("com.adivery.unitylib.AdiveryUnity");
            adivery.CallStatic("requestBannerAd", activity, placementId, bannerType, gravity, callback);
        }

        public static void setBannerEnabled(bool enabled)
        {
            if (Application.isEditor)
            {
                return;
            }
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaClass adivery = new AndroidJavaClass("com.adivery.unitylib.AdiveryUnity");

            adivery.CallStatic("setBannerAdEnabled", activity, enabled);
        }
    }
    
}
#endif
