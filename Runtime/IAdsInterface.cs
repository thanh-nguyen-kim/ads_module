
public interface IAdsInterface
{
    bool IsVideoRewardAdsReady();
    void LoadRewardedVideo();
    void ShowVideoAds(System.Action onUserEarnedReward, System.Action onAdClosed);
    bool IsInterstitialAdsReady();
    void LoadInterstitial();
    void ShowInterstitial();
}
