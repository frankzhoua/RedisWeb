export interface DataModel {
  name?: string;
  region?: string;
  subscription?: string;
  group?: string;
  port?: string;
  cases?: string[];
  quantity?: string;
  numKeysPerShard?: string;
  // 添加其他字段
}
export interface PerfModel {
  subscription?: string;
  group?: string;
  sku?: string;
}
export interface ManModel {
  subscription?: string;
  group?: string;
  region?: string;
}
