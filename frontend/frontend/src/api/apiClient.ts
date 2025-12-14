import axios from 'axios';
import { DataRecord } from '../components/types';

// In-cluster service DNS
const api = axios.create({
  baseURL: 'http://bff:4000/api', // Use the BFF Kubernetes service name
  headers: { 'Content-Type': 'application/json' },
});

export const submitData = async (data: DataRecord) => {
  const response = await api.post('/submit', data);
  return response.data;
};
