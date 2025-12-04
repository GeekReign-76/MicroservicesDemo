import { precacheAndRoute } from 'workbox-precaching';
import { registerRoute } from 'workbox-routing';
import { NetworkFirst } from 'workbox-strategies';

precacheAndRoute(self._WB_MANIFEST || []);
registerRoute(
    ({url}) => url.pathname.startsWith('/api/'),
    new NetworkFirst({cacheName: 'api-cache', networkTimeoutSeconds: 5})
);