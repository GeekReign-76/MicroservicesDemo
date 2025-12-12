const CACHE_NAME = 'pwa-cache-v1';
const ASSETS_TO_CACHE = [
  '/',
  '/index.html',
  '/static/js/bundle.js',
  '/static/css/main.css',
  '/favicon.ico',
  '/manifest.json',
  '/handdrawn_192x192.png'
];

// Install event: cache app shell
self.addEventListener('install', event => {
  event.waitUntil(
    caches.open(CACHE_NAME).then(cache => cache.addAll(ASSETS_TO_CACHE))
  );
  self.skipWaiting();
});

// Activate event: clean up old caches
self.addEventListener('activate', event => {
  event.waitUntil(
    caches.keys().then(keys =>
      Promise.all(
        keys.map(key => key !== CACHE_NAME && caches.delete(key))
      )
    )
  );
  self.clients.claim();
});

// IndexedDB setup for queued requests
const DB_NAME = 'offline-requests';
const DB_STORE = 'requests';

function openDB() {
  return new Promise((resolve, reject) => {
    const request = indexedDB.open(DB_NAME, 1);
    request.onupgradeneeded = () => {
      const db = request.result;
      db.createObjectStore(DB_STORE, { autoIncrement: true });
    };
    request.onsuccess = () => resolve(request.result);
    request.onerror = () => reject(request.error);
  });
}

async function saveRequest(request) {
  const db = await openDB();
  const tx = db.transaction(DB_STORE, 'readwrite');
  tx.objectStore(DB_STORE).add(request);
  await tx.complete;
  db.close();
}

async function replayRequests() {
  const db = await openDB();
  const tx = db.transaction(DB_STORE, 'readwrite');
  const store = tx.objectStore(DB_STORE);

  const allRequests = [];
  store.openCursor().onsuccess = async event => {
    const cursor = event.target.result;
    if (!cursor) return;

    const req = cursor.value;
    try {
      const response = await fetch(req.url, {
        method: req.method,
        headers: req.headers,
        body: req.body
      });

      // Parse response as JSON
      const data = await response.clone().json();

      // Notify all clients about the synced request
      const clients = await self.clients.matchAll();
      clients.forEach(client => {
        client.postMessage({ type: 'sw-api-result', data });
      });

      // Remove request from store
      store.delete(cursor.key);
    } catch (err) {
      console.log('Retry failed, will try later', err);
    }
    cursor.continue();
  };

  await tx.complete;
  db.close();
}

// Fetch event
self.addEventListener('fetch', event => {
  const url = new URL(event.request.url);

  // Handle GET requests (cache-first)
  if (event.request.method === 'GET') {
    event.respondWith(
      caches.match(event.request).then(cachedResponse =>
        cachedResponse || fetch(event.request).then(networkResponse => {
          caches.open(CACHE_NAME).then(cache =>
            cache.put(event.request, networkResponse.clone())
          );
          return networkResponse;
        }).catch(() => {
          if (event.request.destination === 'document') {
            return caches.match('/index.html');
          }
        })
      )
    );
  }

  // Handle POST requests to API (queue if offline)
  if (event.request.method === 'POST' && url.pathname.startsWith('/api/')) {
    event.respondWith(
      fetch(event.request.clone())
        .catch(async () => {
          // Save request for later
          const reqClone = {
            url: event.request.url,
            method: event.request.method,
            headers: Object.fromEntries(event.request.headers),
            body: await event.request.clone().text()
          };
          await saveRequest(reqClone);
          return new Response(JSON.stringify({ offline: true }), {
            headers: { 'Content-Type': 'application/json' }
          });
        })
    );
  }
});

// Listen for online event to replay requests
self.addEventListener('sync', event => {
  if (event.tag === 'replay-requests') {
    event.waitUntil(replayRequests());
  }
});

// Listen for messages from the page to manually trigger replay
self.addEventListener('message', event => {
  if (event.data === 'sync-requests') {
    event.waitUntil(replayRequests());
  }
});
