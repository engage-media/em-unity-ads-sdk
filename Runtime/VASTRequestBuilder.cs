using System;
using System.Globalization;
using UnityEngine;

namespace EMAds.Ads
{

    public class VASTRequestBuilder
    {
        public static Uri BuildUrl(string adTagUrl, VastAdRequestDto requestBody)
        {
            // Device Information
            string deviceId = SystemInfo.deviceUniqueIdentifier; // Device ID from Unity
            string userAgent = GetUserAgent(); // Custom method to build user agent string
            string countryCode = GetCountryCode(); // Replace this with your own logic to retrieve the country code

            UriBuilder uriBuilder = new UriBuilder(adTagUrl);
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            // App information
            query["device_id"] = deviceId;
            query["ua"] = userAgent;
            query["appName"] = requestBody.app.name;
            query["appBundle"] = requestBody.app.bundle;
            query["appURL"] = requestBody.app.storeUrl;

            // Screen dimensions
            query["width"] = Screen.width.ToString();
            query["height"] = ((Screen.height * 9) / 16).ToString();  // Aspect ratio adjustment

            // GDPR and Privacy-related
            query["us_privacy"] = requestBody.regs.gdpr == 1 ? "1" : "0";
            query["idfa"] = deviceId;  // Unity doesn’t provide IDFA by default, so using deviceId as a placeholder
            query["adid"] = deviceId;  // Same as above

            // Country code if available
            if (!string.IsNullOrEmpty(countryCode))
            {
                query["country"] = countryCode;
            }

            // Do not track / limited tracking
            query["dnt"] = "0";  // Replace with your logic to get Do Not Track
            query["lmt"] = "0";  // Replace with your logic for Limited Ad Tracking

            // Device-specific details
            query["os"] = requestBody.device.os; // Device OS
            query["ifa"] = requestBody.device.ifa; // ID for Advertising (custom)
            query["ifa_type"] = requestBody.device.ext.ifaType;  // Custom IFA Type
            query["model"] = SystemInfo.deviceModel;  // Device model from Unity
            query["js"] = requestBody.device.js.ToString();  // JS Enabled or not (custom)
            query["devicetype"] = requestBody.device.devicetype.ToString(); // Device type

            // If IP is needed and available
            // if (!string.IsNullOrEmpty(requestBody.device.ip))
            // {
            //     query["ip"] = requestBody.device.ip;
            // }

            // Secure connection
            query["secure"] = requestBody.imp[0].secure.ToString();
            query["vast_version"] = "3.0"; // Fixed VAST version
            query["channel"] = requestBody.app.channelId;  // Custom channel ID
            query["publisher"] = requestBody.app.publisherId;  // Custom publisher ID

            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri;
        }

        // Mock function to generate user-agent string
        private static string GetUserAgent()
        {
            return $"UnityPlayer/{Application.unityVersion} ({SystemInfo.operatingSystem}; {SystemInfo.deviceModel.Replace("KFRAWI", "FireTV")})";
        }

        // Mock function to get country code (you need to provide your own logic here)
        private static string GetCountryCode()
        {

            RegionInfo region = RegionInfo.CurrentRegion;
            return region.TwoLetterISORegionName ?? "US"; // Returns the ISO 3166-1 alpha-2 country code
        }
    }

    // DTO classes to mock your request structure
    public class VastAdRequestDto
    {
        public AppInfo app;
        public Imp[] imp;
        public Regs regs;
        public DeviceInfo device;

        public class AppInfo
        {
            public string name;
            public string bundle;
            public string storeUrl;
            public string channelId;
            public string publisherId;
        }

        public class Imp
        {
            public Video video;
            public bool secure;
        }

        public class Video
        {
            public int w;
            public int h;
        }

        public class Regs
        {
            public int gdpr;
        }

        public class DeviceInfo
        {
            public string os;
            public string ifa;
            public DeviceExt ext;
            public string model;
            public bool js;
            public int devicetype;
            public string ip;
        }

        public class DeviceExt
        {
            public string ifaType;
        }
    }

}