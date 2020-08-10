using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using adivery;


public class MyRewardedAdCallback : RewardedAdCallback 
{
    public override void onAdLoaded(string adId)
    {
        Adivery.showAd(adId);
    }

    public override void onAdRewarded()
    {

    }
}
