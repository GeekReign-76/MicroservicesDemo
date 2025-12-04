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

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const data = await postRecord(record);
      setResult(data);
    } catch (err) {
      console.error(err);
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

      {result && (
        <pre>{JSON.stringify(result, null, 2)}</pre>
      )}
    </div>
  );
};
