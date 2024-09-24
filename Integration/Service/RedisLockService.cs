using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Service
{
    public class RedisLockService
    {
        private readonly IDatabase _redisDatabase;
        private readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(30);  

        public RedisLockService(ConnectionMultiplexer redisConnection)
        {
            _redisDatabase = redisConnection.GetDatabase();
        }

        public async Task<bool> AcquireLockAsync(string lockKey)
        { 
            return await _redisDatabase.StringSetAsync(lockKey, "locked", _lockTimeout, When.NotExists);
        }

        public async Task ReleaseLockAsync(string lockKey)
        {
            // Kilidi sil
            await _redisDatabase.KeyDeleteAsync(lockKey);
        }
    }
}
