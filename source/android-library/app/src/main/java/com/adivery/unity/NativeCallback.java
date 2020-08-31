package com.adivery.unity;

public interface NativeCallback {
  void onAdShowFailed(int errorCode);

  void onAdLoadFailed(int errorCode);

  void onAdClicked();

  void onAdShown();

  void onAdLoaded();
}
