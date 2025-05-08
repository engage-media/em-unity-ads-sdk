using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace EMAds.Ads
{
    public class EMAdsSdkManager : MonoBehaviour, IAdEventListener
    {
        private AdBreakPlayer adBreakPlayer;
        internal EMAdSdkConfig adConfig;
        private IAdEventListener adSdkListener;

        private EngageAdsUnityBridge engageAdsBridge;
        private bool isAndroid;

        // Entry point to initialize the SDK with config, video player, and listener
        public void Initialize(EMAdSdkConfig config, VideoPlayer videoView, IAdEventListener listener)
        {
            this.adConfig = config;
            this.adSdkListener = listener;

            if (RuntimePlatform.Android == Application.platform)
            {
                isAndroid = true;
                engageAdsBridge = gameObject.AddComponent<EngageAdsUnityBridge>();
                engageAdsBridge.Initialize(config, listener);
            }
            else
            {
                isAndroid = false;
                // Create and initialize AdBreakPlayer for non-Android platforms
                adBreakPlayer = gameObject.AddComponent<AdBreakPlayer>();
                adBreakPlayer.Initialize(this);
                adBreakPlayer.videoPlayer = videoView;
                LoadAds();
            }
        }

        public void PlayNextAd()
        {
            if (isAndroid && engageAdsBridge != null)
            {
                engageAdsBridge.ShowAd();
            }
            else
            {
                adBreakPlayer.PlayNextAd();
            }
        }

        private void LoadAds()
        {
            if (isAndroid && engageAdsBridge != null)
            {
                engageAdsBridge.LoadAd();
            }
            else
            {
                ContentToAdAdapter adAdapter = gameObject.AddComponent<ContentToAdAdapter>();
                adAdapter.adEventListener = this;
                adAdapter.adBreakPlayer = adBreakPlayer;
                adAdapter.adConfig = adConfig;

                adAdapter.LoadAds();
            }
        }

        // Methods to handle ad events and notify the user
        public void OnAdBreakStarted()
        {
            adSdkListener?.OnAdBreakStarted();
        }

        public void OnAdStarted(VASTAd ad)
        {
            adSdkListener?.OnAdStarted(ad);

        }

        public void OnAdCompleted(VASTAd ad)
        {
            adSdkListener?.OnAdCompleted(ad);
        }

        public void OnAdBreakFinished()
        {
            adSdkListener?.OnAdBreakFinished();
        }

        public void OnAdLoading(VASTAd ad)
        {
            adSdkListener?.OnAdLoading(ad);
        }

        public void OnAdError(VASTAd ad, string error)
        {
            adSdkListener?.OnAdError(ad, error);
        }
    }
}
