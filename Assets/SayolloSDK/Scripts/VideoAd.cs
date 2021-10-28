using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Video;

namespace SayolloSDK
{
    class VideoAd : IDisposable
    {
        private string uri;
        private GameObject canvasPrefab;
        private GameObject canvas;
        public bool videoEnded;
        public bool cancelled;

        public VideoAd(GameObject videoAdCanvasPrefab, string videoUri)
        {
            canvasPrefab = videoAdCanvasPrefab;
            uri = videoUri;
        }

        private static string videoFileName = "video.webm";
        private string savedVideoPath => $"{Application.dataPath}/{videoFileName}";
        private bool videoIsLoaded => File.Exists(savedVideoPath);

        public async Task ShowVideo(Action<SayolloSDK.AdResult, string> resultCallback)
        {
            try
            {
                var videoShowTask = ShowVideo();
                while (!videoEnded && !cancelled)
                {
                    await Task.Yield();
                }

                if (videoEnded)
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
                resultCallback(SayolloSDK.AdResult.Error, e.Message);
            }
            finally
            {
                Dispose();
            }
        }

        async Task ShowVideo()
        {
            if (videoIsLoaded)
            {
                ShowSavedVideo(savedVideoPath);
                return;
            }

            var xml = await WebTools.LoadString(uri);
            XDocument document = XDocument.Parse(xml);
            var data = document.ToDictionary();
            if (!data.ContainsKey("MediaFile"))
            {
                return;
            }

            var videoUrl = data["MediaFile"];
            var video = await WebTools.LoadData(videoUrl);

            File.WriteAllBytes(savedVideoPath, video);
            ShowSavedVideo(savedVideoPath);
            
            while (!videoEnded && !cancelled)
            {
                await Task.Yield();
            }
        }

        private void ShowSavedVideo(string path)
        {
            canvas = GameObject.Instantiate(canvasPrefab);
            VideoPlayer videoPlayer = canvas.GetComponentInChildren<VideoPlayer>();
            videoPlayer.url = path;
            videoPlayer.EnableAudioTrack(0, true);
            videoPlayer.Prepare();
            videoPlayer.Play();
            videoPlayer.loopPointReached += HandleVideoEnded;
        }

        private void HandleVideoEnded(VideoPlayer player)
        {
            videoEnded = true;
        }

        public void Dispose()
        {
            cancelled = true;
            if (canvas) GameObject.Destroy(canvas);
        }
    }
}