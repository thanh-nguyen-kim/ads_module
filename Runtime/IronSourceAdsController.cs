using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class IronSourceAdsController : MonoBehaviour, IAdsInterface
{
    private string appKey;
    private System.Action onUserEarnedReward, onAdClosed;
    public bool IsInterstitialAdsReady()
    {
        return false;
        // return IronSource.Agent.isInterstitialReady();
    }
    public bool IsVideoRewardAdsReady()
    {
        return false;
        // return IronSource.Agent.isRewardedVideoAvailable();
    }
    public void LoadInterstitial()
    {
        // IronSource.Agent.loadInterstitial();
    }
    public void LoadRewardedVideo()
    {
        throw new NotImplementedException();
    }
    public void ShowInterstitial()
    {
        // IronSource.Agent.showInterstitial();
    }
    public void ShowVideoAds(Action onUserEarnedReward, Action onAdClosed)
    {
        // this.onUserEarnedReward = onUserEarnedReward;
        // this.onAdClosed = onAdClosed;
        // IronSource.Agent.showRewardedVideo();
    }
    //Invoked when the RewardedVideo ad view has opened.
    //Your Activity will lose focus. Please avoid performing heavy 
    //tasks till the video ad will be closed.
    void RewardedVideoAdOpenedEvent()
    {
    }
    //Invoked when the RewardedVideo ad view has clicked.
    // void RewardedVideoAdClickedEvent(IronSourcePlacement placement)
    // {
    // }
    //Invoked when the RewardedVideo ad view is about to be closed.
    //Your activity will now regain its focus.
    void RewardedVideoAdClosedEvent()
    {
        // Debug.Log("Close ads: " + System.DateTime.UtcNow.ToLongTimeString());
    }
    //Invoked when there is a change in the ad availability status.
    //@param - available - value will change to true when rewarded videos are available. 
    //You can then show the video by calling showRewardedVideo().
    //Value will change to false when no videos are available.
    void RewardedVideoAvailabilityChangedEvent(bool available)
    {
        //Change the in-app 'Traffic Driver' state according to availability.
        // isVideoLoaded = available;
    }

    //Invoked when the user completed the video and should be rewarded. 
    //If using server-to-server callbacks you may ignore this events and wait for 
    // the callback from the  ironSource server.
    //@param - placement - placement object which contains the reward data
    // void RewardedVideoAdRewardedEvent(IronSourcePlacement placement)
    // {
    //     // Debug.Log("Reward player: " + System.DateTime.UtcNow.ToLongTimeString());
    //     if (onUserEarnedReward != null)
    //         onUserEarnedReward.Invoke();
    // }
    //Invoked when the Rewarded Video failed to show
    //@param description - string - contains information about the failure.
    // void RewardedVideoAdShowFailedEvent(IronSourceError error)
    // {
    // }

    // ----------------------------------------------------------------------------------------
    // Note: the events below are not available for all supported rewarded video ad networks. 
    // Check which events are available per ad network you choose to include in your build. 
    // We recommend only using events which register to ALL ad networks you include in your build. 
    // ----------------------------------------------------------------------------------------

    //Invoked when the video ad starts playing. 
    void RewardedVideoAdStartedEvent()
    {
    }
    //Invoked when the video ad finishes playing. 
    void RewardedVideoAdEndedEvent()
    {
    }
    //Invoked when the initialization process has failed.
    //@param description - string - contains information about the failure.
    // void InterstitialAdLoadFailedEvent(IronSourceError error)
    // {
    // }
    //Invoked right before the Interstitial screen is about to open.
    void InterstitialAdShowSucceededEvent()
    {
    }
    //Invoked when the ad fails to show.
    //@param description - string - contains information about the failure.
    // void InterstitialAdShowFailedEvent(IronSourceError error)
    // {
    // }
    // Invoked when end user clicked on the interstitial ad
    void InterstitialAdClickedEvent()
    {
    }
    //Invoked when the interstitial ad closed and the user goes back to the application screen.
    void InterstitialAdClosedEvent()
    {
    }
    //Invoked when the Interstitial is Ready to shown after load function is called
    void InterstitialAdReadyEvent()
    {
    }
    //Invoked when the Interstitial Ad Unit has opened
    void InterstitialAdOpenedEvent()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID
        appKey = "";
#elif UNITY_IOS
       appKey= "";
#endif
        // IronSource.Agent.init(appKey);
        // #region video reward
        // IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
        // IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
        // IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
        // IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
        // IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
        // IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
        // IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
        // IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
        // #endregion
        // #region interstitial
        // IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
        // IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
        // IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
        // IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
        // IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
        // IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
        // IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
        // #endregion
        // IronSource.Agent.validateIntegration();
        // IronSource.Agent.setAdaptersDebug(true);
    }
    void OnDestroy()
    {
        // #region video reward
        // IronSourceEvents.onRewardedVideoAdOpenedEvent -= RewardedVideoAdOpenedEvent;
        // IronSourceEvents.onRewardedVideoAdClickedEvent -= RewardedVideoAdClickedEvent;
        // IronSourceEvents.onRewardedVideoAdClosedEvent -= RewardedVideoAdClosedEvent;
        // IronSourceEvents.onRewardedVideoAvailabilityChangedEvent -= RewardedVideoAvailabilityChangedEvent;
        // IronSourceEvents.onRewardedVideoAdStartedEvent -= RewardedVideoAdStartedEvent;
        // IronSourceEvents.onRewardedVideoAdEndedEvent -= RewardedVideoAdEndedEvent;
        // IronSourceEvents.onRewardedVideoAdRewardedEvent -= RewardedVideoAdRewardedEvent;
        // IronSourceEvents.onRewardedVideoAdShowFailedEvent -= RewardedVideoAdShowFailedEvent;
        // #endregion
        // #region interstitial
        // IronSourceEvents.onInterstitialAdReadyEvent -= InterstitialAdReadyEvent;
        // IronSourceEvents.onInterstitialAdLoadFailedEvent -= InterstitialAdLoadFailedEvent;
        // IronSourceEvents.onInterstitialAdShowSucceededEvent -= InterstitialAdShowSucceededEvent;
        // IronSourceEvents.onInterstitialAdShowFailedEvent -= InterstitialAdShowFailedEvent;
        // IronSourceEvents.onInterstitialAdClickedEvent -= InterstitialAdClickedEvent;
        // IronSourceEvents.onInterstitialAdOpenedEvent -= InterstitialAdOpenedEvent;
        // IronSourceEvents.onInterstitialAdClosedEvent -= InterstitialAdClosedEvent;
        // #endregion
    }
    void OnApplicationPause(bool isPaused)
    {
        //IronSource.Agent.onApplicationPause(isPaused);
    }
    private float timeStamp = 0;
#if !UNITY_EDITOR
    private void Update()
    {
        if (Time.time - timeStamp > 15)
        {
            timeStamp = Time.time;
            if (!IsInterstitialAdsReady())
                LoadInterstitial();
        }
    }
#endif
}
