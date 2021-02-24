using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class CSCInterstitialAds : MonoBehaviour
{
    private AdsElement adsData;
    [SerializeField] private RawImage adsImage;
    private bool shouldUpdateAdsStatus = false;
    public void Spawn(AdsElement element)
    {
        GameObject go = Instantiate(gameObject);
        go.GetComponent<CSCInterstitialAds>().Init(element);
    }
    public void Init(AdsElement element)
    {
        this.adsData = element;
    }
    void Start()
    {
        Time.timeScale = 0;
        if (File.Exists(adsData.GetAdsPath()))
        {
            var fileData = File.ReadAllBytes(adsData.GetAdsPath());
            var tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
            adsImage.texture = tex;
        }
        else adsImage.gameObject.SetActive(false);
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
    public void OnClickClose()
    {
        Destroy(gameObject);
        Time.timeScale = 1;
    }
}
