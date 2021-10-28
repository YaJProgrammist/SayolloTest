using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace SayolloSDK
{
    public static class WebTools
    {
        public static async Task<byte[]> LoadData(string uri)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(uri))
            {
                await www.SendWebRequest();
                return www.downloadHandler.data;
            }
        }
        
        public static async Task<string> LoadString(string uri)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(uri))
            {
                await www.SendWebRequest();
                return www.downloadHandler.text;
            }
        }
        
        public static async Task<Texture2D> LoadImage(string uri)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri))
            {
                await www.SendWebRequest();
                return ((DownloadHandlerTexture)www.downloadHandler).texture;
            }
        }

        public static async Task<string> PurchasePost(string uri)
        {
            using (UnityWebRequest www = new UnityWebRequest(uri, "POST"))
            {
                byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes("{}");
                www.uploadHandler = new UploadHandlerRaw(jsonToSend);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                await www.SendWebRequest();
                return www.downloadHandler.text;
            }
        }

        public static async Task<string> PostUserData(string url, UserData userData)
        {
            using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
            {
                byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(JsonUtility.ToJson(userData));
                www.uploadHandler = new UploadHandlerRaw(jsonToSend);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                await www.SendWebRequest();
                return www.downloadHandler.text;
            }
        }
    }
}
