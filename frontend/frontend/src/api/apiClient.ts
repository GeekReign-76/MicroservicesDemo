import axios from 'axios';
import { DataRecord } from '../components/types';

const api = axios.create({
  baseURL: 'http://localhost:30002/api', // BFF NodePort
  headers: { 'Content-Type': 'application/json' },
});

export const submitData = async (data: DataRecord) => {
  const response = await api.post('/submit', data);
  return response.data;
};
