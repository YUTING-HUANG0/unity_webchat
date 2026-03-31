
// 1. 知識庫定義 (Knowledge Base Definition)
//    - 這是您的 QA 數據，包含用於匹配的關鍵字。
// ----------------------------------------------------

const KNOWLEDGE_BASE = [
    {
        "主題": "生成式AI",
        "問題": "什麼是生成式AI（Generative AI）？",
        "答案": "生成式AI是一種人工智慧，它能夠根據學習到的數據模式，創造出全新的、原創的內容，如文本、圖像、音訊、視訊和程式碼。",
        "keywords": ["生成式AI", "Generative AI", "定義", "創造內容"]
    },
    {
        "主題": "生成式AI",
        "問題": "生成式AI的核心技術是什麼？",
        "答案": "目前生成式AI的核心技術主要基於大型語言模型（LLMs）和擴散模型（Diffusion Models），它們使用變形器（Transformer）架構來處理序列數據並生成內容。",
        "keywords": ["核心技術", "LLMs", "大型語言模型", "擴散模型", "Diffusion Models"]
    },
    {
        "主題": "科技接受模型 (TAM)",
        "問題": "TAM的兩個核心構念是什麼？",
        "答案": "兩個核心構念是「感知有用性」（Perceived Usefulness, PU）和「感知易用性」（Perceived Ease of Use, PEOU）。",
        "keywords": ["TAM", "核心構念", "感知有用性", "感知易用性", "PU", "PEOU"]
    },
    {
        "主題": "科技接受模型 (TAM)",
        "問題": "「感知有用性」指的是什麼？",
        "答案": "感知有用性是指使用者認為使用特定系統將會提升其工作績效的程度。",
        "keywords": ["感知有用性", "定義", "績效提升"]
    },
    {
        "主題": "情境學習",
        "問題": "情境學習的核心觀點是什麼？",
        "答案": "核心觀點是「知識是情境化的」（Knowledge is situated）和「學習是參與性的」（Learning is participation），主張通過實踐和社群參與進行學習。",
        "keywords": ["情境學習", "核心觀點", "知識情境化", "學習參與性"]
    },
    {
        "主題": "情境學習",
        "問題": "情境學習中「合法邊緣性參與」（Legitimate Peripheral Participation）是什麼意思？",
        "答案": "這是指新手通過在實踐社群（Community of Practice）中從事低風險的任務，逐漸從邊緣走向核心的參與過程，從而獲得知識與技能。",
        "keywords": ["合法邊緣性參與", "LPP", "新手", "社群參與"]
    },
    {
        "主題": "資訊領域學習",
        "問題": "資訊素養（Information Literacy）的定義是什麼？",
        "答案": "資訊素養是指個體能夠有效地識別資訊需求、定位、評估、組織和創造性地使用資訊的能力。",
        "keywords": ["資訊素養", "定義", "Information Literacy", "評估資訊"]
    },
    {
        "主題": "資訊領域學習",
        "問題": "什麼是「數位落差」（Digital Divide）？",
        "答案": "數位落差指的是不同群體之間在獲取、使用資訊和通訊技術（ICT）的能力和機會上的差距。",
        "keywords": ["數位落差", "Digital Divide", "ICT", "差距"]
    }
];

// ----------------------------------------------------
// 2. 核心功能：知識查詢 (Lookup Function)
// ----------------------------------------------------

/**
 * 根據輸入文本，使用關鍵字匹配從知識庫中查找最相關的答案。
 * @param {string} inputText 用戶輸入的查詢文本。
 * @returns {string} 匹配到的答案文本或默認提示。
 */
function lookupKnowledge(inputText) {
    // 將輸入文本轉換為小寫，以便進行不區分大小寫的匹配
    const normalizedInput = inputText.toLowerCase();
    
    // 過濾出所有匹配到的知識條目
    const matchingResults = KNOWLEDGE_BASE.filter(item => {
        // 檢查輸入文本是否包含該條目中的任何一個關鍵字
        return item.keywords.some(keyword => {
            // 使用 String.prototype.includes 進行簡單的子字串匹配
            return normalizedInput.includes(keyword.toLowerCase());
        });
    });
    
    // 3. 結果處理和輸出
    
    if (matchingResults.length === 0) {
        // 如果沒有匹配到任何結果
        return "🤖 抱歉，我目前無法在知識庫中找到與您的問題直接相關的資訊。\n\n💡 建議：\n1. 嘗試使用更簡短的關鍵字 (例如：生成式AI, TAM, 情境學習)。\n2. 檢查是否有錯字。\n3. 您可以點選下方的預設建議按鈕來了解系統功能。";
    }
    
    if (matchingResults.length === 1) {
        // 僅匹配到一個結果
        const result = matchingResults[0];
        return (
            `🤖 知識查詢結果 (主題: ${result.主題}):\n` +
            `Q: ${result.問題}\n` +
            `A: ${result.答案}`
        );
    }
    
    // 匹配到多個結果
    const output = [
        "🤖 知識庫匹配到多個相關結果，請參考以下資訊："
    ];
    
    matchingResults.forEach((result, index) => {
        output.push(
            `---\n` +
            `結果 ${index + 1} (主題: ${result.主題}):\n` +
            `Q: ${result.問題}\n` +
            `A: ${result.答案}`
        );
    });
    
    return output.join('\n');
}

// ----------------------------------------------------
// 4. 執行範例 (Execution Example)
// ----------------------------------------------------

console.log("--- Agent 知識庫查詢測試 (Node.js) ---");

/*
// 測試案例 1: 精確匹配
const query1 = "請問 TAM 的兩個核心構念是什麼？";
console.log(`\n> 查詢 1: ${query1}`);
const response1 = lookupKnowledge(query1);
console.log(response1);

// 測試案例 2: 包含關鍵字，但不是精確問題
const query2 = "我想知道什麼是 Generative AI";
console.log(`\n> 查詢 2: ${query2}`);
const response2 = lookupKnowledge(query2);
console.log(response2);

// 測試案例 3: 匹配不到結果
const query3 = "關於永續發展目標有沒有資訊？";
console.log(`\n> 查詢 3: ${query3}`);
const response3 = lookupKnowledge(query3);
console.log(response3);

// 測試案例 4: 模糊或多重匹配 (例如同時包含 TAM 和 感知有用性)
const query4 = "我想查查 TAM 模型的 感知有用性 是什麼？";
console.log(`\n> 查詢 4: ${query4}`);
const response4 = lookupKnowledge(query4);
console.log(response4);
*/

// 導出函數以供 server.js 使用
module.exports = { lookupKnowledge };
