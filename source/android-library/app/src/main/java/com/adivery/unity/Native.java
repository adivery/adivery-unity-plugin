package com.adivery.unity;

import android.app.Activity;
import android.graphics.Bitmap;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;

import com.adivery.sdk.Adivery;
import com.adivery.sdk.AdiveryNativeCallback;
import java.io.ByteArrayOutputStream;
import com.adivery.sdk.NativeAd;
import com.adivery.sdk.networks.adivery.AdiveryNativeAd;
import com.adivery.sdk.networks.admob.AdMobNativeAd;

public class Native {

  private final String placementId;
  private final Activity activity;
  private final NativeCallback callback;
  private NativeAd loadedAd;
  private boolean loading = false;

  private byte[] iconBytes;
  private byte[] imageBytes;

  public Native(Activity activity, String placementId, NativeCallback callback) {
    this.placementId = placementId;
    this.activity = activity;
    this.callback = callback;
  }

  public void loadAd() {
    if (loading) {
      return;
    }

    loading = true;

    Adivery.requestNativeAd(
        activity,
        placementId,
        new AdiveryNativeCallback() {
          @Override
          public void onAdLoaded(final NativeAd ad) {
            loadedAd = ad;
            if (ad instanceof AdiveryNativeAd) {
              loading = false;
              Utils.execute(new Runnable() {
                @Override
                public void run() {
                  iconBytes = getDrawableBytes(((AdiveryNativeAd)loadedAd).getIcon());
                  imageBytes = getDrawableBytes(((AdiveryNativeAd)loadedAd).getImage());
                  callback.onAdLoaded();
                }
              });
            } else {
              Utils.execute(new Runnable() {
                @Override
                public void run() {
                  AdMobNativeAd nativeAd = (AdMobNativeAd) ad;
                  iconBytes = getDrawableBytes(nativeAd.getIcon());
                  imageBytes = getDrawableBytes(nativeAd.getImage());
                  callback.onAdLoaded();
                }
              });
            }

          }

          @Override
          public void onAdLoadFailed(final String reason) {
            loading = false;
            Utils.execute(new Runnable() {
              @Override
              public void run() {
                callback.onError(reason);
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
          public void onAdShowFailed(final String reason) {
            Utils.execute(new Runnable() {
              @Override
              public void run() {
                callback.onError(reason);
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
        });
  }

  private static byte[] getDrawableBytes(Drawable drawable) {
    if (drawable instanceof BitmapDrawable) {
      Bitmap bitmap = ((BitmapDrawable) drawable).getBitmap();
      ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
      bitmap.compress(Bitmap.CompressFormat.PNG, 100, outputStream);
      return outputStream.toByteArray();
    }
    return null;
  }

  public boolean isLoaded() {
    return loadedAd != null;
  }

  public String getHeadline() {
    if (loadedAd instanceof AdiveryNativeAd){
      return ((AdiveryNativeAd) loadedAd).getHeadline();
    } else if (loadedAd instanceof AdMobNativeAd){
      return ((AdMobNativeAd) loadedAd).getHeadline();
    }
    return null;
  }

  public String getDescription() {
    if (loadedAd instanceof AdiveryNativeAd){
      return ((AdiveryNativeAd) loadedAd).getDescription();
    } else if (loadedAd instanceof AdMobNativeAd){
      return ((AdMobNativeAd) loadedAd).getDescription();
    }
    return null;
  }

  public String getAdvertiser() {
    if (loadedAd instanceof AdiveryNativeAd){
      return ((AdiveryNativeAd) loadedAd).getAdvertiser();
    } else if (loadedAd instanceof AdMobNativeAd){
      return ((AdMobNativeAd) loadedAd).getAdvertiser();
    }
    return null;
  }

  public String getCallToAction() {
    if (loadedAd instanceof AdiveryNativeAd){
      return ((AdiveryNativeAd) loadedAd).getCallToAction();
    } else if (loadedAd instanceof AdMobNativeAd){
      return ((AdMobNativeAd) loadedAd).getCallToAction();
    }
    return null;
  }

  public byte[] getIcon() {
    return iconBytes;
  }

  public byte[] getImage() {
    return imageBytes;
  }

  public void recordImpression() {
    if (loadedAd instanceof AdiveryNativeAd){
      ((AdiveryNativeAd) loadedAd).recordImpression();
    } else if (loadedAd instanceof AdMobNativeAd){
      ((AdMobNativeAd) loadedAd).recordImpression();
    }
  }

  public void recordClick() {
    if (loadedAd instanceof AdiveryNativeAd){
      ((AdiveryNativeAd) loadedAd).recordClick();
    }
  }

  public void destroy() {
    // Currently there is no native.destroy() method. This method is a placeholder in case
    // there is any cleanup to do here in the future.
  }
}
