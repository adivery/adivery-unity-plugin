using UnityEngine;

namespace AdiveryUnity
{
    public class Adivery
    {
        internal static bool IsAdiverySupported()
        {
            return Application.platform == RuntimePlatform.Android;
        }

        internal static bool IsAdMobSupported()
        {
            AndroidJavaClass mobileAdsClass = null;
            try
            {
                mobileAdsClass = new AndroidJavaClass("com.google.android.gms.ads.MobileAds");
            } catch (AndroidJavaException)
            {
                // MobileAds class was not found
            }
            return mobileAdsClass != null;
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

            if (!IsAdMobSupported())
            {
                // Initialize without AdMob adapter
                GetAdiveryClass().CallStatic("configure", GetAndroidApplication(), appId, new AndroidJavaObject[] { });
                return;
            }

            // Initialize with AdMob adapter
            AndroidJavaObject adMobAdapter = new AndroidJavaObject("com.adivery.sdk.networks.admob.AdMobAdapter");
            GetAdiveryClass().CallStatic("configure", GetAndroidApplication(), appId, new AndroidJavaObject[] { adMobAdapter });
        }

        public static void SetLoggingEnabled(bool enabled)
        {
            if (!IsAdiverySupported())
            {
                return;
            }

            GetAdiveryClass().CallStatic("setLoggingEnabled", enabled);
        }
    }
}
