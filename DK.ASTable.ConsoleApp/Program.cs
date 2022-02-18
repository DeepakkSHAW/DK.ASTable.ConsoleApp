using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DK.AzureTableStorage.Operations;
using DK.AzureTableStorage.Operations.Models;

namespace DK.ASTable.ConsoleApp
{
    class Program
    {
        private static readonly string _ConfigFilePath = "appsettings.json";
        static async Task Main(string[] args)
        {
            Console.WriteLine("Azure Table Storage - testing Console App");

            try
            {
                var host = AppStartup();
                var azTHandler = ActivatorUtilities.CreateInstance<AzureTableHandler>(host.Services);

                //New Order >> POST Operation
                var aOrder = new OrderItemEntity("CUST_001") { ItemName = "Milk", Price = 5.5, Quantity = 2, OrderRemarks = "2 Suger each" };
                var newOrder = await azTHandler.NewOrderAsync(aOrder);
                DisplayOrder(newOrder, "--New Order Placed--");

                //Find Order >> GET Operation
                var foundOrder = await azTHandler.GetOrdersAsync(newOrder.PartitionKey, newOrder.RowKey);
                DisplayOrder(newOrder, $"--A Order Found--");

                //Find All orders from a customer >> Get All from Partition Key
                var allOrders = await azTHandler.GetOrdersAsync(newOrder.PartitionKey);
                Console.WriteLine($"--All Orders from a customer --"); foreach (var order in allOrders) DisplayOrder(order);

                //Find All orders from a customer for a date >> Get All from Partition Key & Date
                var allDorderForToday = await azTHandler.GetOrdersAsync(newOrder.PartitionKey, DateTime.UtcNow);
                Console.WriteLine($"--All Orders from a customer and Date--"); foreach (var order in allDorderForToday) DisplayOrder(order);

                //All orders for all customer >> Get All row from Table
                var allDorderForAllCustomers = await azTHandler.GetOrdersAsync();
                Console.WriteLine($"--All Orders from Table--"); foreach (var order in allDorderForAllCustomers) DisplayOrder(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #region Private methods
        static void DisplayOrder(OrderItemEntity order, string header = "")
        {
            if (header != "") Console.WriteLine($"-----{header}----");
            if (order != null)
            {
                Console.WriteLine($"CustomerID: {order.PartitionKey}" +
                    $" OrderID: {order.RowKey}" +
                    $" Item Name: {order.ItemName}" +
                    $" Quantity: {order.Quantity}" +
                    $" Price: {order.Price}" +
                    $" Order Remarks: {order.OrderRemarks}" +
                    $" Order Date: {order.OrderDate}"
                    );
            }
            else
            {
                Console.WriteLine($"No Order found");
            }
            if (header != "") Console.WriteLine($"------------------");
        }
        static void ConfigSetup(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(_ConfigFilePath, optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables();
        }
        static IHost AppStartup()
        {
            var builder = new ConfigurationBuilder();
            ConfigSetup(builder);

            var host = Host.CreateDefaultBuilder()
                       .ConfigureServices((context, services) =>
                       {
                           services.AddTransient<IAzureTableHandler, AzureTableHandler>();
                           //services.AddTransient<IMap, Map>();
                           //services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("blah-blah"));
                       })
                       //.UseSerilog()
                       .Build();
            return host;
        }
        #endregion
    }
}


