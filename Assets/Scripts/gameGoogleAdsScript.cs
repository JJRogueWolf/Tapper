using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class gameGoogleAdsScript : MonoBehaviour
{
    private string APP_ID = "ca-app-pub-2982815476605964~4250580166";
    private string adUnitId = "ca-app-pub-2982815476605964/3019590671";
    [HideInInspector]
    public BannerView bannerView;


    // Start is called before the first frame update
    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        this.RequestBanner();
    }

    public void RequestBanner()
    {
        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        bannerView.Hide();
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
