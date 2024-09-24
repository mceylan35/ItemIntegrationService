using Integration.Service;
using StackExchange.Redis;

namespace Integration;

public abstract class Program
{
    public static void Main(string[] args)
    {
        var service = new ItemIntegrationService();
        
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("a"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("b"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("c"));

        Thread.Sleep(500);

        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("a"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("b"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("c"));

        Thread.Sleep(5000);

        Console.WriteLine("Everything recorded:");

        service.GetAllItems().ForEach(Console.WriteLine);
         

        ///DistributedItemIntegrationService

        var redisConnection =   ConnectionMultiplexer.ConnectAsync("localhost:6379").Result;
        var redisLockService = new RedisLockService(redisConnection);

      
        var distributedItemIntegrationService = new DistributedItemIntegrationService(redisLockService);
         
        ThreadPool.QueueUserWorkItem(async _ => Console.WriteLine(await distributedItemIntegrationService.SaveItemAsync("a")));
        ThreadPool.QueueUserWorkItem(async _ => Console.WriteLine(await distributedItemIntegrationService.SaveItemAsync("b")));
        ThreadPool.QueueUserWorkItem(async _ => Console.WriteLine(await distributedItemIntegrationService.SaveItemAsync("c")));

        Thread.Sleep(500);
         
        ThreadPool.QueueUserWorkItem(async _ => Console.WriteLine(await distributedItemIntegrationService.SaveItemAsync("a")));
        ThreadPool.QueueUserWorkItem(async _ => Console.WriteLine(await distributedItemIntegrationService.SaveItemAsync("b")));
        ThreadPool.QueueUserWorkItem(async _ => Console.WriteLine(await distributedItemIntegrationService.SaveItemAsync("c")));

        // İşlemlerin tamamlanması için yeterli süre
        Thread.Sleep(5000);

        Console.WriteLine("Everything recorded:");

        // Kayıtlı tüm öğeleri listele
        service.GetAllItems().ForEach(Console.WriteLine);

        Console.ReadLine();
    }
}
}