package com.adivery.unity;

import android.app.Activity;
import com.adivery.sdk.Adivery;
import com.adivery.sdk.AdiveryInterstitialCallback;
import com.adivery.sdk.AdiveryLoadedAd;

public class Interstitial {

  private final String placementId;
  private final Activity activity;
  private final InterstitialCallback callback;
  private AdiveryLoadedAd loadedAd;
  private boolean loading = false;

  public Interstitial(Activity activity, String placementId, InterstitialCallback callback) {
    this.activity = activity;
    this.placementId = placementId;
    this.callback = callback;
  }

  public void loadAd() {
    if (loading || isLoaded()) {
      return;
    }

    loadedAd = null;
    loading = true;

    Adivery.requestInterstitialAd(
        activity,
        placementId,
        new AdiveryInterstitialCallback() {
          @Override
          public void onAdLoaded(AdiveryLoadedAd ad) {
            loadedAd = ad;
            loading = false;
            Utils.execute(new Runnable() {
              @Override
              public void run() {
                callback.onAdLoaded();
              }
            });
          }

          @Override
          public void onAdLoadFailed(final int errorCode) {
            loading = false;
            Utils.execute(new Runnable() {
              @Override
              public void run() {
                callback.onAdLoadFailed(errorCode);
              }
            });
          }

          @Override
          public void onAdShown() {
            Utils.execute(new Runnable() {
              @Override
              public void run() {
                callback.onAdShown();
              }
            });
          }

          @Override
          public void onAdShowFailed(final int errorCode) {
            Utils.execute(new Runnable() {
              @Override
              public void run() {
                callback.onAdShowFailed(errorCode);
              }
            });
          }

          @Override
          public void onAdClicked() {
            Utils.execute(new Runnable() {
              @Override
              public void run() {
                callback.onAdClicked();
              }
            });
          }

          @Override
          public void onAdClosed() {
            Utils.execute(new Runnable() {
              @Override
              public void run() {
                callback.onAdClosed();
              }
            });
          }
        });
  }

  public boolean isLoaded() {
    return loadedAd != null;
  }

  public void show() {
    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            if (isLoaded()) {
              loadedAd.show();
              loadedAd = null;
            } else {
              Utils.log("Interstitial was not ready to be shown.");
            }
          }
        });
  }

  public void destroy() {
    // Currently there is no interstitial.destroy() method. This method is a placeholder in case
    // there is any cleanup to do here in the future.
  }
}
