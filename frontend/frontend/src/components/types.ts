export interface DataRecord {
  id?: number;           // optional, returned by API
  name: string;
  value: number;
  metadata: Record<string, any>;
  processedAt?: string;  // optional, returned by API
}
