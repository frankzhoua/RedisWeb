export interface DataModel {
    name?: string
    region?: string
    subscription?: string
    group?: string
    port?: string
    cases?: string[]
    quantity?: string
    numKeysPerShard?: string
}
export interface PerfModel {
    subscription?: string
    group?: string
    sku?: string
}
export interface ManModel {
    name?: string
    subscription?: string
    group?: string
    region?: string
    cases?: string[]
    quantity?: string
}
