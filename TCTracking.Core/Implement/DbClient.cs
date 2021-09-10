using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TCTracking.Core.Interface;
using TCTracking.Core.Models;

namespace TCTracking.Core.Implement
{
    public class DbClient : IDbClient
    {
        private readonly IMongoCollection<TCS> _tcs;
        private readonly IMongoCollection<Users> _users;
        private readonly IMongoCollection<Notification> _notification;

        public DbClient(IOptions<TCTrackingDatabaseSettings> dbConfig)
        {
            var client = new MongoClient(dbConfig.Value.Connection_String);
            var database = client.GetDatabase(dbConfig.Value.Database_Name);
            _tcs = database.GetCollection<TCS>(dbConfig.Value.TCS_Collection_Name);
            _users = database.GetCollection<Users>(dbConfig.Value.Users_Collection_Name);
            _notification = database.GetCollection<Notification>(dbConfig.Value.Notification_Collection_Name);


        }

        public IMongoCollection<Notification> GetNotificationCollections()
        {
            return _notification;
        }

        public IMongoCollection<TCS> GetTCSCollections()
        {
            return _tcs;
        }

        public IMongoCollection<Users> GetUsersCollections()
        {
            return _users;
        }
    }
}
