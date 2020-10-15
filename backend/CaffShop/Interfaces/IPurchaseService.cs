using System.Collections.Generic;
using System.Threading.Tasks;
using CaffShop.Entities;

namespace CaffShop.Interfaces
{
    public interface IPurchaseService
    {
        Task<Purchase> PurchaseItem(int itemId, int userId);
        Task<List<Purchase>> GetItemPurchases(int itemId);
        Task<int> CountItemPurchases(int itemId);
        Task<bool> IsUserPurchasedItem(int itemId, int userId);
    }
}