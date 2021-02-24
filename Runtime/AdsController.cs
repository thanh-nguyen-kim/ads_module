using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
public class AdsController : MonoBehaviour
{
    private static AdsController instance;
    public static AdsController Instance { get { return instance; } }
    private AdsSetting[] adsSettings;
    private AdsFullCondition adsFullCondition;
    private AdsDataCollection adsDataCollection;
    private string dataPath = "";
    private List<IAdsInterface> adsController;
    private int adsFullConditionWinCount, adsFullConditionFailCount, adsFullConditionPauseCount, adsFullConditionGiveUpCount;
    private float adsFullTimestamp;
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            dataPath = Path.Combine(Application.persistentDataPath, "ads_data.dat");
            adsController = new List<IAdsInterface>();
            for (int i = 0; i < transform.childCount; i++)
                if (transform.GetChild(i).GetComponent<IAdsInterface>() != null)
                    adsController.Add(transform.GetChild(i).GetComponent<IAdsInterface>());
            LoadAdsData();
        }
        else
            Destroy(gameObject);
    }
    public void LoadAdsData()
    {
        adsSettings = new AdsSetting[1] { new AdsSetting() };//We should load ads setting from a json file
        adsFullCondition = new AdsFullCondition();//We should load ads setting from a json file
        if (File.Exists(dataPath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            string data;
            using (FileStream fileStream = File.Open(dataPath, FileMode.Open))
            {
                try
                {
                    data = (string)binaryFormatter.Deserialize(fileStream);
                    adsDataCollection = JsonUtility.FromJson<AdsDataCollection>(data);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.Message);
                    ResetData();
                }
            }
        }
        else
            ResetData();
        adsDataCollection.UpdateAdsCollection(adsSettings);
    }
    public void SaveAdsData()
    {
        string origin = JsonUtility.ToJson(adsDataCollection);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (FileStream fileStream = File.Open(dataPath, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, origin);
        }
    }
    private void ResetData()
    {
        adsDataCollection = new AdsDataCollection(adsSettings);
        SaveAdsData();
    }
    public bool CanShowAds(string adsName)
    {
        return adsDataCollection.CanShowAds(adsName);
    }
    public void OnWatchAdsCompleted(string adsName)
    {
        adsDataCollection.OnWatchAdsCompleted(adsName);
        SaveAdsData();
        //Firebase.Analytics.FirebaseAnalytics.LogEvent("ads_reward", "source", adsName);//report ads event to firebase
    }
    public void ResetAdsCount(string adsName)
    {
        adsDataCollection.ResetAdsCount(adsName);
    }
    public int GetAdsCooldown(string adsName)
    {
        return adsDataCollection.GetAdsCooldown(adsName);
    }
    public int GetAdsCooldownSecond(string adsName)
    {
        return adsDataCollection.GetAdsCooldownSecond(adsName);
    }
    public AdsSetting GetAdsSettingByName(string name)
    {
        AdsSetting result = new AdsSetting();
        foreach (var adsSetting in adsSettings)
            if (adsSetting.adsName == name)
                result = adsSetting;
        return result;
    }
    public bool IsRewardVideoAdsReady()
    {
        foreach (var ads in adsController)
            if (ads.IsVideoRewardAdsReady())
                return true;
#if UNITY_EDITOR
        return true;
#endif
        return false;
    }
    public void ShowVideoReward(System.Action onUserEarnedReward, System.Action onAdClosed)
    {
        foreach (var ads in adsController)
        {
            if (ads.IsVideoRewardAdsReady())
            {
                ads.ShowVideoAds(onUserEarnedReward, onAdClosed);
                return;
            }
        }
#if UNITY_EDITOR
        onUserEarnedReward?.Invoke();
#endif
    }
    public void ShowInterstitial(int type)
    {
        // int currentChapter = 1, currentLevel = 1;//hard code value
        // if (currentChapter < adsFullCondition.chapter || (currentChapter == adsFullCondition.chapter && currentLevel < adsFullCondition.level)) return;
        // if (type == 1 && adsFullConditionWinCount < adsFullCondition.winThreshold)
        // {
        //     adsFullConditionWinCount++;
        //     return;
        // }
        // else if (type == 2 && adsFullConditionFailCount < adsFullCondition.failThreshold)
        // {
        //     adsFullConditionFailCount++;
        //     return;
        // }
        // else if (type == 0 && adsFullConditionPauseCount < adsFullCondition.pauseThreshold)
        // {
        //     adsFullConditionPauseCount++;
        //     return;
        // }
        // else if (type == 3 && adsFullConditionGiveUpCount < adsFullCondition.giveUpThreshold)
        // {
        //     adsFullConditionGiveUpCount++;
        //     return;
        // }
        if (Time.time - adsFullTimestamp > adsFullCondition.adsCooldown)
        {
            foreach (var ads in adsController)
                if (ads.IsInterstitialAdsReady())
                {
                    adsFullConditionWinCount = adsFullConditionPauseCount = adsFullConditionGiveUpCount = adsFullConditionFailCount = 0;
                    ads.ShowInterstitial();
                    break;
                }
                else
                    Debug.Log("No Interestitial Ads to show");
            adsFullTimestamp = Time.time;
        }
    }
    public void TestShowVideoReward()
    {
        ShowVideoReward(null, null);
    }
}
[Serializable]
public class AdsFullCondition
{
    public int chapter = 1, level = 13;
    public int pauseThreshold = 5, failThreshold = 5, winThreshold = 3, giveUpThreshold = 1;
    public int adsCooldown;
}
[Serializable]
public class AdsSettings
{
    public AdsSetting[] adsSettings;
}
[Serializable]
public class AdsSetting
{
    public string adsName = "default";
    public int adsQuota = 1;
    public int adsCooldown = 1800;//cooldown in seconds
}
[Serializable]
public class AdsData
{
    public string adsName = "";
    public int adsCount = 0;
    public double adsTimeStamp = 0;//store timestamp when last time open ads
    public AdsData(string name)
    {
        adsName = name;
        adsCount = 0;
        adsTimeStamp = 0;
    }
    public bool CanShowAds()
    {
        bool result = false;
        AdsSetting adsSetting = AdsController.Instance.GetAdsSettingByName(adsName);
        var tmpTimestamp = Utils.ConvertToUnixTime(DateTime.UtcNow);
        if (tmpTimestamp - adsTimeStamp > 64800)
            adsCount = 0;//reset ads count after 24h cooldown
        if (adsCount < adsSetting.adsQuota && tmpTimestamp - adsTimeStamp > adsSetting.adsCooldown)
            result = true;
        return result;
    }
    public void OnShowAds()
    {
        adsCount++;
        adsTimeStamp = Utils.ConvertToUnixTime(DateTime.UtcNow);
    }
    public int GetAdsCooldown()
    {
        AdsSetting adsSetting = AdsController.Instance.GetAdsSettingByName(adsName);
        var remainTimeInSecond = adsTimeStamp + adsSetting.adsCooldown - Utils.ConvertToUnixTime(DateTime.UtcNow);
        return Mathf.Max(1, (int)(remainTimeInSecond / 60));
    }
    public int GetAdsCooldownSecond()
    {
        AdsSetting adsSetting = AdsController.Instance.GetAdsSettingByName(adsName);
        var remainTimeInSecond = adsTimeStamp + adsSetting.adsCooldown - Utils.ConvertToUnixTime(DateTime.UtcNow);
        return Mathf.Max(1, (int)remainTimeInSecond);
    }
}
[Serializable]
public class AdsDataCollection
{
    public AdsData[] adsCollection;
    public AdsDataCollection(AdsSetting[] settings)
    {
        List<AdsData> tmp = new List<AdsData>();
        foreach (var setting in settings)
            tmp.Add(new AdsData(setting.adsName));
        adsCollection = tmp.ToArray();
    }
    public void UpdateAdsCollection(AdsSetting[] settings)
    {
        List<AdsData> tmpAdsData = new List<AdsData>();
        foreach (var setting in settings)
            tmpAdsData.Add(new AdsData(setting.adsName));
        if (tmpAdsData.Count > adsCollection.Length)
        {
            for (int i = 0; i < tmpAdsData.Count; i++)//update machine state
            {
                for (int j = 0; j < adsCollection.Length; j++)
                {
                    if (tmpAdsData[i].adsName == adsCollection[j].adsName)
                        tmpAdsData[i] = adsCollection[j];
                }
            }
            adsCollection = tmpAdsData.ToArray();
        }
    }
    public bool CanShowAds(string adsName)
    {
        AdsData ads = GetAdsDataByName(adsName);
        return ads.CanShowAds();
    }
    public void OnWatchAdsCompleted(string adsName)
    {
        AdsData ads = GetAdsDataByName(adsName);
        ads.OnShowAds();
    }
    public void ResetAdsCount(string adsName)
    {
        AdsData ads = GetAdsDataByName(adsName);
        ads.adsCount = 0;
    }
    public int GetAdsCooldown(string adsName)
    {
        AdsData ads = GetAdsDataByName(adsName);
        return ads.GetAdsCooldown();
    }
    public int GetAdsCooldownSecond(string adsName)
    {
        AdsData ads = GetAdsDataByName(adsName);
        return ads.GetAdsCooldownSecond();
    }
    private AdsData GetAdsDataByName(string name)
    {
        AdsData result = new AdsData("default");
        foreach (var ads in adsCollection)
            if (ads.adsName == name) result = ads;
        return result;
    }
}