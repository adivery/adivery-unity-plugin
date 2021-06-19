package com.adivery.unity;

public interface NativeCallback {
  void onError(String reason);

  void onAdClicked();

  void onAdShown();

  void onAdLoaded();
}
