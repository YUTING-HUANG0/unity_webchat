const fs = require('fs');
const zlib = require('zlib');
const path = require('path');

const buildDir = path.join(__dirname, 'Build');
const filesToCompress = [
  'knowledge-agent-api.data',
  'knowledge-agent-api.wasm'
];

filesToCompress.forEach(fileName => {
  const filePath = path.join(buildDir, fileName);
  const gzipPath = filePath + '.gz';

  if (!fs.existsSync(filePath)) {
    console.log(`File not found: ${filePath}`);
    return;
  }

  console.log(`Compressing ${fileName}...`);
  const fileContents = fs.createReadStream(filePath);
  const writeStream = fs.createWriteStream(gzipPath);
  const zip = zlib.createGzip({ level: 9 });

  fileContents
    .pipe(zip)
    .pipe(writeStream)
    .on('finish', (err) => {
      if (err) {
        console.error(`Error compressing ${fileName}:`, err);
      } else {
        const oldSize = fs.statSync(filePath).size;
        const newSize = fs.statSync(gzipPath).size;
        console.log(`✅ Successfully compressed ${fileName}!`);
        console.log(`   Original Size: ${(oldSize / 1024 / 1024).toFixed(2)} MB`);
        console.log(`   Compressed Size: ${(newSize / 1024 / 1024).toFixed(2)} MB`);
        console.log(`   Reduction: ${((1 - newSize / oldSize) * 100).toFixed(1)}%`);
      }
    });
});
