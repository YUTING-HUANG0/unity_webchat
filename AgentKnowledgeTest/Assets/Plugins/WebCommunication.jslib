mergeInto(LibraryManager.library, {
    PassMessageToWeb: function (str) {
        var message = UTF8ToString(str);
        // 這會觸發網頁上的事件，你可以在 HTML 裡接收
        console.log("Unity Data Sent:", message);
        if (window.onUnityMessage) {
            window.onUnityMessage(message);
        }
    },
    SetBotTypingStatus: function (isTyping) {
        console.log("Bot Typing:", isTyping);
        // 可以在網頁顯示 "AI 正在輸入中..." 的動畫
    }
});