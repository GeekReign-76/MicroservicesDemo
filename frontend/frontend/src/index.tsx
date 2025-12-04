import React from 'react';
import { createRoot } from 'react-dom/client';
import App from './App';
import { register } from './serviceWorkerRegistration';

const container = document.getElementById('root');
if (!container) throw new Error("Root container missing in index.html");

// Create the root
const root = createRoot(container);

// Render the app
root.render(<App />);

register();
