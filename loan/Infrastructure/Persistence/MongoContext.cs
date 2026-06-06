using Loan.Domain.Entities;
using MongoDB.Driver;

namespace Loan.Infrastructure.Persistence
{
    public class MongoContext
    {
        private readonly IMongoDatabase _db;
        public MongoContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _db = client.GetDatabase(databaseName);
        }

        public IMongoCollection<LoanApplication> LoanApplications => _db.GetCollection<LoanApplication>("loanApplications");
    }
}
