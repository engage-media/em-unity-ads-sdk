using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

namespace EMAds.Ads
{
    internal class AdBreakPlayer : MonoBehaviour
    {

        public VideoPlayer videoPlayer;
        private Queue<VASTAd> adQueue = new Queue<VASTAd>();
        private EMAdsSdkManager sdkManager;
        private VASTAd nextAd;
        private AudioSource audioSource;

        // Initialize with reference to the EMAdsSdkManager
        internal void Initialize(EMAdsSdkManager manager)
        {
            sdkManager = manager;
        }

        // Load ads and start playing
        internal void LoadAdBreak(List<VASTAd> ads)
        {
            Debug.Log("Loading ad break with " + ads.Count + " ads.");
            foreach (VASTAd ad in ads)
            {
                adQueue.Enqueue(ad);
            }
            if (sdkManager.adConfig.isAutoplay && GetCurrentAd() == null)
            {
                PlayNextAd();
            }
        }

        internal void PlayNextAd()
        {
            Debug.Log("Playing next ad...");
            if (nextAd == null)
            {
                sdkManager.OnAdBreakStarted();
            }
            if (adQueue.Count > 0)
            {
                this.nextAd = adQueue.Dequeue();
                sdkManager.OnAdLoading(nextAd);

                videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                    videoPlayer.SetTargetAudioSource(0, audioSource);
                    videoPlayer.timeUpdateMode = VideoTimeUpdateMode.DSPTime;
                }
                videoPlayer.url = nextAd.MediaFileUrl;
                videoPlayer.prepareCompleted += OnPrepareCompleted;
                videoPlayer.Prepare();
            }
            else
            {
                Debug.Log("Ad break finished.");
                nextAd = null;
                sdkManager.OnAdBreakFinished();
            }
        }

        private void OnPrepareCompleted(VideoPlayer vp)
        {
            Debug.Log("Ad prepared, starting to play.");
            vp.prepareCompleted -= OnPrepareCompleted;
            vp.Play();
            vp.loopPointReached += OnAdFinished;
            sdkManager.OnAdStarted(GetCurrentAd());
        }

        private void OnAdFinished(VideoPlayer vp)
        {
            Debug.Log("Ad finished playing.");
            vp.Stop();
            vp.loopPointReached -= OnAdFinished;
            sdkManager.OnAdCompleted(GetCurrentAd());
            PlayNextAd();
        }

        private VASTAd GetCurrentAd()
        {

            return nextAd;
        }
    }
}