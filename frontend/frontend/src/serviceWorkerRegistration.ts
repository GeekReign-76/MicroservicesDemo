export function register(){
    if ('serviceWorker' in navigator){
        window.addEventListener('load', () => {
            navigator.serviceWorker
                .register('/service-worker.js')
                .then(reg=> console.log('SW regitered', reg))
                .catch(err => console.error('SW registration failed', err));
        });
    }
}