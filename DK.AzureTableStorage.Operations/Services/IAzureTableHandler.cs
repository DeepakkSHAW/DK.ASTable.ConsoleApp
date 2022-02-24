using DK.AzureTableStorage.Operations.Models;

namespace DK.AzureTableStorage.Operations.Services
{
    public interface IAzureTableHandler
    {
        public Task<OrderItemEntity> NewOrderAsync(OrderItemEntity order);
        public Task<List<OrderItemEntity>> GetOrdersAsync();
        public Task<List<OrderItemEntity>> GetOrdersAsync(string customerID);
        public Task<OrderItemEntity> GetOrdersAsync(string customerID, string orderID);
        public Task<List<OrderItemEntity>> GetOrdersAsync(string customerID, DateTime? orderDate);
        public Task<OrderItemEntity> DeleteOrdersAsync(OrderItemEntity order);
        public Task<OrderItemEntity> UpdateOrdersAsync(OrderItemEntity order);
    }
}
