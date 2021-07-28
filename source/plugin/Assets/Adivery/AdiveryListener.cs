using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


namespace AdiveryUnity
{
    public class AdiveryError : EventArgs
    {
        public string PlacementId;
        public string Reason;
    }

    public class AdiveryReward : EventArgs
    {
        public string PlacementId;
        public bool IsRewarded;
    }

    public class AdiveryListener : AndroidJavaProxy
    {
        internal AndroidJavaObject adiveryListenreObject = new AndroidJavaObject("com.adivery.sdk.plugins.unity.FullScreenAd");

        private EventHandler<AdiveryError> _onError;
        public event EventHandler<AdiveryError> OnError
        {
            add
            {
                if (_onError == null || !_onError.GetInvocationList().Contains(value))
                {
                    _onError += value;
                }
            }

            remove
            {
                _onError -= value;
            }
        }
        private EventHandler<string> _onInterstitialAdLoaded;
        public event EventHandler<string> OnInterstitialAdLoaded
        {
            add
            {
                if (_onInterstitialAdLoaded == null || !_onInterstitialAdLoaded.GetInvocationList().Contains(value))
                {
                    _onInterstitialAdLoaded += value;
                }
            }

            remove
            {
                _onInterstitialAdLoaded -= value;
            }
        }
        private EventHandler<string> _onInterstitialAdClicked;
        public event EventHandler<string> OnInterstitialAdClicked
        {
            add
            {
                if (_onInterstitialAdClicked == null || !_onInterstitialAdClicked.GetInvocationList().Contains(value))
                {
                    _onInterstitialAdClicked += value;
                }
            }

            remove
            {
                _onInterstitialAdClicked -= value;
            }
        }
        private event EventHandler<string> _onInterstitialAdShown;
        public event EventHandler<string> OnInterstitialAdShown
        {
            add
            {
                if (_onInterstitialAdShown == null || !_onInterstitialAdShown.GetInvocationList().Contains(value))
                {
                    _onInterstitialAdShown += value;
                }
            }

            remove
            {
                _onInterstitialAdShown -= value;
            }
        }
        private EventHandler<string> _onInterstitialAdClosed;
        public event EventHandler<string> OnInterstitialAdClosed
        {
            add
            {
                if (_onInterstitialAdClosed == null || !_onInterstitialAdClosed.GetInvocationList().Contains(value))
                {
                    _onInterstitialAdClosed += value;
                }
            }

            remove
            {
                _onInterstitialAdClosed -= value;
            }
        }

        private EventHandler<string> _onRewardedAdLoaded;
        public event EventHandler<string> OnRewardedAdLoaded
        {
            add
            {
                if (_onRewardedAdLoaded == null || !_onRewardedAdLoaded.GetInvocationList().Contains(value))
                {
                    _onRewardedAdLoaded += value;
                }
            }

            remove
            {
                _onRewardedAdLoaded -= value;
            }
        }
        private EventHandler<string> _onRewardedAdShown;
        public event EventHandler<string> OnRewardedAdShown
        {
            add
            {
                if (_onRewardedAdShown == null || !_onRewardedAdShown.GetInvocationList().Contains(value))
                {
                    _onRewardedAdShown += value;
                }
            }

            remove
            {
                _onRewardedAdShown -= value;
            }
        }
        private EventHandler<string> _onRewardedAdClicked;
        public event EventHandler<string> OnRewardedAdClicked
        {
            add
            {
                if (_onRewardedAdClicked == null || !_onRewardedAdClicked.GetInvocationList().Contains(value))
                {
                    _onRewardedAdClicked += value;
                }
            }

            remove
            {
                _onRewardedAdClicked -= value;
            }
        }

        private EventHandler<AdiveryReward> _onRewardedAdClosed;
        public event EventHandler<AdiveryReward> OnRewardedAdClosed
        {
            add
            {
                if (_onRewardedAdClosed == null || !_onRewardedAdClosed.GetInvocationList().Contains(value))
                {
                    _onRewardedAdClosed += value;
                }
            }

            remove
            {
                _onRewardedAdClosed -= value;
            }
        }

        public AdiveryListener() : base("com.adivery.sdk.plugins.unity.FullScreenAdCallback") { }

        public virtual void onError(string placementId, string reason)
        {
            AdiveryEventExecutor.ExecuteInUpdate(() =>
            {
                AdiveryError error = new AdiveryError();
                error.PlacementId = placementId;
                error.Reason = reason;
                _onError?.Invoke(this, error);
            });

        }

        public virtual void onInterstitialAdLoaded(string placementId)
        {
            AdiveryEventExecutor.ExecuteInUpdate(() =>
            {
                _onInterstitialAdLoaded?.Invoke(this, placementId);
            });
        }

        public virtual void onInterstitialAdShown(string placementId)
        {
            AdiveryEventExecutor.ExecuteInUpdate(() =>
            {
                _onInterstitialAdShown?.Invoke(this, placementId);
            });
        }

        public virtual void onInterstitialAdClicked(string placementId)
        {
            AdiveryEventExecutor.ExecuteInUpdate(() =>
            {
                _onInterstitialAdClicked?.Invoke(this, placementId);
            });
        }

        public virtual void onInterstitialAdClosed(string placementId)
        {
            AdiveryEventExecutor.ExecuteInUpdate(() =>
            {
                _onInterstitialAdClosed?.Invoke(this, placementId);
            });
        }

        public virtual void onRewardedAdLoaded(string placementId)
        {
            AdiveryEventExecutor.ExecuteInUpdate(() =>
            {
                _onRewardedAdLoaded?.Invoke(this, placementId);
            });
        }

        public virtual void onRewardedAdShown(string placementId)
        {
            AdiveryEventExecutor.ExecuteInUpdate(() =>
            {
                _onRewardedAdShown?.Invoke(this, placementId);
            });
        }

        public virtual void onRewardedAdClicked(string placementId)
        {
            AdiveryEventExecutor.ExecuteInUpdate(() =>
            {
                _onRewardedAdClicked?.Invoke(this, placementId);
            });
        }

        public virtual void onRewardedAdClosed(string placementId, bool isRewarded)
        {
            AdiveryEventExecutor.ExecuteInUpdate(() =>
            {
                AdiveryReward reward = new AdiveryReward();
                reward.PlacementId = placementId;
                reward.IsRewarded = isRewarded;
                _onRewardedAdClosed?.Invoke(this, reward);
            });

        }
    }

}
