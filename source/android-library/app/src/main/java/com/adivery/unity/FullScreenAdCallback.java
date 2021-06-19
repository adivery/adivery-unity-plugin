package com.adivery.unity;

public interface FullScreenAdCallback {

  void onError(String placementId, String reason);

  void onInterstitialAdLoaded(String placementId);

  void onInterstitialAdShown(String placementId);

  void onInterstitialAdClicked(String placementId);

  void onInterstitialAdClosed(String placement);

  void onRewardedAdLoaded(String placementId);

  void onRewardedAdShown(String placementId);

  void onRewardedAdClicked(String placementId);

  void onRewardedAdClosed(String placementId, boolean isRewarded);
}
