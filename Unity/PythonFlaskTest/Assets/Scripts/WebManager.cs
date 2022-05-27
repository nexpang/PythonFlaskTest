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

[System.Serializable]
public class Result
{
    public bool result;
    public string str;
    public string token;
}

public class WebManager : MonoBehaviour
{
    public CanvasGroup loginPanel;
    public InputField loginIdInputField;
    public InputField loginPwInputField;

    public CanvasGroup registerPanel;
    public InputField registerNameInputField;
    public InputField registerIdInputField;
    public InputField registerPwInputField;
    public InputField registerPwCInputField;

    public CanvasGroup popUpPanel;
    public Text popUpText;

    private bool getRusult = false;
    private string token = null;

    const string URL = "http://127.0.0.1:5000";

    private void Start()
    {
        //StartCoroutine(GetRequest(url));
        //StartCoroutine(Upload(url));
    }

    IEnumerator RegisterPost(string json)
    {
        string url = URL + "/register";
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
                    Result result = JsonUtility.FromJson<Result>(www.downloadHandler.text);
                    if(result.result)
                    {
                        OpenPopup(result.str);
                        OpenLoginPanel();
                    }
                    else
                    {
                        OpenPopup(result.str);
                    }
                    break;
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(www.error);
                    break;
            }
        }
    }


    IEnumerator LoginPost(string json)
    {
        string url = URL + "/login";
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
                    Result result = JsonUtility.FromJson<Result>(www.downloadHandler.text);
                    if (result.result)
                    {
                        token = result.token;
                        OpenPopup(result.str);
                    }
                    else
                    {
                        OpenPopup(result.str);
                    }
                    break;
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(www.error);
                    break;
            }
        }
    }

    public void OnClickRegister()
    {
        if(registerNameInputField.text == "" ||registerIdInputField.text == "" || registerPwInputField.text == ""|| registerPwCInputField.text == "")
        {
            OpenPopup("비어있는 항목이 있습니다");
            return;
        }
        else if(registerPwInputField.text != registerPwCInputField.text)
        {
            OpenPopup("비밀번호가 일치하지 않습니다");
            return;
        }
        string json = JsonUtility.ToJson(new User() { uId = registerIdInputField.text, uPw = registerPwInputField.text, uName = registerNameInputField.text });
        StartCoroutine(RegisterPost(json));
    }

    public void OnClickLogin()
    {
        if (loginIdInputField.text == "" || loginPwInputField.text == "")
        {
            OpenPopup("비어있는 항목이 있습니다");
            return;
        }
        string json = JsonUtility.ToJson(new User() { uId = loginIdInputField.text, uPw = loginPwInputField.text});
        StartCoroutine(LoginPost(json));
    }

    private void OpenPopup(string popUpString)
    {
        popUpText.text = popUpString;
        OpenPanel(popUpPanel, true);
    }

    public void ClosePopup()
    {
        popUpText.text = "";
        OpenPanel(popUpPanel, false);
    }


    public void OpenLoginPanel()
    {
        OpenPanel(registerPanel, false);
        OpenPanel(loginPanel, true);
    }

    public void OpenResisterPanel()
    {
        OpenPanel(loginPanel, false);
        OpenPanel(registerPanel, true);
    }

    private void OpenPanel(CanvasGroup cvsGroup, bool enable)
    {
        cvsGroup.alpha = enable ? 1 : 0;
        cvsGroup.interactable = enable;
        cvsGroup.blocksRaycasts = enable;
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
