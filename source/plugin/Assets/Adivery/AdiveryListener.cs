using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdiveryListener : AndroidJavaProxy
{
    public AdiveryListener() : base("com.adivery.unity.FullScreenAdCallback") { }

    public virtual void onError(string placementId, string reason) { }

    public virtual void onInterstitialAdLoaded(string placementId) { }

    public virtual void onInterstitialAdShown(string placementId) { }

    public virtual void onInterstitialAdClicked(string placementId) { }

    public virtual void onInterstitialAdClosed(string placementId) { }

    public virtual void onRewardedAdLoaded(string placementId) { }

    public virtual void onRewardedAdShown(string placementId) { }

    public virtual void onRewardedAdClicked(string placementId) { }

    public virtual void onRewardedAdClosed(string placementId, bool isRewarded) { }
}
