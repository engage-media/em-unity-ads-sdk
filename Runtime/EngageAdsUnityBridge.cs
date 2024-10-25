using UnityEngine;

namespace EMAds.Ads
{

    public class EngageAdsUnityBridge : MonoBehaviour
    {
        private AndroidJavaObject engageAdsPlugin;
        private IAdEventListener listener;

        void Start()
        {
#if UNITY_ANDROID
            using AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            engageAdsPlugin = new AndroidJavaObject("com.example.engageadsplugin.EMAdsPlugin");
#endif
        }

        public void Initialize(AdSdkConfig config, IAdEventListener listener)
        {
            this.listener = listener;
            string configJson = JsonUtility.ToJson(config);
            // get activity
            using AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            engageAdsPlugin.Call("initialize", activity, configJson);
        }

        public void LoadAd()
        {
#if UNITY_ANDROID
            engageAdsPlugin.Call("loadAd");
            listener.OnAdLoading(null);
#endif
        }

        public void ShowAd()
        {
#if UNITY_ANDROID
            engageAdsPlugin.Call("showAd");
#endif
        }

        // Called from Android plugin
        public void OnPauseContent()
        {
            Debug.Log("AdManager: Pausing content");
            // Pause your Unity game content
            listener.OnAdBreakStarted();
        }

        public void OnResumeContent()
        {
            Debug.Log("AdManager: Resuming content");
            // Resume your Unity game content
            listener.OnAdBreakFinished();
        }

        public void OnAdEnded()
        {
            Debug.Log("AdManager: Ad ended");
            // Take actions after the ad ends, e.g., resume gameplay
            listener.OnAdCompleted(null);
        }

        public void OnAdStarted()
        {
            Debug.Log("AdManager: Ad started");
            // Actions to take when ad starts
            listener.OnAdStarted(null);
        }
    }

}