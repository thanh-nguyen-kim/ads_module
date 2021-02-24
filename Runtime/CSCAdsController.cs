using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public class CSCAdsController : MonoBehaviour, IAdsInterface
{
    public const string GET_ADS_LIST_URL = "", UPDATE_CAMP_URL = "";
    private static CSCAdsController instance = null;
    public static CSCAdsController Instance { get { return instance; } }
    public string appKey = "";
    [SerializeField] private CSCVideoAds landscapeVideoAdsPrefab, verticalVideoAdsPrefab;
    [SerializeField] private CSCInterstitialAds landscapeInterstitialAdsPrefab, verticalInterstitialAdsPrefab;
    private AdsCampaignCollection campaigns;
    private string dataPath = "";
    private bool canShowRewardOffline = true;
    public bool IsInterstitialAdsReady()
    {
        var interstitialElements = GetAdsElementWithType("5");
        interstitialElements.AddRange(GetAdsElementWithType("4"));
        return interstitialElements.Count > 0;
    }
    public bool IsVideoRewardAdsReady()
    {
        var rewardElements = GetAdsElementWithType("7");
        rewardElements.AddRange(GetAdsElementWithType("6"));
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return rewardElements.Count > 0 && canShowRewardOffline;
        }
        return rewardElements.Count > 0;
    }
    public void LoadInterstitial()
    {
        throw new NotImplementedException();
    }
    public void LoadRewardedVideo()
    {
        throw new NotImplementedException();
    }
    public void ShowInterstitial()//instance a ads full here
    {
        var interstitialElements = GetAdsElementWithType("5");
        interstitialElements.AddRange(GetAdsElementWithType("4"));
        if (interstitialElements.Count > 0)
        {
            var adsData = interstitialElements[UnityEngine.Random.Range(0, interstitialElements.Count)];
            if (adsData.type == "5")
                landscapeInterstitialAdsPrefab.Spawn(adsData);
            else
                verticalInterstitialAdsPrefab.Spawn(adsData);
            UpdateAdsCampaign(adsData.id, 1, int.Parse(adsData.type));
        }
    }
    public void ShowVideoAds(Action onUserEarnedReward, Action onAdClosed)//instance a video ads here
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (!canShowRewardOffline)
                return;
            canShowRewardOffline = false;
        }
        var rewardElements = GetAdsElementWithType("7");
        rewardElements.AddRange(GetAdsElementWithType("6"));
        if (rewardElements.Count > 0)
        {
            var adsData = rewardElements[UnityEngine.Random.Range(0, rewardElements.Count)];
            if (adsData.type == "7")
                landscapeVideoAdsPrefab.Spawn(adsData, onUserEarnedReward, onAdClosed);
            else
                verticalVideoAdsPrefab.Spawn(adsData, onUserEarnedReward, onAdClosed);
            UpdateAdsCampaign(adsData.id, 1, int.Parse(adsData.type));
        }
    }
    public List<AdsElement> GetAdsElementWithType(string type)
    {
        var result = new List<AdsElement>();
        if (campaigns == null || campaigns.result == null) return result;
        foreach (var adsCampaign in campaigns.result)
        {
            foreach (var adsElement in adsCampaign.arrImages)
                if (adsElement.type == type && adsElement.IsAdsAvaiable())
                {
                    adsElement.targetUrl = adsCampaign.targetUrl;
                    adsElement.id = adsCampaign.id;
                    result.Add(adsElement);
                }
        }
        return result;
    }
    private IEnumerator Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            yield return new WaitForSeconds(4);//delay inititata inhouse ads
            dataPath = Path.Combine(Application.persistentDataPath, "ads_inhouse_data.dat");
            LoadData();
            AdsCampaignCollection newCampaigns = null;
            UnityWebRequest www = UnityWebRequest.Get(GET_ADS_LIST_URL + "?appId=" + appKey + "&os=" + (Application.platform == RuntimePlatform.Android ? 1 : 2));
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                yield break;
            }
            else
            {
                newCampaigns = JsonUtility.FromJson<AdsCampaignCollection>(www.downloadHandler.text);
                if (newCampaigns.result == null) yield break;
                List<AdsCampaign> updatedCampaign = new List<AdsCampaign>();
                foreach (AdsCampaign campaign in newCampaigns.result)
                {
                    if (campaigns.IsCampaignNew(campaign) || !campaign.IsCampaignCached())
                        updatedCampaign.Add(campaign);
                }
                campaigns = newCampaigns;
                SaveData();
                var adsElements = new List<AdsElement>();
                foreach (var campaign in updatedCampaign)
                    adsElements.AddRange(campaign.arrImages);
                foreach (var element in adsElements)
                    yield return StartCoroutine(CacheElement(element));//cache ads data
            }
        }
        else Destroy(gameObject);
    }
    private IEnumerator CacheElement(AdsElement element)
    {
        UnityWebRequest www = UnityWebRequest.Get(element.url);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
            Debug.Log(www.error);
        else
        {
            try
            {
                System.IO.File.WriteAllBytes(element.GetAdsPath(), www.downloadHandler.data);
            }
            catch (Exception e)
            {
                Debug.Log("Fail to cache ads" + e.Message);
            }
        }
    }
    private void SaveData()
    {
        string origin = JsonUtility.ToJson(campaigns);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (FileStream fileStream = File.Open(dataPath, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, origin);
        }
    }
    private void LoadData()
    {
        if (File.Exists(dataPath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            string data;
            using (FileStream fileStream = File.Open(dataPath, FileMode.Open))
            {
                try
                {
                    data = (string)binaryFormatter.Deserialize(fileStream);
                    campaigns = JsonUtility.FromJson<AdsCampaignCollection>(data);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.Message);
                    campaigns = new AdsCampaignCollection();
                }
            }
        }
        else
            campaigns = new AdsCampaignCollection();
    }
    public void UpdateAdsCampaign(string campId, int act, int type)
    {
        StartCoroutine(_UpdateAdsCampaign(appKey, campId, act, type));
    }
    private IEnumerator _UpdateAdsCampaign(string appId, string campId, int act, int type)
    {
        AdsCampaignStatus adsCampaign = new AdsCampaignStatus(appId, campId, act, type);
        var request = new UnityWebRequest(UPDATE_CAMP_URL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(adsCampaign));
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);
    }
}
[System.Serializable]
public class AdsCampaignCollection
{
    public int code;
    public AdsCampaign[] result = null;
    public bool IsCampaignNew(AdsCampaign campaign)
    {
        if (result == null) return true;
        foreach (var _campaign in result)
            if (_campaign.id == campaign.id && _campaign.time == campaign.time)
                return false;
        return true;
    }
}
[System.Serializable]
public class AdsCampaign
{
    public string id, pknId, targetUrl;
    public AdsElement[] arrImages;
    public long time;
    public bool IsCampaignCached()
    {
        bool result = true;
        foreach (var element in arrImages)
            result = result && element.IsAdsAvaiable();
        return result;
    }
}
[System.Serializable]
public class AdsElement
{
    public string type, url, name, targetUrl, id;//Type:1: icon 2: banner 3: square (icon large) 4: image full portrait 5: image full landscape 6: video full portrait 7: video full landscape
    public bool IsAdsAvaiable()
    {
        return File.Exists(Path.Combine(Application.persistentDataPath, name));
    }
    public string GetAdsPath()
    {
        return Path.Combine(Application.persistentDataPath, name);
    }
}
[System.Serializable]
public class AdsCampaignStatus
{
    public string appId, deviceId, campId;
    public int os, act, type;
    public AdsCampaignStatus(string appId, string campId, int act, int type)
    {
        this.appId = appId;
        os = (Application.platform == RuntimePlatform.Android ? 1 : 2);
        deviceId = SystemInfo.deviceUniqueIdentifier;
        this.appId = appId;
        this.act = act;
        this.type = type;
        this.campId = campId;
    }
}