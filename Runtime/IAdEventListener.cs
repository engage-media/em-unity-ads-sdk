using EMAds.Ads;

namespace EMAds.Ads
{
    public interface IAdEventListener
    {
        void OnAdBreakStarted();
        void OnAdBreakFinished();
        void OnAdStarted(VASTAd ad);
        void OnAdCompleted(VASTAd ad);
        void OnAdLoading(VASTAd ad);
        void OnAdError(VASTAd ad, string error);
    }
}
