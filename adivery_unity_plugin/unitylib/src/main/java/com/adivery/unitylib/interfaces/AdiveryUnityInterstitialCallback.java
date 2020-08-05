package com.adivery.unitylib.interfaces;

public interface AdiveryUnityInterstitialCallback {
    void onAdLoaded(String ad);

    void onAdLoadFailed(int errorCode);

    void onAdShown();

    void onAdShowFailed(int errorCode);

    void onAdClicked();

    void onAdClosed();
}