using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace SayolloSDK
{
    public class PurchaseAd : IDisposable
    {
        public SayolloSDK.AdResult result { get; private set; }

        public UnityEvent onDismissed = new UnityEvent();
        public UnityEvent onFinished = new UnityEvent();

        private string purchaseUri;
        private string userInfoUri;
        private PurchaseViewController canvasPrefab;
        private PurchaseViewController canvas;
        public bool purchaseClosed;
        public bool cancelled;

        public PurchaseAd(PurchaseViewController purchaseAdCanvasPrefab, string purchUri, string userUri)
        {
            canvasPrefab = purchaseAdCanvasPrefab;
            purchaseUri = purchUri;
            userInfoUri = userUri;
        }

        public async Task ShowPurchase(Action<SayolloSDK.AdResult, string> resultCallback)
        {
            try
            {
                var purchaseShowTask = ShowPurchase();
                while (!purchaseClosed && !cancelled)
                {
                    await Task.Yield();
                }

                if (purchaseClosed)
                {
                    resultCallback(SayolloSDK.AdResult.Success, "");
                }
                else if (cancelled)
                { 
                    resultCallback(SayolloSDK.AdResult.Cancelled, "");
                }
            }
            catch (Exception e)
            {
                HandleFailPurchase();
                resultCallback(SayolloSDK.AdResult.Error, e.Message);
            }
            finally
            {
                if (cancelled)
                {
                    Dispose();
                }
            }
        }

        private async Task ShowPurchase()
        {
            if (cancelled)
            {
                return;
            }
            var purchaseJson = await WebTools.PurchasePost(purchaseUri);
            ItemData itemData = JsonUtility.FromJson<ItemData>(purchaseJson.Replace("'", "\""));
            if (cancelled)
            {
                return;
            }
            canvas = GameObject.Instantiate(canvasPrefab);
            canvas.SetItem(itemData);
            canvas.onPurchase.AddListener(OnPurchase);
            canvas.onClose.AddListener(OnClose);
            var image = await WebTools.LoadImage(itemData.item_image);
            canvas.SetImage(image);

            while (!purchaseClosed && !cancelled)
            {
                await Task.Yield();
            }
        }

        private async void OnPurchase(UserData userData)
        {
            try
            {
                await WebTools.PostUserData(userInfoUri, userData);
            }
            catch (Exception e)
            {
                HandleFailPurchase();
            }
            HandleSuccessPurchase();
        }

        private void OnClose()
        {
            purchaseClosed = true;
            if (canvas) GameObject.Destroy(canvas);
        }

        private void HandleSuccessPurchase()
        {
            canvas.ShowSuccess();
        }

        private void HandleFailPurchase()
        {
            canvas.ShowFail();
        }

        public void Dispose()
        {
            cancelled = true;
            if (canvas) GameObject.Destroy(canvas.gameObject);
        }
    }
}
