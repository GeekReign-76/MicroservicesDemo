const CACHE_NAME = 'pwa-cache-v1';
const ASSETS_TO_CACHE = [
  '/',                // index route
  '/index.html',      // main HTML
  '/favicon.ico',
  '/manifest.json',
  '/handdrawn_192x192.png',
  '/handdrawn_512x512.png'
];

// Install event: cache app shell
self.addEventListener('install', event => {
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then(async cache => {
        // Fetch asset manifest to cache hashed JS/CSS
        try {
          const res = await fetch('/asset-manifest.json');
          const manifest = await res.json();
          const files = Object.values(manifest.files);
          await cache.addAll([...ASSETS_TO_CACHE, ...files]);
        } catch (err) {
          console.warn('Failed to cache some assets:', err);
          await cache.addAll(ASSETS_TO_CACHE);
        }
      })
  );
  self.skipWaiting();
});

// Activate event: clean up old caches
self.addEventListener('activate', event => {
  event.waitUntil(
    caches.keys().then(keys =>
      Promise.all(keys.map(key => key !== CACHE_NAME && caches.delete(key)))
    )
  );
  self.clients.claim();
});

// IndexedDB for queued POST requests
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

  store.openCursor().onsuccess = async event => {
    const cursor = event.target.result;
    if (!cursor) return;

    const req = cursor.value;
    try {
      await fetch(req.url, {
        method: req.method,
        headers: req.headers,
        body: req.body
      });
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

  // Cache-first for GET requests
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

  // Queue POST requests if offline
  if (event.request.method === 'POST' && url.pathname.startsWith('/api/')) {
    event.respondWith(
      fetch(event.request.clone())
        .catch(async () => {
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

// Sync queued POST requests when back online
self.addEventListener('sync', event => {
  if (event.tag === 'replay-requests') {
    event.waitUntil(replayRequests());
  }
});

self.addEventListener('message', event => {
  if (event.data === 'sync-requests') {
    event.waitUntil(replayRequests());
  }
});
