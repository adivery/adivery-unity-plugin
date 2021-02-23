package com.adivery.unity;

import android.util.Log;
import java.util.concurrent.LinkedBlockingQueue;
import java.util.concurrent.ThreadPoolExecutor;
import java.util.concurrent.TimeUnit;

class Utils {

  private static final ThreadPoolExecutor executor = new ThreadPoolExecutor(1, 1, 0L,
      TimeUnit.MILLISECONDS, new LinkedBlockingQueue<Runnable>());

  static void execute(Runnable runnable) {
    executor.execute(runnable);
  }

  static void log(String message) {
    Log.i("AdiveryUnityPlugin", message);
  }
}
