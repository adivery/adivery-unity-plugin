package com.adivery.unity;

import android.app.Activity;

import com.adivery.sdk.Adivery;
import com.adivery.sdk.unified.AdiveryListener;

public class FullScreenAd {
  public void setListener(final FullScreenAdCallback callback){
    Adivery.addListener(new AdiveryListener(){
      @Override
      public void onError(final String placementId, final String reason) {
        Utils.execute(new Runnable() {
          @Override
          public void run() {
            callback.onError(placementId, reason);
          }
        });
      }

      @Override
      public void onInterstitialAdClicked(final String placementId) {
        Utils.execute(new Runnable() {
          @Override
          public void run() {
            callback.onInterstitialAdClicked(placementId);
          }
        });
      }

      @Override
      public void onInterstitialAdClosed(final String placement) {
        Utils.execute(new Runnable() {
          @Override
          public void run() {
            callback.onInterstitialAdClosed(placement);
          }
        });
      }

      @Override
      public void onInterstitialAdLoaded(final String placementId) {
        Utils.execute(new Runnable() {
          @Override
          public void run() {
            callback.onInterstitialAdLoaded(placementId);
          }
        });
      }

      @Override
      public void onInterstitialAdShown(final String placementId) {
        Utils.execute(new Runnable() {
          @Override
          public void run() {
            callback.onInterstitialAdShown(placementId);
          }
        });
      }

      @Override
      public void onRewardedAdClicked(final String placementId) {
        Utils.execute(new Runnable() {
          @Override
          public void run() {
            callback.onRewardedAdClicked(placementId);
          }
        });
      }

      @Override
      public void onRewardedAdClosed(final String placementId, final boolean isRewarded) {
        Utils.execute(new Runnable() {
          @Override
          public void run() {
            callback.onRewardedAdClosed(placementId, isRewarded);
          }
        });
      }

      @Override
      public void onRewardedAdLoaded(final String placementId) {
        Utils.execute(new Runnable() {
          @Override
          public void run() {
            callback.onRewardedAdLoaded(placementId);
          }
        });
      }

      @Override
      public void onRewardedAdShown(final String placementId) {
        Utils.execute(new Runnable() {
          @Override
          public void run() {
            callback.onRewardedAdShown(placementId);
          }
        });
      }
    });

  }
}
