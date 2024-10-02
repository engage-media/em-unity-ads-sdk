# Unity Ad SDK

This package provides a VAST-compliant video ad system for Unity, allowing developers to easily integrate video ads into their games or applications.

## Features
- Loads and plays VAST-compliant video ads.
- Supports test mode to load test ads.
- Handles recursive `VASTAdTagUri` to fetch nested ads up to 5 times.
- Provides a unified entry point (`AdSdkManager`) to manage ad lifecycle.
- Notifies developers of key ad events such as ad starts, completions, and errors.

## Installation

1. **Download and Add the Package**:
    - Clone or download the package from the repository and place it into the `Packages` folder of your Unity project.
    - Alternatively, you can add it as a package through Unity’s Package Manager.

2. **Add the Required Components**:
    - Add a `VideoPlayer` to your scene. This will be used to display ads.
    - Add a new `GameObject` and attach the `AdSdkManager` to it.

3. **Create Ad Configuration**:
    - Right-click in your project and create a new `AdSdkConfig` asset: **Create > AdSdk > Configuration**.
    - Fill in the fields with your ad configuration:
        - `Publisher ID`
        - `Channel ID`
        - `Base URL`
        - `Store URL`
        - `Package Name`
        - `Is Test Mode`: Toggle this flag to enable test ads.

## Usage

1. **Add `AdSdkManager`**:
    - Create a new GameObject in your scene and attach the `AdSdkManager` component to it.
    - Assign the `AdSdkConfig` and `VideoPlayer` in the Inspector.
  
2. **Implement `IAdEventListener`**:
    - Create a new script that implements the `IAdEventListener` interface to handle ad events.
    - For example:
  
    ```csharp
    public class GameAdManagerScript : MonoBehaviour, IAdEventListener
    {
        public AdSdkConfig adConfig;
        public VideoPlayer videoPlayer;
        private AdSdkManager adSdkManager;

        void Start()
        {
            adSdkManager = gameObject.AddComponent<AdSdkManager>();
            adSdkManager.Initialize(adConfig, videoPlayer, this); // Initialize the ad system
        }

        // Handle ad events
        public void OnAdBreakStarted(VASTAd ad) => Debug.Log("Ad break started.");
        public void OnAdStarted(VASTAd ad) => Debug.Log($"Ad started: {adUrl}");
        public void OnAdCompleted() => Debug.Log("Ad completed.");
        public void OnAdBreakFinished() => Debug.Log("Ad break finished.");
        public void OnAdLoading(VASTAd ad) => Debug.Log($"Ad loading: {adUrl}");
        public void OnAdError(VASTAd ad, string error) => Debug.LogError($"Ad error: {error}");
    }
    ```

3. **Test Mode**:
    - Set the `isTestMode` flag in `AdSdkConfig` to `true` to use the test host for testing ads.

## API Reference

### AdSdkManager
This is the main entry point for the ad system. It provides the following methods:

- **Initialize(AdSdkConfig config, VideoPlayer videoView, IAdEventListener listener)**:
    Initializes the ad system with the provided configuration, `VideoPlayer`, and listener for event handling.

### IAdEventListener
This interface must be implemented to handle ad lifecycle events:

- `OnAdBreakStarted(VASTAd ad)`: Called when the ad break starts.
- `OnAdStarted(VASTAd ad)`: Called when an individual ad starts playing.
- `OnAdCompleted(VASTAd ad)`: Called when an individual ad completes.
- `OnAdBreakFinished()`: Called when all ads in the ad break are completed.
- `OnAdLoading(VASTAd ad)`: Called when an ad is loading.
- `OnAdError(VASTAd ad, string error)`: Called when an error occurs during ad loading or playback.

## Error Handling

If an error occurs (such as failing to load ads or reaching the maximum recursion depth for `VASTAdTagUri`), the `OnAdError` method will be called with a description of the error.

## Limitations
- A maximum of 5 recursive calls to `VASTAdTagUri` are supported.
- The system is designed to work with VAST-compliant ad servers.

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request with your improvements or bug fixes.

## License

This package is licensed under the [MIT License](LICENSE).
