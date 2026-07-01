// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [/\.dll\.br$/, /\.pdb$/, /\.wasm\.br/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/];
// 排除 OCR 大文件：不在安装时预缓存（避免不用 OCR 的用户首次访问背负 ~21MB），
// 改为按需 cache-first（见 onFetch）。OCR 资产走 Git LFS / 单独 fetch，按需缓存即可。
const offlineAssetsExclude = [
    /^service-worker\.js$/,
    /\/ocr-models\//,                      // OCR 模型（det/rec/dict）
    /\/ocr-ort\//,                         // OCR 运行时（wasm.br / glue.js）
    /\/js\/ocr\/ocr\.selfhost\.bundle\.js$/ // OCR 引擎库 bundle
];

// OCR 大文件匹配规则：命中即走「按需 cache-first」分支
const ocrCachePatterns = [/\/ocr-models\//, /\/ocr-ort\//, /\/js\/ocr\/ocr\.selfhost\.bundle\.js$/];

// Replace with your base path if you are hosting on a subfolder. Ensure there is a trailing '/'.
const base = "/";
const baseUrl = new URL(base, self.origin);
const manifestUrlList = self.assetsManifest.assets.map(asset => new URL(asset.url, baseUrl).href);

async function onInstall(event) {
    console.info('Service worker: Install');

    // Fetch and cache all matching items from the assets manifest
    const assetsRequests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url, { integrity: asset.hash, cache: 'no-cache' }));
    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    // Delete unused caches
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));
}

async function onFetch(event) {
    let cachedResponse = null;
    if (event.request.method === 'GET') {
        // OCR 大文件：按需 cache-first（分层缓存策略）
        //   - 命中缓存 → 直接返回（省流量、断网可用）
        //   - 未命中   → fetch 后写入缓存再返回（下次起即可离线）
        //   ignoreVary：公网静态托管（如 EdgeOne）对所有文件统一回传 Vary 头，
        //   导致缓存误判，统一忽略 Vary 头匹配。
        const url = new URL(event.request.url);
        if (ocrCachePatterns.some(p => p.test(url.pathname))) {
            const ocrCache = await caches.open(cacheName);
            const hit = await ocrCache.match(event.request, { ignoreVary: true });
            if (hit) return hit;
            try {
                const resp = await fetch(event.request);
                if (resp.ok) ocrCache.put(event.request, resp.clone());
                return resp;
            } catch (_) {
                // 断网且无缓存：返回离线提示，避免页面静默失败
                return new Response('OCR 资源离线且未缓存，请联网加载一次。', {
                    status: 504, headers: { 'Content-Type': 'text/plain; charset=utf-8' }
                });
            }
        }

        // For all navigation requests, try to serve index.html from cache,
        // unless that request is for an offline resource.
        // If you need some URLs to be server-rendered, edit the following check to exclude those URLs
        const shouldServeIndexHtml = event.request.mode === 'navigate'
            && !manifestUrlList.some(url => url === event.request.url);

        const request = shouldServeIndexHtml ? 'index.html' : event.request;
        const cache = await caches.open(cacheName);
        cachedResponse = await cache.match(request);
    }

    return cachedResponse || fetch(event.request);
}
