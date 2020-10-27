using System.Collections.Generic;
using System.Threading.Tasks;
using CaffShop.Entities;

namespace CaffShop.Interfaces
{
    public interface IPurchaseService
    {
        Task<Purchase> PurchaseItem(long itemId, long userId);
        Task<List<Purchase>> GetItemPurchases(long itemId);
        Task<long> CountItemPurchases(long itemId);
        Task<bool> IsUserPurchasedItem(long itemId, long userId);
    }
}