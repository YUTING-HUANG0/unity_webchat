using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

[System.Serializable]
public class QueryData
{
    public string query;
}

[System.Serializable]
public class ResponseData
{
    public string question;
    public string answer;
}

public class APITestAgent : MonoBehaviour
{
    public static APITestAgent Instance { get; private set; }

    [Header("模式設定")]
    public bool useLocalKnowledgeBase = true;

    [Header("網路 API 設定")]
    public string apiURL = "http://localhost:3000/api/ask";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AskQuestion(string userQuestion)
    {
        if (useLocalKnowledgeBase)
        {
            StartCoroutine(ProcessLocalQuery(userQuestion));
        }
        else
        {
            StartCoroutine(SendRequest(
                userQuestion,
                (answer) => {
                    if (ChatManager.Instance != null) ChatManager.Instance.ReceiveBotResponse(answer);
                },
                (error) => {
                    if (ChatManager.Instance != null) ChatManager.Instance.ReceiveBotResponse($"[連線錯誤] {error}");
                }
            ));
        }
    }

    private IEnumerator ProcessLocalQuery(string query)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1.0f));

        // 呼叫 KnowledgeBase 的靜態方法
        string answer = KnowledgeBase.LookupKnowledge(query);

        if (ChatManager.Instance != null)
        {
            ChatManager.Instance.ReceiveBotResponse(answer);
        }
    }

    private IEnumerator SendRequest(string question, Action<string> onSuccess, Action<string> onFailure)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            onFailure?.Invoke("發送問題不得為空。");
            yield break;
        }

        QueryData queryPayload = new QueryData { query = question };
        string jsonPayload = JsonUtility.ToJson(queryPayload);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

        using (UnityWebRequest webRequest = new UnityWebRequest(apiURL, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                onFailure?.Invoke(webRequest.error);
            }
            else
            {
                try
                {
                    ResponseData response = JsonUtility.FromJson<ResponseData>(webRequest.downloadHandler.text);
                    onSuccess?.Invoke(response.answer ?? "沒有答案內容。");
                }
                catch (Exception ex)
                {
                    onFailure?.Invoke(ex.Message);
                }
            }
        }
    }
}