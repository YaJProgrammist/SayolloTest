using System;
using UnityEngine;

namespace SayolloSDK
{
    public class SayolloSDK : MonoBehaviour
    {
        public enum AdResult
        {
            Success,
            Error,
            Cancelled
        }

        private static SayolloSDK _instance;
        public static SayolloSDK Instance 
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = (SayolloSDK)FindObjectOfType(typeof(SayolloSDK));
                    if (_instance == null)
                    {
                        _instance = (new GameObject("SayolloSDK", typeof(SayolloSDK))).GetComponent<SayolloSDK>();
                    }

                    _instance.Init();
                    DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }

        private VideoAd videoAd;
        private PurchaseAd purchaseAd;
        private SayolloSdkConfig sayolloSdkConfig;

        private void Init()
        {
            sayolloSdkConfig = Resources.Load<SayolloSdkConfig>("SayolloSdkConfig");
        }

        class EmptyDisposable : IDisposable
        {
            public void Dispose() {}
        }

        // You can dispose returned object to cancel ad showing
        public IDisposable ShowVideoAd(Action<AdResult, string> resultCallback)
        {
            if (videoAd != null)
            {
                resultCallback(AdResult.Error, "ad is already shown");
                return new EmptyDisposable();
            }
            videoAd = new VideoAd(sayolloSdkConfig.VideoAdCanvasPrefab, sayolloSdkConfig.VideoUri);
            async void TrackVideoAd()
            {
                await videoAd.ShowVideo(resultCallback);
                videoAd = null;
            }
            TrackVideoAd();
            return videoAd;
        }

        // You can dispose returned object to cancel ad showing
        public IDisposable ShowPurchaseAd(Action<AdResult, string> resultCallback)
        {
            if (purchaseAd != null)
            {
                resultCallback(AdResult.Error, "ad is already shown");
                return new EmptyDisposable();
            }
            purchaseAd = new PurchaseAd(sayolloSdkConfig.PurchaseAdCanvasPrefab, sayolloSdkConfig.PurchaseItemUrl, sayolloSdkConfig.UserInfoUrl);
            async void TrackPurchaseAd()
            {
                await purchaseAd.ShowPurchase(resultCallback);
                purchaseAd = null;
            }
            TrackPurchaseAd();
            return purchaseAd;
        }
    }
}
