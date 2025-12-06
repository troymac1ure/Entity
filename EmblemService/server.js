const http = require('http');
const url = require('url');
const zlib = require('zlib');

const PORT = process.env.PORT || 3001;

// Halo 2 emblem generation service - PNG output
// URL format: /P{primary}-S{secondary}-EP{tertiary}-ES{quaternary}-EF{fg}-EB{bg}-ET{toggle}.png

const server = http.createServer(async (req, res) => {
    // Enable CORS
    res.setHeader('Access-Control-Allow-Origin', '*');
    res.setHeader('Access-Control-Allow-Methods', 'GET, OPTIONS');
    res.setHeader('Access-Control-Allow-Headers', 'Content-Type');

    if (req.method === 'OPTIONS') {
        res.writeHead(200);
        res.end();
        return;
    }

    if (req.method !== 'GET') {
        res.writeHead(405, { 'Content-Type': 'text/plain' });
        res.end('Method not allowed');
        return;
    }

    const parsedUrl = url.parse(req.url, true);
    const pathname = parsedUrl.pathname;

    // Health check endpoint
    if (pathname === '/health') {
        res.writeHead(200, { 'Content-Type': 'application/json' });
        res.end(JSON.stringify({ status: 'ok', service: 'h2emblem' }));
        return;
    }

    // Parse the emblem URL format: /P{pri}-S{sec}-EP{ter}-ES{qua}-EF{fg}-EB{bg}-ET{toggle}.png
    const emblemMatch = pathname.match(/^\/P([^-]+)-S([^-]+)-EP([^-]+)-ES([^-]+)-EF(\d+)-EB(\d+)-ET(\d+)\.png$/i);

    if (!emblemMatch) {
        // Try query string format as fallback
        const query = parsedUrl.query;
        if (query.emblem || query.fg || query.bg) {
            return handleQueryFormat(query, res);
        }

        res.writeHead(400, { 'Content-Type': 'text/plain' });
        res.end('Invalid emblem URL format. Expected: /P{primary}-S{secondary}-EP{tertiary}-ES{quaternary}-EF{fg}-EB{bg}-ET{toggle}.png');
        return;
    }

    const [, primaryColor, secondaryColor, tertiaryColor, quaternaryColor, fg, bg, toggle] = emblemMatch;

    try {
        const pngData = generateEmblemPng({
            foreground: parseInt(fg) || 0,
            background: parseInt(bg) || 0,
            toggle: parseInt(toggle) || 0,
            primaryColor: primaryColor,
            secondaryColor: secondaryColor,
            tertiaryColor: tertiaryColor,
            quaternaryColor: quaternaryColor
        });

        res.writeHead(200, {
            'Content-Type': 'image/png',
            'Cache-Control': 'public, max-age=86400'
        });
        res.end(pngData);
    } catch (error) {
        console.error('Error generating emblem:', error);
        res.writeHead(500, { 'Content-Type': 'text/plain' });
        res.end('Error generating emblem');
    }
});

function handleQueryFormat(query, res) {
    const { fg, bg, emblem, toggle, pri, sec, ter, qua } = query;

    try {
        const pngData = generateEmblemPng({
            foreground: parseInt(fg || emblem) || 0,
            background: parseInt(bg) || 0,
            toggle: parseInt(toggle) || 0,
            primaryColor: pri || 'red',
            secondaryColor: sec || 'blue',
            tertiaryColor: ter || 'white',
            quaternaryColor: qua || 'black'
        });

        res.writeHead(200, {
            'Content-Type': 'image/png',
            'Cache-Control': 'public, max-age=86400'
        });
        res.end(pngData);
    } catch (error) {
        console.error('Error generating emblem:', error);
        res.writeHead(500, { 'Content-Type': 'text/plain' });
        res.end('Error generating emblem');
    }
}

