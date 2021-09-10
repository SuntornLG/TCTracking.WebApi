using MongoDB.Driver;
using TCTracking.Core.Models;

namespace TCTracking.Core.Interface
{
    public interface IDbClient
    {
        IMongoCollection<TCS> GetTCSCollections();
        IMongoCollection<Users> GetUsersCollections();

        IMongoCollection<Notification> GetNotificationCollections();

       // IMongoCollection<Notification> GetTrackingAlertCollections();
    }
}
