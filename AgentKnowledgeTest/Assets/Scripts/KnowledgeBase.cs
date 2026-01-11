using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 知識庫條目的資料結構
[System.Serializable]
public class KnowledgeItem
{
    public string 主題;
    public string 問題;
    public string 答案;
    public List<string> keywords;
}

public static class KnowledgeBase
{
    // 完整 30 題題庫保留
    private static readonly List<KnowledgeItem> KNOWLEDGE_BASE_DATA = new List<KnowledgeItem>
    {
        // === 1. 生成式 AI 領域 ===
        new KnowledgeItem {
            主題 = "生成式AI",
            問題 = "什麼是生成式AI（Generative AI）？",
            答案 = "生成式AI是一種人工智慧，它能夠根據學習到的數據模式，創造出全新的、原創的內容，如文本、圖像、音訊、視訊和程式碼。",
            keywords = new List<string> { "生成式AI", "Generative AI", "生成式人工智慧" }
        },
        new KnowledgeItem {
            主題 = "生成式AI",
            問題 = "生成式AI的核心技術是什麼？",
            答案 = "目前主要基於大型語言模型（LLMs）和擴散模型（Diffusion Models），使用變形器（Transformer）架構來處理序列數據並生成內容。",
            keywords = new List<string> { "生成式AI技術", "生成式AI原理", "Transformer架構", "核心技術" }
        },
        new KnowledgeItem {
            主題 = "生成式AI",
            問題 = "什麼是大語言模型 (LLM)？",
            答案 = "大語言模型是透過海量文本數據訓練的深度學習模型，具備理解自然語言、翻譯及創作的能力，如 GPT 系列。",
            keywords = new List<string> { "大語言模型", "LLM", "Large Language Model" }
        },
        new KnowledgeItem {
            主題 = "生成式AI",
            問題 = "什麼是提示工程 (Prompt Engineering)？",
            答案 = "提示工程是指透過精心設計輸入指令（Prompt），以引導 AI 模型產生更精確、更高品質輸出內容的技術。",
            keywords = new List<string> { "提示工程", "Prompt Engineering", "AI指令設計" }
        },
        new KnowledgeItem {
            主題 = "生成式AI",
            問題 = "什麼是 AI 幻覺 (AI Hallucination)？",
            答案 = "指 AI 模型產生看似合理但事實錯誤或與現實不符的資訊現象，通常是因為數據偏差或模型預測特性導致。",
            keywords = new List<string> { "AI幻覺", "AI Hallucination", "AI錯誤資訊" }
        },
        new KnowledgeItem {
            主題 = "生成式AI",
            問題 = "擴散模型 (Diffusion Model) 的應用？",
            答案 = "主要用於圖像生成領域，如 Midjourney，透過從雜訊中還原圖像的過程來生成高品質視覺內容。",
            keywords = new List<string> { "擴散模型", "Diffusion Model", "圖像生成技術" }
        },

        // === 2. 科技接受模型 (TAM) ===
        new KnowledgeItem {
            主題 = "科技接受模型 (TAM)",
            問題 = "TAM 的兩個核心構念是什麼？",
            答案 = "兩個核心構念是「感知有用性」（PU）和「感知易用性」（PEOU）。",
            keywords = new List<string> { "TAM核心構念", "科技接受模型構面", "TAM架構", "核心構念" }
        },
        new KnowledgeItem {
            主題 = "科技接受模型 (TAM)",
            問題 = "「感知有用性」(PU) 是什麼？",
            答案 = "指使用者認為使用特定技術或系統後，對其工作績效提升的程度。",
            keywords = new List<string> { "感知有用性", "Perceived Usefulness", "PU" }
        },
        new KnowledgeItem {
            主題 = "科技接受模型 (TAM)",
            問題 = "「感知易用性」(PEOU) 是什麼？",
            答案 = "指使用者感受到的學習或操作該技術系統的費力程度，越容易上手則易用性越高。",
            keywords = new List<string> { "感知易用性", "Perceived Ease of Use", "PEOU" }
        },
        new KnowledgeItem {
            主題 = "科技接受模型 (TAM)",
            問題 = "TAM 模型的目的是什麼？",
            答案 = "用於解釋和預測使用者對於新資訊技術的接受意向與實際使用行為。",
            keywords = new List<string> { "TAM", "目的", "用途", "為什麼使用TAM" }
        },

        // === 3. 情境學習與教學理論 ===
        new KnowledgeItem {
            主題 = "情境學習",
            問題 = "情境學習的核心觀點是什麼？",
            答案 = "核心觀點是「知識是情境化的」和「學習是參與性的」，主張知識必須在真實環境中透過實踐獲得。",
            keywords = new List<string> { "情境學習觀點", "Situated Learning", "情境學習核心" }
        },
        new KnowledgeItem {
            主題 = "情境學習",
            問題 = "什麼是「合法邊緣性參與」(LPP)？",
            答案 = "指新手在實踐社群中透過執行簡單任務，逐漸向社群核心靠攏並習得專家技能的過程。",
            keywords = new List<string> { "合法邊緣性參與", "LPP", "Legitimate Peripheral Participation" }
        },
        new KnowledgeItem {
            主題 = "情境學習",
            問題 = "什麼是實踐社群 (CoP)？",
            答案 = "由一群對某件事有共同興趣、願意透過持續互動來學習如何做得更好的人所組成的團體。",
            keywords = new List<string> { "實踐社群", "Community of Practice", "CoP" }
        },
        new KnowledgeItem {
            主題 = "教育科技",
            問題 = "什麼是翻轉課堂 (Flipped Classroom)？",
            答案 = "將課堂講授與課後作業順序對調，學生課前看教材，課堂時間用於討論與實作。",
            keywords = new List<string> { "翻轉課堂", "Flipped Classroom", "翻轉教學" }
        },
        new KnowledgeItem {
            主題 = "教育科技",
            問題 = "什麼是混成學習 (Blended Learning)？",
            答案 = "結合傳統面對面教學與數位線上學習的教學模式，提供學生更彈性的學習路徑。",
            keywords = new List<string> { "混成學習", "Blended Learning", "混合式學習" }
        },
        new KnowledgeItem {
            主題 = "教育科技",
            問題 = "什麼是鷹架理論 (Scaffolding)？",
            答案 = "指教師或同儕在學生學習初期提供的引導與支持，隨著能力提升，支持會逐漸撤去。",
            keywords = new List<string> { "鷹架理論", "Scaffolding", "教學鷹架" }
        },

        // === 4. 資訊素養與數位安全 ===
        new KnowledgeItem {
            主題 = "資訊素養",
            問題 = "什麼是資訊素養 (Information Literacy)？",
            答案 = "指個體能夠有效地識別資訊需求、定位、評估並創造性地使用資訊的能力。",
            keywords = new List<string> { "資訊素養", "Information Literacy" }
        },
        new KnowledgeItem {
            主題 = "資訊領域",
            問題 = "什麼是數位落差 (Digital Divide)？",
            答案 = "指不同群體間，在獲取與使用資訊通訊技術 (ICT) 機會上的不平等現象。",
            keywords = new List<string> { "數位落差", "Digital Divide" }
        },
        new KnowledgeItem {
            主題 = "資訊領域",
            問題 = "什麼是數位原住民 (Digital Natives)？",
            答案 = "指在數位科技普及環境中出生、成長，自然熟練使用網路與行動裝置的一代。",
            keywords = new List<string> { "數位原住民", "Digital Natives" }
        },
        new KnowledgeItem {
            主題 = "數位倫理",
            問題 = "什麼是資訊隱私 (Information Privacy)？",
            答案 = "指個人有權決定其個人資料在何種程度、何時以及如何被他人或機構使用。",
            keywords = new List<string> { "資訊隱私", "隱私權", "個資保護" }
        },
        new KnowledgeItem {
            主題 = "數位安全",
            問題 = "什麼是深度偽造 (Deepfake)？",
            答案 = "利用 AI 技術合成的人造媒體，最常見的是將現有人像替換成他人的面孔或聲音。",
            keywords = new List<string> { "深度偽造", "Deepfake", "深偽技術" }
        },
        new KnowledgeItem {
            主題 = "數位安全",
            問題 = "什麼是網路霸凌 (Cyberbullying)？",
            答案 = "利用網路通訊設備，持續對他人進行騷擾、恐嚇或詆毀的攻擊行為。",
            keywords = new List<string> { "網路霸凌", "Cyberbullying", "數位暴力" }
        },

        // === 5. 人機互動與趨勢 ===
        new KnowledgeItem {
            主題 = "人機互動",
            問題 = "什麼是用戶體驗 (UX)？",
            答案 = "指用戶在使用產品過程中，產生的主觀感受、認知行為與滿意程度。",
            keywords = new List<string> { "用戶體驗", "User Experience", "UX" }
        },
        new KnowledgeItem {
            主題 = "人機互動",
            問題 = "什麼是用戶介面 (UI)？",
            答案 = "指產品上供用戶操作的視覺佈局、按鈕、顏色等具體設計面。",
            keywords = new List<string> { "用戶介面", "User Interface", "UI" }
        },
        new KnowledgeItem {
            主題 = "新興科技",
            問題 = "什麼是元宇宙 (Metaverse)？",
            答案 = "融合實境與虛擬，讓用戶透過虛擬化身進行互動、交易與社交的沉浸式空間。",
            keywords = new List<string> { "元宇宙", "Metaverse", "虛擬世界" }
        },
        new KnowledgeItem {
            主題 = "新興科技",
            問題 = "什麼是區塊鏈 (Blockchain)？",
            答案 = "一種去中心化的分散式帳本技術，具有資料不可篡改與透明公開的特性。",
            keywords = new List<string> { "區塊鏈", "Blockchain", "分散式帳本" }
        },
        new KnowledgeItem {
            主題 = "新興科技",
            問題 = "什麼是物聯網 (IoT)？",
            答案 = "透過感測器讓物理設備連上網路，實現數據交換與自動化運作的系統。",
            keywords = new List<string> { "物聯網", "IoT", "Internet of Things" }
        },
        new KnowledgeItem {
            主題 = "新興科技",
            問題 = "什麼是虛擬實境 (VR)？",
            答案 = "利用電腦模擬產生一個三維空間的虛擬世界，提供視覺與聽覺的沉浸感。",
            keywords = new List<string> { "虛擬實境", "VR", "Virtual Reality" }
        },
        new KnowledgeItem {
            主題 = "教育科技",
            問題 = "什麼是適性學習 (Adaptive Learning)？",
            答案 = "利用演算法根據學習者的表現與需求，動態調整教學內容與難度的教育方法。",
            keywords = new List<string> { "適性學習", "Adaptive Learning", "個人化學習" }
        },
        new KnowledgeItem {
            主題 = "數位倫理",
            問題 = "什麼是演算法偏見 (Algorithmic Bias)？",
            答案 = "指電腦系統因為不公正的訓練數據或設計，而產生具歧視性的系統性偏差結果。",
            keywords = new List<string> { "演算法偏見", "Algorithmic Bias", "AI歧視" }
        }
    };

    // 唯一的核心查詢邏輯：權重式關鍵字比對
    public static string LookupKnowledge(string inputText)
    {
        if (string.IsNullOrWhiteSpace(inputText)) return "請輸入問題。";
        string normalizedInput = inputText.ToLower();

        // 權重計分系統：只針對 keywords 進行掃描
        var scoredResults = KNOWLEDGE_BASE_DATA.Select(item => {
            int score = 0;
            foreach (var kw in item.keywords)
            {
                if (normalizedInput.Contains(kw.ToLower()))
                {
                    score += 10; // 命中一個關鍵字得 10 分
                }
            }
            return new { item, score };
        })
        .Where(x => x.score > 0)
        .OrderByDescending(x => x.score)
        .ToList();

        if (scoredResults.Count == 0) return "找不到相關資訊。";

        // 回傳分數最高的結果
        var best = scoredResults[0].item;
        return $"[主題: {best.主題}]\nQ: {best.問題}\nA: {best.答案}";
    }
}