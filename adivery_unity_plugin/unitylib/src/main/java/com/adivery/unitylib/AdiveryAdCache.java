package com.adivery.unitylib;

import com.adivery.sdk.AdiveryLoadedAd;

import java.util.HashMap;
import java.util.Map;
import java.util.UUID;

public class AdiveryAdCache {
    static Map<String, AdiveryLoadedAd> adCache = new HashMap<>();

    public static String newAdId(AdiveryLoadedAd ad){
        String id = UUID.randomUUID().toString();
        adCache.put(id,ad);
        return id;
    }

    public static void showAd(String adId){
        AdiveryLoadedAd ad = adCache.get(adId);
        if (ad!=null){
            ad.show();
        }
    }

}