// Halo 2 color palette (18 colors, by index)
const H2_COLORS_BY_INDEX = [
    [255, 255, 255], // 0 - White
    [85, 85, 90],    // 1 - Steel
    [192, 50, 50],   // 2 - Red
    [255, 128, 0],   // 3 - Orange
    [255, 200, 0],   // 4 - Gold
    [128, 128, 48],  // 5 - Olive
    [48, 128, 48],   // 6 - Green
    [180, 192, 128], // 7 - Sage
    [0, 192, 192],   // 8 - Cyan
    [0, 128, 128],   // 9 - Teal
    [48, 80, 160],   // 10 - Cobalt
    [32, 64, 160],   // 11 - Blue
    [128, 64, 160],  // 12 - Violet
    [192, 64, 192],  // 13 - Purple
    [255, 160, 192], // 14 - Pink
    [192, 48, 80],   // 15 - Crimson
    [128, 80, 48],   // 16 - Brown
    [192, 160, 112], // 17 - Tan
];

const H2_COLORS_BY_NAME = {
    'white': [255, 255, 255],
    'steel': [85, 85, 90],
    'red': [192, 50, 50],
    'orange': [255, 128, 0],
    'gold': [255, 200, 0],
    'olive': [128, 128, 48],
    'green': [48, 128, 48],
    'sage': [180, 192, 128],
    'cyan': [0, 192, 192],
    'teal': [0, 128, 128],
    'cobalt': [48, 80, 160],
    'blue': [32, 64, 160],
    'violet': [128, 64, 160],
    'purple': [192, 64, 192],
    'pink': [255, 160, 192],
    'crimson': [192, 48, 80],
    'brown': [128, 80, 48],
    'tan': [192, 160, 112]
};

function getColor(colorValue) {
    if (!colorValue) return [128, 128, 128];

    const index = parseInt(colorValue);
    if (!isNaN(index) && index >= 0 && index < H2_COLORS_BY_INDEX.length) {
        return H2_COLORS_BY_INDEX[index];
    }

    const lower = String(colorValue).toLowerCase();
    if (H2_COLORS_BY_NAME[lower]) {
        return H2_COLORS_BY_NAME[lower];
    }

    return [128, 128, 128];
}

const WIDTH = 256;
const HEIGHT = 256;

function generateEmblemPng(params) {
    const {
        foreground,
        background,
        primaryColor,
        secondaryColor,
        tertiaryColor,
        quaternaryColor
    } = params;

    const emblemFgColor = getColor(tertiaryColor);
    const emblemBgColor = getColor(quaternaryColor);
    const armorPrimary = getColor(primaryColor);

    // Create RGBA pixel buffer
    const pixels = Buffer.alloc(WIDTH * HEIGHT * 4);

    // Fill with armor primary color (background plate)
    for (let i = 0; i < WIDTH * HEIGHT; i++) {
        pixels[i * 4] = armorPrimary[0];
        pixels[i * 4 + 1] = armorPrimary[1];
        pixels[i * 4 + 2] = armorPrimary[2];
        pixels[i * 4 + 3] = 255;
    }

    // Draw background shape
    drawBackgroundShape(pixels, background, emblemBgColor);

    // Draw foreground shape
    drawForegroundShape(pixels, foreground, emblemFgColor);

    // Encode as PNG
    return encodePng(pixels, WIDTH, HEIGHT);
}

function setPixel(pixels, x, y, color) {
    if (x < 0 || x >= WIDTH || y < 0 || y >= HEIGHT) return;
    const i = (y * WIDTH + x) * 4;
    pixels[i] = color[0];
    pixels[i + 1] = color[1];
    pixels[i + 2] = color[2];
    pixels[i + 3] = 255;
}

function fillCircle(pixels, cx, cy, r, color) {
    for (let y = cy - r; y <= cy + r; y++) {
        for (let x = cx - r; x <= cx + r; x++) {
            const dx = x - cx;
            const dy = y - cy;
            if (dx * dx + dy * dy <= r * r) {
                setPixel(pixels, Math.round(x), Math.round(y), color);
            }
        }
    }
}

