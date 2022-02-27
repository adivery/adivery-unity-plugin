using UnityEngine;
using System;
using System.Linq;

namespace AdiveryUnity
{
    public class Adivery
    {
        internal static AndroidJavaObject adiveryListenreObject = new AndroidJavaObject("com.adivery.sdk.plugins.unity.FullScreenAd");

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
            return new AndroidJavaClass("com.adivery.sdk.plugins.unity.AdiveryUnity");
        }

        public static void Configure(string appId)
        {
            if (!IsAdiverySupported())
            {
                return;
            }

            AdiveryEventExecutor.Initialize();
            GetAdiveryClass().CallStatic("configure", GetAndroidApplication(), appId);
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

        internal static void AddListener(AdiveryListener listener)
        {
            if (!IsAdiverySupported())
            {
                return;
            }
            listener.adiveryListenreObject.Call("setListener", listener);
        }

        internal static void RemoveListener(AdiveryListener listener)
        {
            if (!IsAdiverySupported())
            {
                return;
            }
            listener.adiveryListenreObject.Call("removeListener");
        }

        internal static void AddPlacementListener(string placementId, AdiveryListener listener){
            if (!IsAdiverySupported()){
                return;
            }
            listener.adiveryListenreObject.Call("addPlacementListener", placementId, listener);
        }

        internal static void RemovePlacementListener(string placementId, AdiveryListener listener){
            if (!IsAdiverySupported()){
                return;
            }
            listener.adiveryListenreObject.Call("removePlacementListener", placementId);
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
    }
}
