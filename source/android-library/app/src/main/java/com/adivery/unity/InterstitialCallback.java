package com.adivery.unity;

public interface InterstitialCallback {
  void onAdLoaded();

  void onAdLoadFailed(int errorCode);

  void onAdShown();

  void onAdShowFailed(int errorCode);

  void onAdClicked();

  void onAdClosed();
}
