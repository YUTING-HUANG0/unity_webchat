// 引入 Express、cors、fs 和 path
const express = require('express');
const cors = require('cors');
const fs = require('fs');
const path = require('path');

// 引入您的知識庫查詢函數
// ⚠️ 注意：這裡假設您已經在 knowledge_lookup.js 檔案底部導出了 lookupKnowledge 函數
// 請在 knowledge_lookup.js 檔案的**最底部**加上: module.exports = { lookupKnowledge };
const { lookupKnowledge } = require('./knowledge_lookup'); 

const app = express();
const PORT = 3000; // API 將運行在 3000 埠

// 設定中介軟體 (Middleware)
app.use(cors()); // 啟用 CORS，允許跨域請求
app.use(express.json()); // 允許 Express 處理 JSON 格式的請求體

// 提供預先壓縮的 .gz 檔案服務以加速載入時間
app.get(/\.js$/, (req, res, next) => {
    const gzPath = path.join(__dirname, req.path + '.gz');
    if (req.header('Accept-Encoding')?.includes('gzip') && fs.existsSync(gzPath)) {
        req.url = req.url + '.gz';
        res.set('Content-Encoding', 'gzip');
        res.set('Content-Type', 'application/javascript');
    }
    next();
});

app.get(/\.wasm$/, (req, res, next) => {
    const gzPath = path.join(__dirname, req.path + '.gz');
    if (req.header('Accept-Encoding')?.includes('gzip') && fs.existsSync(gzPath)) {
        req.url = req.url + '.gz';
        res.set('Content-Encoding', 'gzip');
        res.set('Content-Type', 'application/wasm');
    }
    next();
});

app.get(/\.data$/, (req, res, next) => {
    const gzPath = path.join(__dirname, req.path + '.gz');
    if (req.header('Accept-Encoding')?.includes('gzip') && fs.existsSync(gzPath)) {
        req.url = req.url + '.gz';
        res.set('Content-Encoding', 'gzip');
        res.set('Content-Type', 'application/octet-stream');
    }
    next();
});

// 提供靜態檔案服務，以便透過 http://localhost:3000/ 預覽 index.html，並避免 file:// 產生的 WebGL CORS 限制
app.use(express.static(__dirname));

// --- 核心 API 端點 ---

// 使用 POST 請求，讓 Unity 可以將問題發送到請求體中
app.post('/api/ask', (req, res) => {
    // 1. 檢查並獲取用戶在請求體中發送的問題
    const userQuery = req.body.query;

    if (!userQuery) {
        // 最快最準確的回應：如果沒有提供問題，直接返回錯誤
        return res.status(400).json({ error: '請求主體中必須包含 "query" 欄位。' });
    }

    console.log(`收到查詢: ${userQuery}`);

    // 2. 呼叫知識庫查詢函數
    const answer = lookupKnowledge(userQuery);

    // 3. 返回結果給 Unity
    res.json({
        question: userQuery,
        answer: answer
    });
});

// 啟動伺服器
app.listen(PORT, () => {
    console.log(`✅ 伺服器已啟動並運行在 http://localhost:${PORT}`);
    console.log(`API 端點: POST http://localhost:${PORT}/api/ask`);
});