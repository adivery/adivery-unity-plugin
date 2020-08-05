package com.adivery.unitylib.interfaces;

public interface AdiveryUnityRewardedCallback {
    void onAdLoaded(String id);

    void onAdLoadFailed(int errorCode);

    void onAdShown();

    void onAdShowFailed(int errorCode);

    void onAdRewarded();

    void onAdClicked();

    void onAdClosed();
}
