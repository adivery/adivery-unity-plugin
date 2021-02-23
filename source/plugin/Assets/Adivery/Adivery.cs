using UnityEngine;

namespace AdiveryUnity
{
    public class Adivery
    {
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
