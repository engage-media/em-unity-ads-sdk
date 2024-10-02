using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace EMAds.Ads
{
    internal class ContentToAdAdapter : MonoBehaviour
    {
        public IAdEventListener adEventListener;
        public AdBreakPlayer adBreakPlayer;
        public AdSdkConfig adConfig;

        private static readonly HttpClient client = new HttpClient();

        // Load ads from the server using the configuration
        internal async void LoadAds(int retryCount = 0, string vastTagUri = null)
        {
            // Check if we have reached the maximum number of retries
            if (retryCount > 5)
            {
                Debug.LogError("Exceeded maximum number of VAST ad tag URI loads.");
                adEventListener?.OnAdError(null, "Exceeded maximum VAST tag recursion limit.");
                return;
            }

            // Fetch ads: either from the given VASTTagUri or from the standard configuration
            Task<string> adsTask = vastTagUri != null ? GetAdsAsync(vastTagUri) : GetAdsAsync(adConfig.publisherId, adConfig.channelId, adConfig.storeUrl, adConfig.packageName);

            string result = await adsTask;
            if (result == null)
            {
                Debug.LogError("Failed to load ads.");
                adEventListener?.OnAdError(null, "Failed to load ads.");
                return;
            }

            VASTParser parser = new VASTParser();
            List<VASTAd> ads = parser.ParseVASTResponse(result);

            // Check if we have a valid media file URL
            if (ads.Count > 0 && ads.Any(ad => !string.IsNullOrEmpty(ad.MediaFileUrl)))
            {
                // We found an ad with a valid media file, load the ad break
                adBreakPlayer.LoadAdBreak(ads.Where(ad => !string.IsNullOrEmpty(ad.MediaFileUrl)).ToList());
            }
            else if (ads.Count > 0 && ads.Any(ad => !string.IsNullOrEmpty(ad.VastAdTagUrl)))
            {
                // We found a VASTAdTagUri, recursively load the ad from that URI
                Debug.Log("VASTAdTagUri found. Recursively loading the next ad...");
                string nextVastTagUri = ads.First(ad => !string.IsNullOrEmpty(ad.VastAdTagUrl)).VastAdTagUrl;
                LoadAds(retryCount + 1, nextVastTagUri);
            }
            else
            {
                // No valid ads or VASTAdTagUri were found, return an error
                Debug.LogWarning("No valid ads found in the response.");
                adEventListener?.OnAdError(null, "No valid ads found in the response.");
            }
        }

        // Fetch ads via HTTP using a VAST URI
        private async Task<string> GetAdsAsync(string vastTagUri)
        {
            try
            {
                VastAdRequestDto dto = new VastAdRequestDto();
                dto.app = new VastAdRequestDto.AppInfo
                {
                    channelId = adConfig.channelId,
                    publisherId = adConfig.publisherId,
                    storeUrl = adConfig.storeUrl,
                    bundle = adConfig.packageName,
                    name = Application.productName
                };
                dto.imp = new VastAdRequestDto.Imp[] {
                new VastAdRequestDto.Imp{
                    secure = true,
                    video = new VastAdRequestDto.Video
                    {
                        w = Screen.width,
                        h = Screen.height
                    }
                }};
                dto.regs = new VastAdRequestDto.Regs
                {
                    gdpr = 0
                };
                dto.device = new VastAdRequestDto.DeviceInfo
                {
                    os = SystemInfo.operatingSystem,
                    ifa = SystemInfo.deviceUniqueIdentifier,
                    ext = new VastAdRequestDto.DeviceExt
                    {
                        ifaType = "IDFA"
                    },
                    model = SystemInfo.deviceModel,
                    js = true,
                    devicetype = 2,
                    ip = ""
                };

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, VASTRequestBuilder.BuildUrl(vastTagUri, dto));
                HttpResponseMessage response = await client.GetAsync(request.RequestUri);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Request error: {e.Message}");
                return null;
            }
        }

        // Fetch ads via HTTP using publisher, channel, store, and package information
        private async Task<string> GetAdsAsync(string publisherId, string channelId, string storeUrl, string packageName)
        {
            string baseUrl = adConfig.isTestMode ? "https://s.adtelligent.com/demo" : "https://vast.engagemedia.com";

            string url = $"{baseUrl}?publisher={publisherId}&channel={channelId}&publisherId={publisherId}&channelId={channelId}&storeUrl={storeUrl}&appBundle={packageName}&country={RegionInfo.CurrentRegion.TwoLetterISORegionName}";
            return await GetAdsAsync(url);
        }
    }

}