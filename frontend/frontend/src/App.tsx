import React from 'react';
import './App.css';
import { RecordForm } from './components/Form'; // matches the named export

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <h1>Processing Service</h1>
        <RecordForm />
      </header>
    </div>
  );
}

export default App;
