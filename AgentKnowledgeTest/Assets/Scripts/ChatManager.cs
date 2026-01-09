using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class ChatManager : MonoBehaviour
{
    // 匯入網頁溝通接口
    [DllImport("__Internal")]
    private static extern void PassMessageToWeb(string str);
    [DllImport("__Internal")]
    private static extern void SetBotTypingStatus(int isTyping);

    public static ChatManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private const string ChatHistoryKey = "FullChatHistory";
    private const string MessageDelimiter = "<MSG_DELIM>";

    [Header("Unity 內部 UI (選用)")]
    public Transform chatContentParent;
    public ScrollRect chatScrollRect;
    public GameObject userMessagePrefab;
    public GameObject friendMessagePrefab;

    private GameObject typingIndicatorInstance;

    void Start()
    {
        ClearChatHistory();
        // 啟動時通知網頁準備就緒 (選配)
#if UNITY_WEBGL && !UNITY_EDITOR
            PassMessageToWeb("UNITY_READY");
#endif
    }

    // --- 網頁專用接口 (由 HTML 的 SendMessage 呼叫) ---

    // 對應 HTML 中的 triggerSend()
    public void GetMessageFromHTML(string userMessage)
    {
        OnMessageSent(userMessage);
    }

    // 對應 HTML 中的 triggerClear()
    public void ClearChat()
    {
        ClearChatHistory();
    }

    // --- 核心邏輯 ---

    public void OnMessageSent(string userMessage)
    {
        // 1. 在 Unity 畫面顯示 (如果你還想保留 Unity 內的對話框)
        DisplaySystemMessage(userMessage, userMessagePrefab);
        SaveChatHistory();

        // 2. 通知網頁：Bot 開始思考
#if UNITY_WEBGL && !UNITY_EDITOR
            SetBotTypingStatus(1);
#endif

        typingIndicatorInstance = DisplaySystemMessage("Bot 正在思考...", friendMessagePrefab);
        ScrollToBottom();

        // 3. 呼叫你的 API 代理人
        if (APITestAgent.Instance != null)
            APITestAgent.Instance.AskQuestion(userMessage);
        else
            RemoveTypingIndicator();
    }

    public void ReceiveBotResponse(string answer)
    {
        RemoveTypingIndicator();
        DisplaySystemMessage(answer, friendMessagePrefab);
        SaveChatHistory();
        ScrollToBottom();

        // 4. 重要：將 AI 的回答傳回給網頁顯示
#if UNITY_WEBGL && !UNITY_EDITOR
            SetBotTypingStatus(0);
            PassMessageToWeb(answer);
#endif
    }

    private GameObject DisplaySystemMessage(string message, GameObject messagePrefab)
    {
        if (messagePrefab == null || chatContentParent == null) return null;
        GameObject messageInstance = Instantiate(messagePrefab, chatContentParent);
        TMP_Text messageText = messageInstance.GetComponentInChildren<TMP_Text>();
        if (messageText != null) messageText.text = message;
        LayoutRebuilder.ForceRebuildLayoutImmediate(messageInstance.GetComponent<RectTransform>());
        return messageInstance;
    }

    private void RemoveTypingIndicator()
    {
        if (typingIndicatorInstance != null)
        {
            Destroy(typingIndicatorInstance);
            typingIndicatorInstance = null;
        }
    }

    public void SaveChatHistory()
    {
        if (chatContentParent == null) return;
        StringBuilder historyBuilder = new StringBuilder();
        for (int i = 0; i < chatContentParent.childCount; i++)
        {
            Transform messageObject = chatContentParent.GetChild(i);
            TMP_Text textComponent = messageObject.GetComponentInChildren<TMP_Text>();
            if (textComponent != null)
            {
                string prefix = messageObject.name.Contains(userMessagePrefab.name) ? "[USER]" : "[BOT]";
                historyBuilder.Append(prefix).Append(textComponent.text).Append(MessageDelimiter);
            }
        }
        PlayerPrefs.SetString(ChatHistoryKey, historyBuilder.ToString());
        PlayerPrefs.Save();
    }

    public void ClearChatHistory()
    {
        PlayerPrefs.DeleteKey(ChatHistoryKey);
        if (chatContentParent != null)
        {
            foreach (Transform child in chatContentParent) Destroy(child.gameObject);
        }
        StartCoroutine(ReinitializeUI());
    }

    private IEnumerator ReinitializeUI()
    {
        yield return new WaitForEndOfFrame();
        DisplaySystemMessage("對話已重新開始。", friendMessagePrefab);
    }

    public void ScrollToBottom()
    {
        if (chatScrollRect != null) StartCoroutine(ForceScrollToBottom());
    }

    private IEnumerator ForceScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (chatScrollRect != null) chatScrollRect.verticalNormalizedPosition = 0f;
    }
}