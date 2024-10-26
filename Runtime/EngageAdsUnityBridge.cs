using UnityEngine;

namespace EMAds.Ads
{

    public class EngageAdsUnityBridge : MonoBehaviour
    {
        private AndroidJavaObject engageAdsPlugin;
        private IAdEventListener listener;

        void Start()
        {
            // only start if android
            if (Application.platform != RuntimePlatform.Android)
            {
                return;
            }

            using AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            engageAdsPlugin = new AndroidJavaObject("com.engage.unityvideoplugin.EMAdsPlugin");
        }

        public void Initialize(AdSdkConfig config, IAdEventListener listener)
        {
            this.listener = listener;
            string configJson = JsonUtility.ToJson(config);
            // get activity
            using AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            engageAdsPlugin ??= new AndroidJavaObject("com.engage.unityvideoplugin.EMAdsPlugin");
            engageAdsPlugin.Call("initialize", activity, configJson);
        }

        public void LoadAd()
        {
            if (RuntimePlatform.Android != Application.platform)
            {
                return;
            }
            engageAdsPlugin.Call("loadAd");
            listener.OnAdLoading(null);
        }

        public void ShowAd()
        {

            if (RuntimePlatform.Android != Application.platform)
            {
                return;
            }
            engageAdsPlugin.Call("showAd");

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
