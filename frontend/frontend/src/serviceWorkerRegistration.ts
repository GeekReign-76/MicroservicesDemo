// serviceWorkerRegistration.ts
export function register() {
  if ('serviceWorker' in navigator) {
    window.addEventListener('load', () => {
      navigator.serviceWorker
        .register('/service-worker.js')
        .then((registration) => {
          console.log('SW registered:', registration);

          // Function to request background sync
          async function requestSync() {
            try {
              // Some browsers don't yet support sync in TS types
              const regWithSync = registration as ServiceWorkerRegistration & { sync?: any };
              if (regWithSync.sync) {
                await regWithSync.sync.register('replay-requests');
                console.log('Background sync registered');
              } else {
                // Fallback: send message to SW
                registration.active?.postMessage('sync-requests');
                console.log('Sync fallback message sent to SW');
              }
            } catch (err) {
              console.warn('Background sync failed:', err);
            }
          }

          // Try to replay queued requests when browser comes online
          window.addEventListener('online', requestSync);
        })
        .catch((error) => {
          console.error('SW registration failed:', error);
        });
    });
  } else {
    console.log('Service Worker not supported in this browser.');
  }
}