function fillRect(pixels, x1, y1, x2, y2, color) {
    for (let y = Math.round(y1); y <= Math.round(y2); y++) {
        for (let x = Math.round(x1); x <= Math.round(x2); x++) {
            setPixel(pixels, x, y, color);
        }
    }
}

function fillTriangle(pixels, x1, y1, x2, y2, x3, y3, color) {
    const minX = Math.floor(Math.min(x1, x2, x3));
    const maxX = Math.ceil(Math.max(x1, x2, x3));
    const minY = Math.floor(Math.min(y1, y2, y3));
    const maxY = Math.ceil(Math.max(y1, y2, y3));

    for (let y = minY; y <= maxY; y++) {
        for (let x = minX; x <= maxX; x++) {
            if (pointInTriangle(x, y, x1, y1, x2, y2, x3, y3)) {
                setPixel(pixels, x, y, color);
            }
        }
    }
}

function pointInTriangle(px, py, x1, y1, x2, y2, x3, y3) {
    const d1 = sign(px, py, x1, y1, x2, y2);
    const d2 = sign(px, py, x2, y2, x3, y3);
    const d3 = sign(px, py, x3, y3, x1, y1);
    const hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
    const hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);
    return !(hasNeg && hasPos);
}

function sign(px, py, x1, y1, x2, y2) {
    return (px - x2) * (y1 - y2) - (x1 - x2) * (py - y2);
}

function fillDiamond(pixels, cx, cy, size, color) {
    for (let y = cy - size; y <= cy + size; y++) {
        for (let x = cx - size; x <= cx + size; x++) {
            if (Math.abs(x - cx) + Math.abs(y - cy) <= size) {
                setPixel(pixels, Math.round(x), Math.round(y), color);
            }
        }
    }
}

function fillStar(pixels, cx, cy, outerR, innerR, points, color) {
    const angleStep = Math.PI / points;
    const starPoints = [];

    for (let i = 0; i < points * 2; i++) {
        const r = i % 2 === 0 ? outerR : innerR;
        const angle = i * angleStep - Math.PI / 2;
        starPoints.push({
            x: cx + r * Math.cos(angle),
            y: cy + r * Math.sin(angle)
        });
    }

    // Fill star using triangles from center
    for (let i = 0; i < starPoints.length; i++) {
        const p1 = starPoints[i];
        const p2 = starPoints[(i + 1) % starPoints.length];
        fillTriangle(pixels, cx, cy, p1.x, p1.y, p2.x, p2.y, color);
    }
}

function fillEllipse(pixels, cx, cy, rx, ry, color) {
    for (let y = cy - ry; y <= cy + ry; y++) {
        for (let x = cx - rx; x <= cx + rx; x++) {
            const dx = (x - cx) / rx;
            const dy = (y - cy) / ry;
            if (dx * dx + dy * dy <= 1) {
                setPixel(pixels, Math.round(x), Math.round(y), color);
            }
        }
    }
}

function fillHexagon(pixels, cx, cy, size, color) {
    const points = [];
    for (let i = 0; i < 6; i++) {
        const angle = i * Math.PI / 3 - Math.PI / 2;
        points.push({
            x: cx + size * Math.cos(angle),
            y: cy + size * Math.sin(angle)
        });
    }

    for (let i = 0; i < 6; i++) {
        const p1 = points[i];
        const p2 = points[(i + 1) % 6];
        fillTriangle(pixels, cx, cy, p1.x, p1.y, p2.x, p2.y, color);
    }
}

