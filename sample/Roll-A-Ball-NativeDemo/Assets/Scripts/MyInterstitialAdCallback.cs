using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using adivery;

public class MyInterstitialCallback : InterstitialAdCallback
{
    public override void onAdLoaded(string adId)
    {
        Adivery.showAd(adId);
    }
}
