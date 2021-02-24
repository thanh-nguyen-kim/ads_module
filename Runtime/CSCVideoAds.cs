using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;
public class CSCVideoAds : MonoBehaviour
{
    private AdsElement adsData;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject closeBtn, installBtn;
    [SerializeField] private Text countDownText;
    [SerializeField] private Image countDownCircle;
    private bool shouldUpdateAdsStatus = false;
    public System.Action onClose, onCloseAndReward;
    float eslapseTime = 0, waitTime = 7;
    public void Spawn(AdsElement element, System.Action onCloseAndReward, System.Action onClose)
    {
        GameObject go = Instantiate(gameObject);
        go.GetComponent<CSCVideoAds>().Init(element, onCloseAndReward, onClose);
    }
    void Start()
    {
        Time.timeScale = 0;
        if (File.Exists(adsData.GetAdsPath()))
        {
            videoPlayer.url = adsData.GetAdsPath();
            videoPlayer.Play();
        }
        StartCoroutine(DelayEnableBtn());
    }
    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && shouldUpdateAdsStatus)
        {
            shouldUpdateAdsStatus = false;
            CSCAdsController.Instance.UpdateAdsCampaign(adsData.id, 2,int.Parse(adsData.type));
        }
    }
    public void Init(AdsElement element, System.Action onCloseAndReward, System.Action onClose)
    {
        this.adsData = element;
        this.onCloseAndReward = onCloseAndReward;
        this.onClose = onClose;
    }
    private IEnumerator DelayEnableBtn()
    {
        var delay = new WaitForSecondsRealtime(1);
        eslapseTime = 0;
        waitTime = 7;
        while (eslapseTime < waitTime)
        {
            countDownText.text = (waitTime - eslapseTime).ToString();
            countDownCircle.fillAmount = 1 - eslapseTime * 1f / waitTime;
            yield return delay;
            eslapseTime++;
        }
        countDownCircle.gameObject.SetActive(false);
        closeBtn.SetActive(true);
        installBtn.SetActive(true);
    }
    public void OnClickAds()
    {
        shouldUpdateAdsStatus = true;
        Application.OpenURL(adsData.targetUrl);
    }
    public void OnClickClose()
    {
        Destroy(gameObject);
        if (eslapseTime > waitTime - 1)
            onCloseAndReward?.Invoke();
        else
            onClose?.Invoke();
        Time.timeScale = 1;
    }
}
