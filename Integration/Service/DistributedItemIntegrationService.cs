using Integration.Backend;
using Integration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Service
{
    public class DistributedItemIntegrationService
    {
        private ItemOperationBackend ItemIntegrationBackend { get; set; } = new();
        private readonly RedisLockService _redisLockService;
        private const string RedisKeyPrefix = "item-lock:";   

        public DistributedItemIntegrationService(RedisLockService redisLockService)
        {
            _redisLockService = redisLockService;
        }

        public async Task<Result> SaveItemAsync(string itemContent)
        {
            string lockKey = $"{RedisKeyPrefix}{itemContent}";

            try
            {

                if (!await _redisLockService.AcquireLockAsync(lockKey))
                {
                    return new Result(false, $"'{itemContent}' şu anda farklı bir servis tarafından işleniyor.");
                }

              
                if (ItemIntegrationBackend.FindItemsWithContent(itemContent).Count != 0)
                {
                    return new Result(false, $"Aynı içerik tekrar alındı: {itemContent}.");
                }

               
                var item = ItemIntegrationBackend.SaveItem(itemContent);
                return new Result(true, $"İçerik kaydedildi: {itemContent}, id: {item.Id}");
            }
            finally
            {
                // Redisten sil
                await _redisLockService.ReleaseLockAsync(lockKey);
            }
        }

        public List<Item> GetAllItems()
        {
            return ItemIntegrationBackend.GetAllItems();
        }
    }
}
