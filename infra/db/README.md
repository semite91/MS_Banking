Database DDL & setup

Files:
- auth/persistence/ddl_postgres.sql
- customer/persistence/ddl_postgres.sql
- transaction/persistence/ddl_postgres.sql
- payment/persistence/ddl_postgres.sql
- fraud/persistence/ddl_postgres.sql
- fraud/persistence/mongo_setup.js
- loan/persistence/mongo_setup.js
- account/persistence/ddl_mssql.sql

Notes:
- Run Postgres DDLs against your Postgres instance. Ensure `pgcrypto` extension is enabled for `gen_random_uuid()`.
- Run MSSQL DDL against your SQL Server instance.
- Run Mongo JS files via `mongosh` to create collections and indexes.

Example:

```bash
psql -h localhost -U postgres -d banking -f auth/persistence/ddl_postgres.sql
mongosh --file fraud/persistence/mongo_setup.js
```
