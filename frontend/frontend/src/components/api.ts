import { DataRecord } from './types';

export const postRecord = async (record: DataRecord): Promise<DataRecord> => {
  const response = await fetch('http://localhost:5035/api/process', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(record),
  });

  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }

  return response.json();
};
