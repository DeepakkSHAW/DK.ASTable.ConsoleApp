using Microsoft.Azure.Cosmos.Table;

namespace DK.AzureTableStorage.Operations.Models
{
    public class OrderItemEntity : TableEntity
    {
        public OrderItemEntity() { }

        public OrderItemEntity(string CustID)
        {
            this.PartitionKey = CustID;
            this.RowKey = Guid.NewGuid().ToString();
        }

        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string OrderRemarks { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    }
}
