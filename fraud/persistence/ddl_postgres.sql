-- Fraud Service DDL (Postgres)
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

CREATE TABLE IF NOT EXISTS fraud_alerts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    transaction_id UUID,
    payment_id UUID,
    score NUMERIC(5,2) NOT NULL,
    rule_id TEXT NOT NULL,
    status TEXT NOT NULL DEFAULT 'Open',
    details JSONB,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS idx_fraud_alerts_tx ON fraud_alerts(transaction_id);
CREATE INDEX IF NOT EXISTS idx_fraud_alerts_status ON fraud_alerts(status);

-- Optional: store rules and thresholds
CREATE TABLE IF NOT EXISTS fraud_rules (
    id TEXT PRIMARY KEY,
    description TEXT,
    severity INT,
    enabled BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);