function drawBackgroundShape(pixels, bgIndex, color) {
    const cx = WIDTH / 2;
    const cy = HEIGHT / 2;
    const scale = WIDTH / 64; // Scale from 64x64 to 256x256

    switch (bgIndex) {
        case 0: // None
            break;
        case 1: // Circle
            fillCircle(pixels, cx, cy, 28 * scale, color);
            break;
        case 2: // Square
            fillRect(pixels, 4 * scale, 4 * scale, 60 * scale, 60 * scale, color);
            break;
        case 3: // Triangle up
            fillTriangle(pixels, cx, 4 * scale, 60 * scale, 60 * scale, 4 * scale, 60 * scale, color);
            break;
        case 4: // Triangle down
            fillTriangle(pixels, cx, 60 * scale, 60 * scale, 4 * scale, 4 * scale, 4 * scale, color);
            break;
        case 5: // Diamond
            fillDiamond(pixels, cx, cy, 28 * scale, color);
            break;
        case 6: // Star
            fillStar(pixels, cx, cy, 28 * scale, 14 * scale, 5, color);
            break;
        case 7: // Oval horizontal
            fillEllipse(pixels, cx, cy, 28 * scale, 18 * scale, color);
            break;
        case 8: // Oval vertical
            fillEllipse(pixels, cx, cy, 18 * scale, 28 * scale, color);
            break;
        case 9: // Rounded rect (just use regular rect)
            fillRect(pixels, 4 * scale, 12 * scale, 60 * scale, 52 * scale, color);
            break;
        case 10: // Hexagon
            fillHexagon(pixels, cx, cy, 28 * scale, color);
            break;
        default:
            fillCircle(pixels, cx, cy, 28 * scale, color);
            break;
    }
}

function drawForegroundShape(pixels, fgIndex, color) {
    const cx = WIDTH / 2;
    const cy = HEIGHT / 2;
    const scale = WIDTH / 64;

    switch (fgIndex) {
        case 0: // None
            break;
        case 1: // 5-point star
            fillStar(pixels, cx, cy, 25 * scale, 10 * scale, 5, color);
            break;
        case 2: // Circle
            fillCircle(pixels, cx, cy, 18 * scale, color);
            break;
        case 3: // Square
            fillRect(pixels, 14 * scale, 14 * scale, 50 * scale, 50 * scale, color);
            break;
        case 4: // Triangle up
            fillTriangle(pixels, cx, 10 * scale, 54 * scale, 54 * scale, 10 * scale, 54 * scale, color);
            break;
        case 5: // Triangle down
            fillTriangle(pixels, cx, 54 * scale, 54 * scale, 10 * scale, 10 * scale, 10 * scale, color);
            break;
        case 6: // Diamond
            fillDiamond(pixels, cx, cy, 24 * scale, color);
            break;
        case 7: // Shield
            fillEllipse(pixels, cx, cy, 22 * scale, 24 * scale, color);
            break;
        case 8: // Octagon
            fillHexagon(pixels, cx, cy, 22 * scale, color);
            break;
        case 9: // Cross/plus
            fillRect(pixels, cx - 6 * scale, 14 * scale, cx + 6 * scale, 50 * scale, color);
            fillRect(pixels, 14 * scale, cy - 6 * scale, 50 * scale, cy + 6 * scale, color);
            break;
        case 10: // Star outline (smaller star)
            fillStar(pixels, cx, cy, 20 * scale, 8 * scale, 5, color);
            break;
        case 11: // T shape
            fillRect(pixels, 20 * scale, 20 * scale, 44 * scale, 28 * scale, color);
            fillRect(pixels, 28 * scale, 28 * scale, 36 * scale, 44 * scale, color);
            break;
        case 12: // Number 7
            fillRect(pixels, 16 * scale, 16 * scale, 48 * scale, 24 * scale, color);
            fillRect(pixels, 36 * scale, 24 * scale, 44 * scale, 48 * scale, color);
            break;
        case 13: // X
        case 14: // X lines
            // Draw X shape
            for (let i = 0; i < 28 * scale; i++) {
                fillRect(pixels, 16 * scale + i - 3 * scale, 16 * scale + i - 3 * scale,
                         16 * scale + i + 3 * scale, 16 * scale + i + 3 * scale, color);
                fillRect(pixels, 48 * scale - i - 3 * scale, 16 * scale + i - 3 * scale,
                         48 * scale - i + 3 * scale, 16 * scale + i + 3 * scale, color);
            }
            break;
        case 15: // Crosshairs
            fillRect(pixels, cx - 2 * scale, 12 * scale, cx + 2 * scale, 52 * scale, color);
            fillRect(pixels, 12 * scale, cy - 2 * scale, 52 * scale, cy + 2 * scale, color);
            break;
        default:
            if (fgIndex > 0) {
                // Default to star for unknown shapes
                fillStar(pixels, cx, cy, 25 * scale, 10 * scale, 5, color);
            }
            break;
    }
}

