using UnityEngine;
using UnityEngine.UI;

namespace SayolloSDK
{
    public class MainMenuViewController : MonoBehaviour
    {
        [SerializeField]
        private Button showVideoButton;

        [SerializeField]
        private Button showItemButton;

        private void Awake()
        {
            showVideoButton.onClick.AddListener(ShowVideo);
            showItemButton.onClick.AddListener(ShowItem);
        }

        private void ShowVideo()
        {
            var videoAdDisposable = SayolloSDK.Instance.ShowVideoAd((result, errorStr) => { Debug.Log($"{result} {errorStr}"); });
        }

        private void ShowItem()
        {
            var purchaseAdDisposable = SayolloSDK.Instance.ShowPurchaseAd((result, errorStr) => { Debug.Log($"{result} {errorStr}"); });
        }
    }
}
