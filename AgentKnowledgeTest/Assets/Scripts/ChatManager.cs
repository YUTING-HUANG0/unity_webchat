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

    [Header("Unity 內部 UI 引用 (請務必在 Inspector 拖入)")]
    public TMP_InputField unityInputField; // 拖入層級中的 InputField (TMP)
    public Button unitySendButton;         // 拖入層級中的 SubmitButton_Hidden

    [Header("對話顯示配置")]
    public Transform chatContentParent;
    public ScrollRect chatScrollRect;
    public GameObject userMessagePrefab;
    public GameObject friendMessagePrefab;

    private GameObject typingIndicatorInstance;
    private const string ChatHistoryKey = "FullChatHistory";
    private const string MessageDelimiter = "<MSG_DELIM>";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 1. 綁定 Unity 本地 UI 事件 (解決 Enter 與按鈕點擊送出問題)
        if (unitySendButton != null)
        {
            unitySendButton.onClick.RemoveAllListeners();
            unitySendButton.onClick.AddListener(() => OnMessageSent(unityInputField.text));
        }

        if (unityInputField != null)
        {
            // 核心修正：使用 onSubmit 取代 onEndEdit，這能精確捕捉 Enter 鍵且不會觸發兩次
            unityInputField.onSubmit.RemoveAllListeners();
            unityInputField.onSubmit.AddListener(OnMessageSent);

            // 確保輸入框在啟動時自動取得焦點
            unityInputField.ActivateInputField();
        }

        ClearChatHistory();

        // 啟動時通知網頁準備就緒
#if UNITY_WEBGL && !UNITY_EDITOR
            try { PassMessageToWeb("UNITY_READY"); } catch { Debug.LogWarning("PassMessageToWeb failed - .jslib missing?"); }
#endif
    }

    // --- 接口：供 Web 呼叫或內部觸發 ---

    public void GetMessageFromHTML(string userMessage)
    {
        OnMessageSent(userMessage);
    }

    public void ClearChat()
    {
        ClearChatHistory();
    }

    // --- 核心邏輯 ---

    public void OnMessageSent(string userMessage)
    {
        if (string.IsNullOrWhiteSpace(userMessage)) return;

        Debug.Log($"[ChatManager] 嘗試送出訊息: {userMessage}");

        // 1. 在 Unity 畫面顯示並清空輸入框
        DisplaySystemMessage(userMessage, userMessagePrefab);

        if (unityInputField != null)
        {
            unityInputField.text = "";
            unityInputField.ActivateInputField(); // 送出後自動重新取得焦點
        }

        SaveChatHistory();

        // 2. 通知網頁：Bot 開始思考
#if UNITY_WEBGL && !UNITY_EDITOR
            try { SetBotTypingStatus(1); } catch { }
#endif

        typingIndicatorInstance = DisplaySystemMessage("Bot 正在思考...", friendMessagePrefab);
        ScrollToBottom();

        // 3. 呼叫 API 代理人
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
            try { SetBotTypingStatus(0); PassMessageToWeb(answer); } catch { }
#endif
    }

    private GameObject DisplaySystemMessage(string message, GameObject messagePrefab)
    {
        if (messagePrefab == null || chatContentParent == null) return null;

        GameObject messageInstance = Instantiate(messagePrefab, chatContentParent);
        TMP_Text messageText = messageInstance.GetComponentInChildren<TMP_Text>();
        if (messageText != null) messageText.text = message;

        // 自動綁定預設問題點擊邏輯
        Button btn = messageInstance.GetComponent<Button>();
        if (btn == null) btn = messageInstance.GetComponentInChildren<Button>();

        if (btn != null)
        {
            // 確保目標 Graphic 有正確設定，避免按鈕失效
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => {
                Debug.Log($"[UI] 點擊預設按鈕: {message}");
                OnMessageSent(message);
            });
        }

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
        yield return new WaitForEndOfFrame();

        DisplaySystemMessage("對話已重新開始，您可以點擊預設問題進行詢問：", friendMessagePrefab);

        // 僅顯示 2 個預設問題
        DisplaySystemMessage("什麼是生成式AI（Generative AI）？", friendMessagePrefab);
        DisplaySystemMessage("TAM 的兩個核心構念是什麼？", friendMessagePrefab);

        ScrollToBottom();
    }

    public void ScrollToBottom()
    {
        if (chatScrollRect != null) StartCoroutine(ForceScrollToBottom());
    }

    public void SetCaptureAllKeyboardInput(int capture)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    // 設為 false 時，Unity 不會強佔鍵盤，HTML 的 input 就能正常打英文
    UnityEngine.WebGLInput.captureAllKeyboardInput = (capture == 1);
#endif
    }

    private IEnumerator ForceScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (chatScrollRect != null) chatScrollRect.verticalNormalizedPosition = 0f;
    }
}