// PNG encoding using zlib
function encodePng(pixels, width, height) {
    // Build raw image data with filter bytes
    const rawData = Buffer.alloc(height * (1 + width * 4));

    for (let y = 0; y < height; y++) {
        const rowOffset = y * (1 + width * 4);
        rawData[rowOffset] = 0; // Filter type: None

        for (let x = 0; x < width; x++) {
            const srcOffset = (y * width + x) * 4;
            const dstOffset = rowOffset + 1 + x * 4;
            rawData[dstOffset] = pixels[srcOffset];     // R
            rawData[dstOffset + 1] = pixels[srcOffset + 1]; // G
            rawData[dstOffset + 2] = pixels[srcOffset + 2]; // B
            rawData[dstOffset + 3] = pixels[srcOffset + 3]; // A
        }
    }

    // Compress with zlib
    const compressed = zlib.deflateSync(rawData, { level: 6 });

    // Build PNG file
    const chunks = [];

    // PNG signature
    chunks.push(Buffer.from([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]));

    // IHDR chunk
    const ihdr = Buffer.alloc(13);
    ihdr.writeUInt32BE(width, 0);
    ihdr.writeUInt32BE(height, 4);
    ihdr[8] = 8;  // Bit depth
    ihdr[9] = 6;  // Color type: RGBA
    ihdr[10] = 0; // Compression
    ihdr[11] = 0; // Filter
    ihdr[12] = 0; // Interlace
    chunks.push(createChunk('IHDR', ihdr));

    // IDAT chunk(s)
    chunks.push(createChunk('IDAT', compressed));

    // IEND chunk
    chunks.push(createChunk('IEND', Buffer.alloc(0)));

    return Buffer.concat(chunks);
}

function createChunk(type, data) {
    const typeBuffer = Buffer.from(type, 'ascii');
    const length = Buffer.alloc(4);
    length.writeUInt32BE(data.length, 0);

    const crcData = Buffer.concat([typeBuffer, data]);
    const crc = Buffer.alloc(4);
    crc.writeUInt32BE(crc32(crcData), 0);

    return Buffer.concat([length, typeBuffer, data, crc]);
}

// CRC32 calculation for PNG chunks
const crcTable = (function() {
    const table = new Uint32Array(256);
    for (let n = 0; n < 256; n++) {
        let c = n;
        for (let k = 0; k < 8; k++) {
            if (c & 1) {
                c = 0xEDB88320 ^ (c >>> 1);
            } else {
                c = c >>> 1;
            }
        }
        table[n] = c;
    }
    return table;
})();

function crc32(data) {
    let crc = 0xFFFFFFFF;
    for (let i = 0; i < data.length; i++) {
        crc = crcTable[(crc ^ data[i]) & 0xFF] ^ (crc >>> 8);
    }
    return (crc ^ 0xFFFFFFFF) >>> 0;
}

server.listen(PORT, '0.0.0.0', () => {
    console.log(`H2 Emblem Service running on port ${PORT}`);
    console.log(`Test URL: http://localhost:${PORT}/P3-S18-EP2-ES27-EF1-EB1-ET0.png`);
    console.log(`Health: http://localhost:${PORT}/health`);
});
