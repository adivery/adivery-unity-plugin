package com.adivery.unitylib.wrappers;

import com.adivery.sdk.AdiveryLoadedAd;
import com.adivery.sdk.AdiveryRewardedCallback;
import com.adivery.unitylib.AdiveryAdCache;
import com.adivery.unitylib.interfaces.AdiveryUnityRewardedCallback;

public class AdiveryRewardedWrapper extends AdiveryRewardedCallback {
    AdiveryUnityRewardedCallback callback;

    public AdiveryRewardedWrapper(AdiveryUnityRewardedCallback callback){
        this.callback = callback;
    }

    @Override
    public void onAdLoaded(AdiveryLoadedAd ad) {
        String id = AdiveryAdCache.newAdId(ad);
        callback.onAdLoaded(id);
    }

    @Override
    public void onAdRewarded() {
        callback.onAdRewarded();
    }

    @Override
    public void onAdShown() {
        callback.onAdShown();
    }

    @Override
    public void onAdShowFailed(int errorCode) {
        callback.onAdShowFailed(errorCode);
    }

    @Override
    public void onAdLoadFailed(int errorCode) {
        callback.onAdLoadFailed(errorCode);
    }

    @Override
    public void onAdClosed() {
        callback.onAdClosed();
    }

    @Override
    public void onAdClicked() {
        callback.onAdClicked();
    }
}
