import axios from 'axios';
import { DataRecord } from '../components/types'; // adjust path relative to apiClient.ts

// Use the BFF endpoint
const api = axios.create({
  baseURL: 'http://localhost:5000/api',
  headers: { 'Content-Type': 'application/json' },
});

export const submitData = async (data: DataRecord) => {
  const response = await api.post('/submit', data);
  return response.data;
};
