package com.adivery.unitylib.wrappers;

import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.widget.FrameLayout;

import com.adivery.sdk.AdiveryBannerCallback;
import com.adivery.unitylib.interfaces.AdiveryUnityBannerCallback;

public class AdiveryBannerWrapper extends AdiveryBannerCallback {
    private FrameLayout layout;
    private AdiveryUnityBannerCallback callback;

    public AdiveryBannerWrapper(FrameLayout layout, AdiveryUnityBannerCallback callback){
        this.layout = layout;
        this.callback = callback;

    }

    @Override
    public void onAdLoaded(View adView) {
        FrameLayout.LayoutParams params = (FrameLayout.LayoutParams) adView.getLayoutParams();
        if (params == null){
            params = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WRAP_CONTENT, ViewGroup.LayoutParams.WRAP_CONTENT);
        }
        params.gravity = Gravity.CENTER;
        adView.setLayoutParams(params);

        layout.addView(adView);
        callback.onAdLoaded();
    }

    @Override
    public void onAdClicked() {
        callback.onAdClicked();
    }

    @Override
    public void onAdLoadFailed(int errorCode) {
        callback.onAdLoadFailed(errorCode);
    }

    @Override
    public void onAdShowFailed(int errorCode) {
        callback.onAdShowFailed(errorCode);
    }
}
