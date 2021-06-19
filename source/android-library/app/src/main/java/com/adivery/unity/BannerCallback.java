package com.adivery.unity;

public interface BannerCallback {
  void onAdLoaded();

  void onError(String reason);

  void onAdClicked();
}
