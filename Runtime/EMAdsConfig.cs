using UnityEngine;

namespace EMAds.Ads
{
    [CreateAssetMenu(fileName = "AdSdkConfig", menuName = "AdSdk/Configuration")]
    public class AdSdkConfig : ScriptableObject
    {
        [SerializeField] private bool _isTestMode;
        [SerializeField] private string _publisherId;
        [SerializeField] private string _channelId;
        [SerializeField] private string _storeUrl;
        [SerializeField] private string _bundleId;
        [SerializeField] private bool _isAutoplay;

        // Implementing the interface properties
        public string publisherId { get => _publisherId; set => _publisherId = value; }
        public string channelId { get => _channelId; set => _channelId = value; }
        public string storeUrl { get => _storeUrl; set => _storeUrl = value; }
        public string bundleId { get => _bundleId; set => _bundleId = value; }
        public bool isTestMode { get => _isTestMode; set => _isTestMode = value; }
        public bool isAutoplay { get => _isAutoplay; set => _isAutoplay = value; }
    }

}