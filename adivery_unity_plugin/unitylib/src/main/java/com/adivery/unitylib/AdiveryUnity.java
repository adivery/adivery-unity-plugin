package com.adivery.unitylib;

import android.app.Activity;
import android.inputmethodservice.InputMethodService;
import android.os.Build;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;
import android.view.DisplayCutout;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.view.WindowInsets;
import android.widget.FrameLayout;

import com.adivery.sdk.Adivery;
import com.adivery.sdk.BannerType;
import com.adivery.unitylib.interfaces.AdiveryUnityInterstitialCallback;
import com.adivery.unitylib.wrappers.AdiveryBannerWrapper;
import com.adivery.unitylib.wrappers.AdiveryInterstitialWrapper;
import com.adivery.unitylib.wrappers.AdiveryRewardedWrapper;
import com.adivery.unitylib.interfaces.AdiveryUnityBannerCallback;
import com.adivery.unitylib.interfaces.AdiveryUnityRewardedCallback;

public class AdiveryUnity {

    static int GRAVITY_TOP = 0;
    static int GRAVITY_BOTTOM = 1;

    static boolean bannerInitialized = false;

    static int BANNER = 0;
    static int LARGE_BANNER = 1;
    static int MEDIUM_RECTANGLE = 2;


    public static void configure(Activity activity, String appId) {
        Adivery.configure(activity.getApplication(), appId);
    }

    public static void requestInterstitialAd(Activity activity, String placementId,
                                             AdiveryUnityInterstitialCallback callback) {
        Adivery.requestInterstitialAd(activity, placementId, new AdiveryInterstitialWrapper(callback));
    }

    public static void requestRewardedAd(Activity activity, String placementId,
                                         AdiveryUnityRewardedCallback callback) {
        Adivery.requestRewardedAd(activity, placementId, new AdiveryRewardedWrapper(callback));
    }

    public static void showAd(String adId) {
        AdiveryAdCache.showAd(adId);
    }

    public static void setLoggingEnabled(boolean enabled) {
        Adivery.setLoggingEnabled(enabled);
    }

    public static void requestBannerAd(Activity activity, String placementId, int bannerType,
                                       int gravity, AdiveryUnityBannerCallback callback) {
        if (!bannerInitialized) {
            initializeBannerLayout(activity, gravity);
            // make sure that created view is added to activity layout
            try {
                Thread.sleep(50);
            } catch (InterruptedException ignored) { }
        }

        FrameLayout adLayout = activity.findViewById(R.id.ad_container);
        AdiveryBannerWrapper callbackWrapper = new AdiveryBannerWrapper(adLayout, callback);

        Adivery.requestBannerAd(activity, placementId, getBannerType(bannerType), callbackWrapper);

    }

    public static void setBannerAdEnabled(final Activity activity, final boolean enabled) {
        new Handler(Looper.getMainLooper()).post(new Runnable() {
            @Override
            public void run() {
                View view = activity.findViewById(R.id.ad_container);

                if (view != null) {
                    if (enabled) {
                        view.setVisibility(View.VISIBLE);
                    } else {
                        view.setVisibility(View.GONE);
                    }
                }
            }
        });
    }

    private static BannerType getBannerType(int bannerType) {
        if (bannerType == BANNER) {
            return BannerType.BANNER;
        } else if (bannerType == LARGE_BANNER) {
            return BannerType.LARGE_BANNER;
        }
        return BannerType.MEDIUM_RECTANGLE;
    }

    private static void initializeBannerLayout(final Activity activity, int gravity) {
        final FrameLayout layout = (FrameLayout) activity.getLayoutInflater()
                .inflate(R.layout.ad_container, null, false);
        final FrameLayout.LayoutParams params = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT,
                ViewGroup.LayoutParams.WRAP_CONTENT);
        if (gravity == GRAVITY_TOP) {
            params.gravity = Gravity.TOP;
        } else {
            params.gravity = Gravity.BOTTOM;
        }
        Insets insets = getSafeInsets(activity);
        params.setMargins(insets.left,insets.top,insets.right,insets.bottom);

        new Handler(Looper.getMainLooper()).post(
                new Runnable() {
                    @Override
                    public void run() {
                        activity.getWindow().addContentView(layout, params);
                    }
                }
        );
        bannerInitialized = true;
    }

    private static Insets getSafeInsets(Activity activity) {
        Insets insets = new Insets();

        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.P) {
            return insets;
        }
        Window window = activity.getWindow();
        if (window == null) {
            return insets;
        }
        WindowInsets windowInsets = window.getDecorView().getRootWindowInsets();
        if (windowInsets == null) {
            return insets;
        }
        DisplayCutout displayCutout = windowInsets.getDisplayCutout();
        if (displayCutout == null) {
            return insets;
        }
        insets.top = displayCutout.getSafeInsetTop();
        insets.left = displayCutout.getSafeInsetLeft();
        insets.bottom = displayCutout.getSafeInsetBottom();
        insets.right = displayCutout.getSafeInsetRight();
        return insets;
    }
}
