package com.adivery.unity;

public interface RewardedCallback {
  void onAdLoaded();

  void onAdLoadFailed(int errorCode);

  void onAdShown();

  void onAdShowFailed(int errorCode);

  void onAdRewarded();

  void onAdClicked();

  void onAdClosed();
}
