package com.adivery.unitylib.interfaces;

public interface AdiveryUnityBannerCallback {
    void onAdLoaded();

    void onAdLoadFailed(int errorCode);

    void onAdShowFailed(int errorCode);

    void onAdClicked();
}
