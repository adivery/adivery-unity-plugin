package com.adivery.unity;

public interface BannerCallback {
  void onAdLoaded();

  void onAdLoadFailed(int errorCode);

  void onAdShowFailed(int errorCode);

  void onAdClicked();
}
