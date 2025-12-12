import axios from 'axios';
import { DataRecord } from '../components/types'; // adjust relative path if needed

const api = axios.create({
  baseURL: 'http://localhost:4000/api', // BFF endpoint
  headers: { 'Content-Type': 'application/json' },
});

export const submitData = async (data: DataRecord) => {
  const response = await api.post('/submit', data);
  return response.data;
};
