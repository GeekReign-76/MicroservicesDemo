import React, { useState } from 'react';
import { DataRecord } from './types';
import { postRecord } from './api';

export const RecordForm: React.FC = () => {
  const [record, setRecord] = useState<DataRecord>({
    name: '',
    value: 0,
    metadata: {}
  });
  const [result, setResult] = useState<DataRecord | null>(null);
  const [statusMessage, setStatusMessage] = useState<string>('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setStatusMessage('Submitting record...');
    console.log('Submit clicked', record);

    try {
      const data = await postRecord(record);

      if ((data as any).offline) {
        // Service worker queued the request
        console.log('Record queued for submission when back online.');
        setStatusMessage('You are offline. Your record will be submitted when online.');
        setResult(null);
      } else {
        // Successfully submitted
        console.log('Response from API:', data);
        setStatusMessage('Record submitted successfully!');
        setResult(data);
      }
    } catch (err) {
      console.error('Error submitting record:', err);
      setStatusMessage('Error submitting record. Please try again.');
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

      {statusMessage && <p>{statusMessage}</p>}

      {result && (
        <pre>{JSON.stringify(result, null, 2)}</pre>
      )}
    </div>
  );
};
