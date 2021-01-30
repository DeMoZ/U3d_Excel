using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace DataLoaders
{
    public class MemoryStreamLoader : IMemoryStreamLoader
    {
        public async void Load(string fileName, Action<MemoryStream> onComplete, Action<string> onFailed)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            Debug.Log(filePath);

            UnityWebRequest uwr = UnityWebRequest.Get(filePath);
            await uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
                onFailed?.Invoke(uwr.error);
            else
            {
                byte[] results = uwr.downloadHandler.data;
                using (var stream = new MemoryStream(results))
                {
                    onComplete?.Invoke(stream);
                }
            }
        }
    }
}