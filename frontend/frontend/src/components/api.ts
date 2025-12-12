import { DataRecord } from './types';
import { submitData } from '../api/apiClient'; // correct relative path

export const postRecord = async (record: DataRecord): Promise<DataRecord> => {
  return submitData(record); // forwards to BFF
};
