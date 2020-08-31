package com.adivery.unity;

import android.app.Activity;
import com.adivery.sdk.Adivery;
import com.adivery.sdk.AdiveryLoadedAd;
import com.adivery.sdk.AdiveryRewardedCallback;

public class Rewarded {

  private String placementId;
  private Activity activity;
  private RewardedCallback callback;
  private AdiveryLoadedAd loadedAd;
  private boolean isLoaded;

  public Rewarded(Activity activity, String placementId, RewardedCallback callback) {
    this.activity = activity;
    this.placementId = placementId;
    this.callback = callback;
    isLoaded = false;
  }

  public void loadAd() {
    Adivery.requestRewardedAd(
        activity,
        placementId,
        new AdiveryRewardedCallback() {
          @Override
          public void onAdLoaded(AdiveryLoadedAd ad) {
            loadedAd = ad;
            isLoaded = true;
            new Thread(
                    new Runnable() {
                      @Override
                      public void run() {
                        callback.onAdLoaded();
                      }
                    })
                .start();
          }

          @Override
          public void onAdLoadFailed(final int errorCode) {
            new Thread(
                    new Runnable() {
                      @Override
                      public void run() {
                        callback.onAdLoadFailed(errorCode);
                      }
                    })
                .start();
          }

          @Override
          public void onAdShown() {
            new Thread(
                    new Runnable() {
                      @Override
                      public void run() {
                        callback.onAdShown();
                      }
                    })
                .start();
          }

          @Override
          public void onAdShowFailed(final int errorCode) {
            new Thread(
                    new Runnable() {
                      @Override
                      public void run() {
                        callback.onAdShowFailed(errorCode);
                      }
                    })
                .start();
          }

          @Override
          public void onAdClicked() {
            new Thread(
                    new Runnable() {
                      @Override
                      public void run() {
                        callback.onAdClicked();
                      }
                    })
                .start();
          }

          @Override
          public void onAdClosed() {
            new Thread(
                    new Runnable() {
                      @Override
                      public void run() {
                        callback.onAdClosed();
                      }
                    })
                .start();
          }

          @Override
          public void onAdRewarded() {
            new Thread(
                    new Runnable() {
                      @Override
                      public void run() {
                        callback.onAdRewarded();
                      }
                    })
                .start();
          }
        });
  }

  public boolean isLoaded() {
    return isLoaded;
  }

  public void show() {
    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            if (isLoaded()) {
              isLoaded = false;
              loadedAd.show();
              loadedAd = null;
            } else {
              Utils.log("Rewarded was not ready to be shown.");
            }
          }
        });
  }

  public void destroy() {
    // Currently there is no rewarded.destroy() method. This method is a placeholder in case
    // there is any cleanup to do here in the future.
  }
}
