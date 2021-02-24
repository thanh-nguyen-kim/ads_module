# Ads Module

<https://killertee.wordpress.com/about/>

A collection of scripts to handle Unity Ads Implement.

+ AdsController.cs is the wraper of all ads network.
+ IronSourceAdsController.cs is the wrapper of Iron Source Plugin.
+ CSCAdsController.cs is the inhouse ads. It handle load, display CSC inhouse ads.

## Set up AdsController

+ Next, Set up your first scene similar with the example scene.
+ Implemented ads network must be child of AdsController in hierachy.
+ Priority of an Ads network is determined by its child index.
+ The function in the scripts is name-explained.

## Set up IronSourceAdsController.cs

+ Each game have an unique "appKey". You have to change it inside the script.
+ Implement Ironsource callback and IAdsInterface inside the script after import IronSource Plugin

## Set up CSCInterstitialAds.cs

+ Each game have an unique "appKey". You have to change it in the inspector view.

## Set up the ads canvas

+ Take a look at 4 ads prefabs and modify it to fit your use case