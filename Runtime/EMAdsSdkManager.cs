using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EMAdsSdkManager : MonoBehaviour, IAdEventListener
{
    private AdBreakPlayer adBreakPlayer;
    internal AdSdkConfig adConfig;
    private IAdEventListener adSdkListener;
    private VideoPlayer videoPlayer;

    // Entry point to initialize the SDK with config, video player, and listener
    public void Initialize(AdSdkConfig config, VideoPlayer videoView, IAdEventListener listener)
    {
        this.adConfig = config;
        this.videoPlayer = videoView;
        this.adSdkListener = listener;

        // Create and initialize AdBreakPlayer
        adBreakPlayer = gameObject.AddComponent<AdBreakPlayer>();
        adBreakPlayer.Initialize(this);
        adBreakPlayer.videoPlayer = videoPlayer;

        // Start loading ads
        LoadAds();
    }

    public void PlayNextAd() => adBreakPlayer.PlayNextAd();

    private void LoadAds()
    {
        ContentToAdAdapter adAdapter = gameObject.AddComponent<ContentToAdAdapter>();
        adAdapter.adEventListener = this;
        adAdapter.adBreakPlayer = adBreakPlayer;
        adAdapter.adConfig = adConfig;

        adAdapter.LoadAds();
    }

    // Methods to handle ad events and notify the user
    public void OnAdBreakStarted() => adSdkListener?.OnAdBreakStarted();
    public void OnAdStarted(VASTAd ad) => adSdkListener?.OnAdStarted(ad);
    public void OnAdCompleted(VASTAd ad) => adSdkListener?.OnAdCompleted(ad);
    public void OnAdBreakFinished() => adSdkListener?.OnAdBreakFinished();
    public void OnAdLoading(VASTAd ad) => adSdkListener?.OnAdLoading(ad);
    public void OnAdError(VASTAd ad, string error) => adSdkListener?.OnAdError(ad, error);
}