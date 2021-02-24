using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class CSCAdsIcon : MonoBehaviour
{
    private AdsElement adsData;
    [SerializeField] private RawImage adsImage;
    private bool shouldUpdateAdsStatus = false;
    public int delay = 10;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);//wait for in house ads to init
        var rewardElements = CSCAdsController.Instance.GetAdsElementWithType("1");
        if (rewardElements.Count == 0)
        {
            gameObject.SetActive(false);
            yield break;
        }
        adsData = rewardElements[UnityEngine.Random.Range(0, rewardElements.Count)];
        if (File.Exists(adsData.GetAdsPath()))
        {
            var fileData = File.ReadAllBytes(adsData.GetAdsPath());
            var tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
            adsImage.texture = tex;
            CSCAdsController.Instance.UpdateAdsCampaign(adsData.id, 1, int.Parse(adsData.type));
        }
        else gameObject.SetActive(false);
    }
    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && shouldUpdateAdsStatus)
        {
            shouldUpdateAdsStatus = false;
            CSCAdsController.Instance.UpdateAdsCampaign(adsData.id, 2, int.Parse(adsData.type));
        }
    }
    public void OnClickAds()
    {
        shouldUpdateAdsStatus = true;
        Application.OpenURL(adsData.targetUrl);
    }
}
