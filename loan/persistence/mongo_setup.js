// Mongo setup for Loan service: collection and helpful indexes
db = db.getSiblingDB('banking_loan');

db.createCollection('loanApplications');

// Indexes to support common queries
db.loanApplications.createIndex({ customerId: 1 });
db.loanApplications.createIndex({ status: 1 });
