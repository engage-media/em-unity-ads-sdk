using UnityEngine;

[CreateAssetMenu(fileName = "AdSdkConfig", menuName = "AdSdk/Configuration")]
public class AdSdkConfig : ScriptableObject
{
    [SerializeField] private bool _isTestMode;
    [SerializeField] private string _publisherId;
    [SerializeField] private string _channelId;
    [SerializeField] private string _baseUrl;
    [SerializeField] private string _storeUrl;
    [SerializeField] private string _packageName;
    [SerializeField] private bool _isAutoplay;

    // Implementing the interface properties
    public string publisherId { get => _publisherId; set => _publisherId = value; }
    public string channelId { get => _channelId; set => _channelId = value; }
    public string baseUrl { get => _baseUrl; set => _baseUrl = value; }
    public string storeUrl { get => _storeUrl; set => _storeUrl = value; }
    public string packageName { get => _packageName; set => _packageName = value; }

    public bool isTestMode { get => _isTestMode; set => _isTestMode = value; }

    public bool isAutoplay { get => _isAutoplay; set => _isAutoplay = value; }
}
