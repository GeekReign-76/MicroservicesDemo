import React, { useState, useEffect } from 'react';
import { DataRecord } from './types';
import { postRecord } from './api';

export const RecordForm: React.FC = () => {
  const [record, setRecord] = useState<DataRecord>({
    name: '',
    value: 0,
    metadata: {}
  });
  const [result, setResult] = useState<DataRecord | null>(null);
  const [message, setMessage] = useState<string | null>(null);

  // Listen for service worker messages (replayed requests)
  useEffect(() => {
    const handleSWMessage = (event: MessageEvent) => {
      const { type, data } = event.data;
      if (type === 'sw-api-result') {
        setResult(data);
        setMessage('Request synced from offline queue.');
      }
    };
    navigator.serviceWorker?.addEventListener('message', handleSWMessage);
    return () => {
      navigator.serviceWorker?.removeEventListener('message', handleSWMessage);
    };
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    console.log('Submit clicked', record);
    setMessage(null);
    setResult(null);

    try {
      const data = await postRecord(record);
      console.log('Response from API:', data);
      setResult(data);
    } catch (err: any) {
      // If service worker queued the request while offline
      if (err.response?.data?.offline) {
        setMessage('You are offline. Request queued for later.');
      } else {
        console.error('Error submitting record:', err);
        setMessage('Error submitting record.');
      }
    }
  };

  return (
    <div>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          placeholder="Name"
          value={record.name}
          onChange={e => setRecord({ ...record, name: e.target.value })}
        />
        <input
          type="number"
          placeholder="Value"
          value={record.value}
          onChange={e => setRecord({ ...record, value: +e.target.value })}
        />
        <button type="submit">Submit</button>
      </form>

      {message && <p>{message}</p>}
      {result && <pre>{JSON.stringify(result, null, 2)}</pre>}
    </div>
  );
};
