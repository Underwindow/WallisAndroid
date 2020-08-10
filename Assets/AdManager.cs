using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.UI;
using System.Collections;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }
    public Text statusText;
    private InterstitialAd interstitial;
    bool RemoveAdsPurchased 
        => PlayerPrefs.HasKey("NoAds");
#if UNITY_ANDROID
    //private const string interstitialId = "ca-app-pub-3940256099942544/1033173712";

    private const string interstitialId = "ca-app-pub-7634851492231456/4371858920";
#elif UNITY_IPHONE
    private const string interstitialId = "ca-app-pub-7634851492231456/2468051638";
#endif

    public void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (RemoveAdsPurchased)
            return;
        //MobileAds.SetiOSAppPauseOnBackground(true);

        List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };

        // Add some test device IDs (replace with your own device IDs).
        deviceIds.Add(SystemInfo.deviceUniqueIdentifier.ToUpper());

        // Configure TagForChildDirectedTreatment and test device IDs.
        RequestConfiguration requestConfiguration =
            new RequestConfiguration.Builder()
            .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
            .SetTagForUnderAgeOfConsent(TagForUnderAgeOfConsent.Unspecified)
            .SetTestDeviceIds(deviceIds)
            .build();

        MobileAds.SetRequestConfiguration(requestConfiguration);

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => {
            print(initStatus);
        }); 
        RequestInterstitial();
    }

    private void RequestInterstitial()
    {
        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(interstitialId);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the interstitial with the request.
        interstitial.LoadAd(request);

        interstitial.OnAdLoaded += HandleOnAdLoaded;
        interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        interstitial.OnAdOpening += HandleOnAdOpened;
        interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        print("Interstitial failed to load: " + args.Message);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
        InterstitialDestroy();

        RequestInterstitial();
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
        InterstitialDestroy();
    }

    private void InterstitialDestroy() 
        => interstitial?.Destroy();

    public void InterstitialShow(float delay = 0)
        => StartCoroutine(AdDelayCoroutine(delay));

    IEnumerator AdDelayCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!RemoveAdsPurchased)
            interstitial.Show();
    }

    private void OnApplicationQuit()
    {
        InterstitialDestroy();
    }
}
