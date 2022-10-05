using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Utility
{
    
    /// <summary>
    /// 
    /// </summary>
    public class WebHelper: SingletonGeneric<WebHelper>
    {
        public IEnumerator Get(string uri, Action<string> callback=null)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(uri))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"GetResp:{www.downloadHandler.text}");
                    callback?.Invoke(www.downloadHandler.text);
                }
            }
        }

    
        public IEnumerator Post(string uri, WWWForm form, Action<string> callback=null)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Debug.Log($"PostResp{www.downloadHandler.text}");
                    callback?.Invoke(www.downloadHandler.text);
                }
            }
        }
    }
}
