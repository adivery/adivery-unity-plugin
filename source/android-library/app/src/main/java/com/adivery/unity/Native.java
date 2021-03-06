package com.adivery.unity;

import android.app.Activity;
import android.graphics.Bitmap;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;
import com.adivery.sdk.Adivery;
import com.adivery.sdk.AdiveryNativeAd;
import com.adivery.sdk.AdiveryNativeCallback;
import java.io.ByteArrayOutputStream;

public class Native {

  private final String placementId;
  private final Activity activity;
  private final NativeCallback callback;
  private AdiveryNativeAd loadedAd;
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
          public void onAdLoaded(AdiveryNativeAd ad) {
            loadedAd = ad;
            loading = false;
            Utils.execute(new Runnable() {
              @Override
              public void run() {
                iconBytes = getDrawableBytes(loadedAd.getIcon());
                imageBytes = getDrawableBytes(loadedAd.getImage());
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
    return loadedAd != null ? loadedAd.getHeadline() : null;
  }

  public String getDescription() {
    return loadedAd != null ? loadedAd.getDescription() : null;
  }

  public String getAdvertiser() {
    return loadedAd != null ? loadedAd.getAdvertiser() : null;
  }

  public String getCallToAction() {
    return loadedAd != null ? loadedAd.getCallToAction() : null;
  }

  public byte[] getIcon() {
    return iconBytes;
  }

  public byte[] getImage() {
    return imageBytes;
  }

  public void recordImpression() {
    if (loadedAd != null) {
      loadedAd.recordImpression();
    }
  }

  public void recordClick() {
    if (loadedAd != null) {
      loadedAd.recordClick();
    }
  }

  public void destroy() {
    // Currently there is no native.destroy() method. This method is a placeholder in case
    // there is any cleanup to do here in the future.
  }
}
