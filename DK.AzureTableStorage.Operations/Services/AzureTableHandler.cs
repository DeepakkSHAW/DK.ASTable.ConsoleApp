using DK.AzureTableStorage.Operations.Models;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;


namespace DK.AzureTableStorage.Operations.Services
{
    //public interface IAzureTableHandler
    //{

    //}

        public class AzureTableHandler : IAzureTableHandler
    {
        private readonly IConfiguration _config;
        private readonly string _AzureStorageConfig = string.Empty;
        private readonly string _TableName = string.Empty;
        public AzureTableHandler(IConfiguration config)
        {
            _config = config;
            _AzureStorageConfig = _config.GetValue<string>($"AzureTableConfig:connection_string");
            _TableName = _config.GetValue<string>($"AzureTableConfig:TableName");
        }
        #region Private Methods
        private async Task<object> ExecuteTableOperation(TableOperation tableOperation)
        {
            var table = await GetCloudTable();
            var tableResult = await table.ExecuteAsync(tableOperation);
            return tableResult.Result;
        }

        private async Task<CloudTable> GetCloudTable()
        {
            var storageAccount = CloudStorageAccount.Parse(_AzureStorageConfig);
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var table = tableClient.GetTableReference(_TableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }
        #endregion
        public async Task<OrderItemEntity> NewOrderAsync(OrderItemEntity order)
        {
            var insertOrMergeOperation = TableOperation.InsertOrMerge(order);
            return await ExecuteTableOperation(insertOrMergeOperation) as OrderItemEntity;
        }

        public async Task<List<OrderItemEntity>> GetOrdersAsync()
        {
            var table = await GetCloudTable();
            TableContinuationToken token = null;
            var tableResult = await table.ExecuteQuerySegmentedAsync(new TableQuery<OrderItemEntity>(), token);
            return tableResult.Results;
        }

        public async Task<List<OrderItemEntity>> GetOrdersAsync(string customerID)
        {
            var table = await GetCloudTable();
            List<OrderItemEntity> tableResult = new();
            TableQuery<OrderItemEntity> query = new TableQuery<OrderItemEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, customerID));
            tableResult = table.ExecuteQuery<OrderItemEntity>(query).ToList();
            return tableResult;
        }

        public async Task<List<OrderItemEntity>> GetOrdersAsync(string customerID, DateTime? orderDate)
        {
            var startDate = orderDate.HasValue ? orderDate.Value: DateTime.MinValue;
            var nextDate = startDate.AddDays(1).Date;
            var table = await GetCloudTable();

            List<OrderItemEntity> tableResult = new();
            string filter = $"PartitionKey eq '{customerID}' and OrderDate ge datetime'{startDate.ToString("yyyy-MM-dd")}' and OrderDate lt datetime'{nextDate.ToString("yyyy-MM-dd")}'";

            TableQuery<OrderItemEntity> query = new TableQuery<OrderItemEntity>().Where(filter);//.Take(50);
            tableResult = table.ExecuteQuery<OrderItemEntity>(query).ToList();
            return tableResult;
        }

        public async Task<OrderItemEntity> GetOrdersAsync(string customerID, string orderID)
        {
            var retrieveOperation = TableOperation.Retrieve<OrderItemEntity>(customerID, orderID);
            return await ExecuteTableOperation(retrieveOperation) as OrderItemEntity;
        }

        public async Task<OrderItemEntity> DeleteOrdersAsync(OrderItemEntity order)
        {
            var deleteOperation = TableOperation.Delete(order);
            return await ExecuteTableOperation(deleteOperation) as OrderItemEntity;
        }

        public async Task<OrderItemEntity> UpdateOrdersAsync(OrderItemEntity order)
        {
            var insertOrMergeOperation = TableOperation.Merge(order);
            return await ExecuteTableOperation(insertOrMergeOperation) as OrderItemEntity;
        }
    }
}