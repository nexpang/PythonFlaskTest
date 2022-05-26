using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class User
{
    public string uId;
    public string uPw;
    public string uName;
}

public class WebManager : MonoBehaviour
{
    public InputField postData;

    const string url = "http://127.0.0.1:5000/getData";

    private void Start()
    {
        //StartCoroutine(GetRequest(url));
        //StartCoroutine(Upload(url));
    }

    public void PostSend()
    {
        string json = JsonUtility.ToJson(new User() { uId = "wlrn98765", uPw = "chfhrdl365!", uName = "nexpang" });
        StartCoroutine(Upload(url));
    }

    IEnumerator Upload(string url, string json)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(url, json))
        {
            byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            switch (www.result)
            {
                case UnityWebRequest.Result.InProgress:
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Form upload complete!");
                    Debug.Log(www.downloadHandler.text);
                    break;
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(www.error);
                    break;
            }
        }
    }

    IEnumerator GetRequest(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            switch (www.result)
            {
                case UnityWebRequest.Result.InProgress:
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Form upload complete!");
                    Debug.Log(www.downloadHandler.text);
                    break;
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(www.error);
                    break;
            }
        }
    }
}
