mergeInto(LibraryManager.library, {
    // 將 Unity 的訊息傳遞給網頁 UI 並顯示
    PassMessageToWeb: function (str) {
        var message = UTF8ToString(str);
        console.log("Unity Response: " + message);
        
        // 對接 index.html 中的全局函數
        if (window.dispatchAnswerToHTML) {
            window.dispatchAnswerToHTML(message);
        }
    },

    // 設定 Bot 正在打字的狀態，控制發送鈕是否禁用
    SetBotTypingStatus: function (isTyping) {
        console.log("Bot Typing Status: " + isTyping);
        
        // 對接 index.html 中的狀態函數
        if (window.setTypingStatus) {
            window.setTypingStatus(isTyping);
        }
    }
});