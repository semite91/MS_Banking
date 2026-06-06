// Mongo setup for Fraud service: create collection and indexes
db = db.getSiblingDB('banking_fraud');

db.createCollection('traces');

// Index on transactionId and createdAt for fast search
db.traces.createIndex({ transactionId: 1, createdAt: -1 });
