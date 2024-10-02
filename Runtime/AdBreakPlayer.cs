using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

internal class AdBreakPlayer : MonoBehaviour
{

    public VideoPlayer videoPlayer;
    private Queue<VASTAd> adQueue = new Queue<VASTAd>();
    private EMAdsSdkManager sdkManager;
    private VASTAd nextAd;

    // Initialize with reference to the AdSdkManager
    internal void Initialize(EMAdsSdkManager manager)
    {
        sdkManager = manager;
    }

    // Load ads and start playing
    internal void LoadAdBreak(List<VASTAd> ads)
    {
        foreach (VASTAd ad in ads)
        {
            adQueue.Enqueue(ad);
        }
        if (sdkManager.adConfig.isAutoplay)
        {
            PlayNextAd();
        }
    }

    internal void PlayNextAd()
    {
        if (adQueue.Count > 0)
        {
            sdkManager.OnAdBreakStarted();
            this.nextAd = adQueue.Dequeue();
            sdkManager.OnAdLoading(nextAd);

            videoPlayer.url = nextAd.MediaFileUrl;
            videoPlayer.prepareCompleted += OnPrepareCompleted;
            videoPlayer.Prepare();
        }
        else
        {
            sdkManager.OnAdBreakFinished();
            nextAd = null;
        }
    }

    private void OnPrepareCompleted(VideoPlayer vp)
    {
        sdkManager.OnAdStarted(GetCurrentAd());
        vp.Play();
        vp.loopPointReached += OnAdFinished;
    }

    private void OnAdFinished(VideoPlayer vp)
    {
        sdkManager.OnAdCompleted(GetCurrentAd());
        vp.loopPointReached -= OnAdFinished;
        nextAd = null;
        PlayNextAd();
    }

    private VASTAd GetCurrentAd()
    {

        return nextAd;
    }
}