# Unity Ad SDK

This package provides a VAST-compliant video ad system for Unity, allowing developers to easily integrate video ads into their games or applications.

## Features
- Loads and plays VAST-compliant video ads.
- Supports test mode to load test ads.
- Handles recursive `VASTAdTagUri` to fetch nested ads up to 5 times.
- Provides a unified entry point (`EMAdsSdkManager`) to manage ad lifecycle.
- Notifies developers of key ad events such as ad starts, completions, and errors.

## Installation

### Requirements

1. **Kotlin Version**: The project requires **Kotlin 1.9.0 or later**.
2. **Java Version**: The project requires **Java 17**.
3. **Gradle Plugin**: Ensure that the Gradle plugin in your project supports Kotlin 1.9.0 and Java 17.

### Installation Steps

To install the package, follow these steps:

1. Open your project's `manifest.json` file located in the `/Packages` folder.
2. Add the following line to the `dependencies` section:

    ```json
    {
      "dependencies": {
        "com.engage-media.ads-sdk": "https://github.com/engage-media/em-unity-ads-sdk.git#main"
      }
    }
    ```

3. Save the `manifest.json` file. Unity will automatically fetch and install the package from the specified GitHub repository.

4. **Create Ad Configuration**:
    - Right-click in your project and create a new configuration asset via: **Create > AdSdk > Configuration**.
    - This creates a scriptable object file named `EMAdSdkConfig`.
    - **Note:** You will see a warning "No script asset for AdSdkConfig" - this is expected and won't affect functionality. The warning appears because the class name (`EMAdSdkConfig`) doesn't match the file name (`EMAdsConfig.cs`).
    - Fill in the required fields:
        - `Publisher ID`
        - `Channel ID`
        - `Store URL`
        - `Bundle ID`
        - `Is Test Mode`: Toggle this flag to enable test ads.
        - `Is Autoplay`: Toggle this flag to enable automatic playback.

## Usage

1. **Add `EMAdsSdkManager`**:
    - Create a new GameObject in your scene.
    - Add the `EMAdsSdkManager` component to your GameObject.
    - Assign the created `EMAdSdkConfig` asset and a `VideoPlayer` component in the Inspector.

2. **Implement `IAdEventListener`**:
    [Keep existing IAdEventListener example, but update class name]
    ```csharp
    public class GameAdManagerScript : MonoBehaviour, IAdEventListener
    {
        public EMAdSdkConfig emAdSdkConfig;
        public VideoPlayer videoPlayer;
        private EMAdsSdkManager emAdSdkManager; 

        void Start()
        {
            emAdSdkManager = gameObject.AddComponent<EMAdsSdkManager>();
            emAdSdkManager.Initialize(emAdSdkConfig, videoPlayer, this);
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
    - Set the `isTestMode` flag in `EMAdSdkConfig` to `true` to use the test host for testing ads.

## API Reference

### EMAdsSdkManager
This is the main entry point for the ad system. It provides the following methods:

- **Initialize(EMAdSdkConfig config, VideoPlayer videoView, IAdEventListener listener)**:
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

> **Important**: There's a class name mismatch in the SDK. The configuration class is named `EMAdSdkConfig` but appears as "AdSdkConfig" in the Unity Editor. This is a known issue but won't affect functionality.

## Limitations
- A maximum of 5 recursive calls to `VASTAdTagUri` are supported.
- The system is designed to work with VAST-compliant ad servers.

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request with your improvements or bug fixes.

## License

This package is licensed under the [MIT License](LICENSE).
