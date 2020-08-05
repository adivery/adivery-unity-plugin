package com.adivery.unitylib.wrappers;

import com.adivery.sdk.AdiveryLoadedAd;
import com.adivery.unitylib.AdiveryAdCache;
import com.adivery.unitylib.interfaces.AdiveryUnityInterstitialCallback;
import com.adivery.sdk.AdiveryInterstitialCallback;

public class AdiveryInterstitialWrapper extends AdiveryInterstitialCallback {
    AdiveryUnityInterstitialCallback callback;
    public AdiveryInterstitialWrapper(AdiveryUnityInterstitialCallback callback){
        this.callback = callback;
    }

    @Override
    public void onAdLoaded(AdiveryLoadedAd ad) {
        String id = AdiveryAdCache.newAdId(ad);
        callback.onAdLoaded(id);
    }

    @Override
    public void onAdClicked() {
        callback.onAdClicked();
    }

    @Override
    public void onAdClosed() {
        callback.onAdClosed();
    }

    @Override
    public void onAdLoadFailed(int errorCode) {
        callback.onAdLoadFailed(errorCode);
    }

    @Override
    public void onAdShowFailed(int errorCode) {
        callback.onAdShowFailed(errorCode);
    }

    @Override
    public void onAdShown() {
        callback.onAdShown();
    }
}
