package com.adivery.unity;

import android.app.Activity;
import android.os.Build;
import android.util.Log;
import android.view.DisplayCutout;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.view.ViewParent;
import android.view.Window;
import android.view.WindowInsets;
import android.widget.FrameLayout;

import com.adivery.sdk.AdiveryAdListener;
import com.adivery.sdk.AdiveryBannerAdView;
import com.adivery.sdk.BannerSize;

public class Banner {

  private static final int POSITION_TOP = 0;
  private static final int POSITION_BOTTOM = 1;

  private static final int TYPE_BANNER = 0;
  private static final int TYPE_LARGE_BANNER = 1;
  private static final int TYPE_MEDIUM_RECTANGLE = 2;
  private static final int TYPE_SMART_BANNER = 3;

  private final String placementId;
  private final Activity activity;
  private final BannerSize bannerType;
  private final int bannerGravity;
  private final BannerCallback callback;
  private FrameLayout bannerContainer;
  private boolean loading = false;
  private boolean loaded = false;
  private AdiveryBannerAdView adView;

  public Banner(
      Activity activity,
      String placementId,
      int bannerType,
      int bannerPosition,
      BannerCallback callback) {
    this.activity = activity;
    this.placementId = placementId;
    this.bannerType = getBannerType(bannerType);
    this.bannerGravity = getBannerGravity(bannerPosition);
    this.callback = callback;
    Log.d("PLUGIN", "banner created");
  }

  private static int getBannerGravity(int bannerPosition) {
    switch (bannerPosition) {
      case POSITION_TOP:
        return Gravity.TOP;
      case POSITION_BOTTOM:
        return Gravity.BOTTOM;
      default:
        return Gravity.CENTER;
    }
  }

  public void loadAd() {
    if (loading) {
      return;
    }

    loading = true;
    activity.runOnUiThread(new Runnable() {
      @Override
      public void run() {
        adView = new AdiveryBannerAdView(activity);
        adView.setBannerSize(bannerType);
        adView.setPlacementId(placementId);
        adView.setBannerAdListener(new AdiveryAdListener() {
          @Override
          public void onAdLoaded() {
            loaded = true;
            Utils.execute(new Runnable() {
              @Override
              public void run() {
                callback.onAdLoaded();
              }
            });
          }

          @Override
          public void onError(final String reason) {
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
        if (bannerContainer == null) {
          bannerContainer = new FrameLayout(activity);
          activity.addContentView(bannerContainer, getLayoutParams());
          bannerContainer.removeAllViews();
          bannerContainer.addView(adView);
        }
        adView.loadAd();
      }
    });
  }

  private FrameLayout.LayoutParams getLayoutParams() {
    final FrameLayout.LayoutParams adParams =
        new FrameLayout.LayoutParams(
            FrameLayout.LayoutParams.WRAP_CONTENT, FrameLayout.LayoutParams.WRAP_CONTENT);
    adParams.gravity = bannerGravity | Gravity.CENTER_HORIZONTAL;

    Insets insets = getSafeInsets();
    adParams.setMargins(insets.left, insets.top, insets.right, insets.bottom);

    return adParams;
  }

  private BannerSize getBannerType(int bannerType) {
    switch (bannerType) {
      case TYPE_BANNER:
        return BannerSize.BANNER;
      case TYPE_LARGE_BANNER:
        return BannerSize.LARGE_BANNER;
      case TYPE_MEDIUM_RECTANGLE:
        return BannerSize.MEDIUM_RECTANGLE;
      case TYPE_SMART_BANNER:
        return BannerSize.SMART_BANNER;
      default:
        return null;
    }
  }

  private Insets getSafeInsets() {
    Insets insets = new Insets();

    if (Build.VERSION.SDK_INT < Build.VERSION_CODES.P) {
      return insets;
    }
    Window window = activity.getWindow();
    if (window == null) {
      return insets;
    }
    WindowInsets windowInsets = window.getDecorView().getRootWindowInsets();
    if (windowInsets == null) {
      return insets;
    }
    DisplayCutout displayCutout = windowInsets.getDisplayCutout();
    if (displayCutout == null) {
      return insets;
    }
    insets.top = displayCutout.getSafeInsetTop();
    insets.left = displayCutout.getSafeInsetLeft();
    insets.bottom = displayCutout.getSafeInsetBottom();
    insets.right = displayCutout.getSafeInsetRight();
    return insets;
  }

  public boolean isLoaded() {
    return loaded;
  }

  public void show() {
    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            if (bannerContainer != null) {
              bannerContainer.setVisibility(View.VISIBLE);
            }
          }
        });
  }

  public void hide() {
    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            if (bannerContainer != null) {
              bannerContainer.setVisibility(View.GONE);
            }
          }
        });
  }

  public void destroy() {
    activity.runOnUiThread(
        new Runnable() {
          @Override
          public void run() {
            if (bannerContainer != null) {
              ViewParent parentView = bannerContainer.getParent();
              if (parentView instanceof ViewGroup) {
                ((ViewGroup) parentView).removeView(bannerContainer);
              }
            }
          }
        });
  }

  private static class Insets {

    int top = 0;
    int bottom = 0;
    int left = 0;
    int right = 0;
  }
}
